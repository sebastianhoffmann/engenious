using System;
using System.Collections.Generic;
using System.IO;
using engenious.Graphics;
using System.Reflection;
using engenious.Content.Serialization;
using System.Linq;

namespace engenious.Content
{
    public class ContentManager
    {
        private readonly Dictionary<string, IContentTypeReader> _typeReaders;
        private readonly Dictionary<string, IContentTypeReader> _typeReadersOutput;
        private readonly Dictionary<string, object> _assets;
        private readonly System.Runtime.Serialization.IFormatter _formatter;
        internal GraphicsDevice GraphicsDevice;

        public ContentManager(GraphicsDevice graphicsDevice)
            : this(
                graphicsDevice,
                Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) ?? "", "Content"))
        {
        }

        public ContentManager(GraphicsDevice graphicsDevice, string rootDirectory)
        {
            RootDirectory = rootDirectory;
            this.GraphicsDevice = graphicsDevice;
            _typeReaders = new Dictionary<string, IContentTypeReader>();
            _typeReadersOutput = new Dictionary<string, IContentTypeReader>();
            _assets = new Dictionary<string, object>();
            _formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            AddAssembly(Assembly.GetExecutingAssembly());
        }

        internal void AddAssembly(Assembly assembly)
        {
            foreach (var t in assembly.GetTypes())
            {
                if (t.GetInterfaces().Contains(typeof(IContentTypeReader)) && !(t.IsInterface || t.IsAbstract))
                {
                    var attr =
                        (ContentTypeReaderAttribute)
                        t.GetCustomAttributes(typeof(ContentTypeReaderAttribute), true).FirstOrDefault();
                    if (attr != null)
                    {
                        IContentTypeReader reader = Activator.CreateInstance(t) as IContentTypeReader;
                        _typeReaders.Add(t.FullName, reader);
                        if (attr.OutputType != null)
                            _typeReadersOutput.Add(attr.OutputType.FullName, reader);
                    }
                }
            }
        }

        internal IContentTypeReader GetReaderByOutput(string outputType)
        {
            IContentTypeReader res;
            return !_typeReadersOutput.TryGetValue(outputType, out res) ? null : res;
        }

        internal IContentTypeReader GetReader(string reader)
        {
            IContentTypeReader res;
            return !_typeReaders.TryGetValue(reader, out res) ? null : res;
        }

        public string RootDirectory { get; set; }

        public T Load<T>(string assetName)
        {
            object asset;
            bool containsName = false;
            if (_assets.TryGetValue(assetName, out asset))
            {
                containsName = true;
                if (asset is T)
                {
                    return (T) asset;
                }
            }
            T tmp = ReadAsset<T>(assetName);
            if (tmp != null)
            {
                if (!containsName)
                    _assets.Add(assetName, tmp);
            }
            return tmp;
        }

        public IEnumerable<string> ListContent(string path, bool recursive = false)
        {
            path = Path.Combine(RootDirectory, path);
            return Directory.GetFiles(path, "*.ego",
                recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }

        public IEnumerable<string> ListContent<T>(string path, bool recursive = false)
            //rather slow(needs to load part of files)
        {
            var tp = GetReaderByOutput(typeof(T).FullName);
            foreach (var file in ListContent(path, false))
            {
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    var res = _formatter.Deserialize(fs) as ContentFile;
                    if (res == null)
                        continue;
                    Console.WriteLine(res.FileType);
                    if (res.FileType == tp.GetType().FullName) //TODO inheritance
                        yield return file;
                }
            }
            yield break;
        }

        protected T ReadAsset<T>(string assetName)
        {
            try
            {
                using (
                    var fs = new FileStream(Path.Combine(RootDirectory, assetName + ".ego"), FileMode.Open,
                        FileAccess.Read))
                {
                    var res = _formatter.Deserialize(fs) as ContentFile;
                    if (res == null)
                        throw new Exception("Could not load non content file");
                    return (T) res.Load(this, fs, typeof(T));
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
            //return default(T);
        }
    }
}