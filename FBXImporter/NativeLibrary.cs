using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace FBXImporter
{
    [AttributeUsage(AttributeTargets.Field)]
    public class NativeMethodAttribute : Attribute
    {
        public NativeMethodAttribute(string nativeName=null,Type delegateTye=null)
        {
            this.NativeName = nativeName;
            this.DelegateType = delegateTye;
        }
        public string NativeName{get;set;}
        public Type DelegateType{get;set;}
    }
    public abstract class NativeLibrary : IDisposable
    {
        protected IntPtr handle;



        public NativeLibrary(string file)
        {
            file = System.IO.Path.GetFullPath(NativeLibraryImplementation.Instance.FindLibraryFile(file));

            Console.WriteLine("Try to open library: " + file);

            handle = NativeLibraryImplementation.Instance.LoadLibrary(file);

            Console.WriteLine("Preloading functions");
            PreloadFunctions();
        }
        protected void PreloadFunctions()
        {
            Type t = this.GetType();
            foreach(var field in t.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))
            {
                var attr = (NativeMethodAttribute)field.GetCustomAttributes(typeof(NativeMethodAttribute),true).FirstOrDefault();
                if (attr == null)
                    continue;
                string name = string.IsNullOrWhiteSpace(attr.NativeName) ? field.Name : attr.NativeName;
                Type type = attr.DelegateType == null ? field.FieldType :attr.DelegateType;
                field.SetValue(this,GetDelegate(name,type));

            }
        }
        public T GetDelegate<T>(string funcName)
        {
            return (T)(object)Marshal.GetDelegateForFunctionPointer(GetDelegate(funcName),typeof(T));
        }
        public Delegate GetDelegate(string funcName,Type delegateType)
        {
            return Marshal.GetDelegateForFunctionPointer(GetDelegate(funcName),delegateType);
        }
        public IntPtr GetDelegate(string funcName)
        {
            var ptr = NativeLibraryImplementation.Instance.GetProcAddress(handle,funcName);
            if (ptr == IntPtr.Zero)
            {
                throw new BadImageFormatException("Invalid function '"+funcName+"'");
            }
            return ptr;
        }
        #region IDisposable implementation
        public virtual void Dispose()
        {
            NativeLibraryImplementation.Instance.FreeLibrary(handle);
        }
        #endregion


    }


}

