using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using engenious.Content.Pipeline;
using System.Linq.Expressions;

namespace ContentTool
{
    public static class PipelineHelper
    {
        private static IList<Type> _importers;
        private static readonly Dictionary<string, Type> Processors = new Dictionary<string, Type>();

        private static readonly List<KeyValuePair<Type, string>> ProcessorsByType =
            new List<KeyValuePair<Type, string>>();

        private static readonly List<Assembly> Assemblies = new List<Assembly>();

        public static void DefaultInit()
        {
            Assemblies.Clear();
            Assemblies.Add(Assembly.GetExecutingAssembly());
            Assemblies.Add(typeof(IContentImporter).Assembly);
            ListImporters();
            ListProcessors();
        }

        public static void PreBuilt(ContentProject currentProject)
        {
            if (currentProject == null)
                return;
            Assemblies.Clear();
            Assemblies.Add(Assembly.GetExecutingAssembly());
            Assemblies.Add(typeof(IContentImporter).Assembly);
            if (currentProject.References == null)
                currentProject.References = new List<string>();
            foreach (string reference in currentProject.References)
            {
                try
                {
                    if (System.IO.File.Exists(reference))
                        Assemblies.Add(Assembly.LoadFile(reference));
                }
                catch
                {
                }
            }
            ListImporters();
            ListProcessors();
        }

        private static void ListImporters()
        {
            _importers = EnumerateImporters().ToList();
        }

        public static List<string> GetProcessors(Type tp)
        {
            List<string> fitting = new List<string>();
            foreach (var pair in ProcessorsByType)
            {
                if (pair.Key.IsAssignableFrom(tp))
                {
                    fitting.Add(pair.Value);
                }
            }
            return fitting;
        }

        private static void ListProcessors()
        {
            Processors.Clear();
            ProcessorsByType.Clear();
            foreach (var assembly in Assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(IContentProcessor).IsAssignableFrom(type) && !(type.IsAbstract || type.IsInterface))
                    {
                        if (!Processors.ContainsKey(type.Name))
                            Processors.Add(type.Name, type);

                        var baseType = GetProcessorInputType(type);
                        ProcessorsByType.Add(new KeyValuePair<Type, string>(baseType, type.Name));
                    }
                }
            }
        }

        public static Type GetImporterOutputType(string extension)
        {
            var tp = GetImporterType(extension);
            var prop = tp.GetProperty("ExportType",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            if (prop == null)
                return typeof(object);
            var method = prop.GetGetMethod();
            var call = Expression.Call(method);
            var lambda = Expression.Lambda<Func<Type>>(call);
            var func = lambda.Compile();
            return func();
        }

        public static Type GetProcessorInputType(Type tp)
        {
            var prop = tp.GetProperty("ImportType",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            if (prop == null)
                return typeof(object);
            var method = prop.GetGetMethod();
            var call = Expression.Call(method);
            var lambda = Expression.Lambda<Func<Type>>(call);
            var func = lambda.Compile();
            return func();
        }

        public static Type GetImporterType(string extension)
        {
            foreach (var type in _importers)
            {
                var attribute =
                    (ContentImporterAttribute) type.GetCustomAttributes(typeof(ContentImporterAttribute), true).First();
                if (attribute.FileExtensions.Contains(extension))
                    return type;
            }
            return null;
        }

        public static IContentImporter CreateImporter(string extension)
        {
            if (_importers == null)
                DefaultInit();
            foreach (var type in _importers)
            {
                var attribute =
                    (ContentImporterAttribute) type.GetCustomAttributes(typeof(ContentImporterAttribute), true).First();
                if (attribute.FileExtensions != null && attribute.FileExtensions.Contains(extension))
                    return (IContentImporter) Activator.CreateInstance(type);
            }
            return null;
        }

        public static IContentProcessor CreateProcessor(Type importerType, string processorName)
        {
            Type type = null;
            if (!string.IsNullOrEmpty(processorName) && Processors.TryGetValue(processorName, out type))
                return (IContentProcessor) Activator.CreateInstance(type);

            var attribute =
                (ContentImporterAttribute)
                importerType.GetCustomAttributes(typeof(ContentImporterAttribute), true).First();
            if (Processors.TryGetValue(attribute.DefaultProcessor, out type))
                return (IContentProcessor) Activator.CreateInstance(type);

            return null;
        }


        private static TValue GetAttributeValue<TAttribute, TValue>(
            this Type type,
            Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            var att = type.GetCustomAttributes(
                typeof(TAttribute), true
            ).FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return valueSelector(att);
            }
            return default(TValue);
        }

        private static IEnumerable<Type> EnumerateImporters()
        {
            foreach (Assembly assembly in Assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (typeof(IContentImporter).IsAssignableFrom(type))
                    {
                        if (type.IsValueType || type.IsInterface || type.IsAbstract || type.ContainsGenericParameters ||
                            type.GetConstructor(
                                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                null, Type.EmptyTypes, null) == null)
                            continue;

                        var importerAttribute =
                            (ContentImporterAttribute)
                            Attribute.GetCustomAttributes(type, typeof(ContentImporterAttribute)).FirstOrDefault();
                        if (importerAttribute != null)
                        {
                            yield return type;
                        }
                    }
                }
            }
            yield break;
        }
    }
}