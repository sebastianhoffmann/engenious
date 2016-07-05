using System;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;

namespace FBXImporter
{
    public abstract class NativeLibraryImplementation : IDisposable
    {
        private static NativeLibraryImplementation instance;
        private static bool IsLinux()
        {
            int platform = (int) Environment.OSVersion.Platform;
            return (platform == 4) || (platform == 6) || (platform == 128);
        }
        public static NativeLibraryImplementation Instance
        {
            get{
                if (instance == null)
                {
                    if (IsLinux())
                        instance = new NativeLibraryLinuxImplementation();
                    else
                        instance = new NativeLibraryWindowsImplementation();
                }
                return instance;
            }
        }

        public string DefaultLibraryPath32Bit
        {
            get;
            private set;
        }

        public string DefaultLibraryPath64Bit
        {
            get;
            private set;
        }

        #region IDisposable implementation


        public abstract void Dispose();


        #endregion

        public abstract IntPtr LoadLibrary(string file);
        public abstract void FreeLibrary(IntPtr handle);
        public abstract IntPtr GetProcAddress(IntPtr handle,string funcName);

        public abstract string FindLibraryFile(string name);
    }
    public class NativeLibraryLinuxImplementation : NativeLibraryImplementation
    {
        internal enum DLOpenFlags : Int32
        {
            Lazy = 0x0001,
            Now = 0x0002,
            Global = 0x0100,
            Local = 0x0000,
        }
        const string lib = "dl";

        [DllImport("libdl.so", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        internal static extern IntPtr dlopen(string filename, DLOpenFlags flags);

        [DllImport("libdl.so", CallingConvention = CallingConvention.Cdecl,CharSet = CharSet.Auto)]
        internal static extern int dlclose(IntPtr handle);
        [DllImport("libdl.so", CallingConvention = CallingConvention.Cdecl,CharSet = CharSet.Auto)]
        internal static extern IntPtr dlerror();
        [DllImport("libdl.so", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        internal static extern IntPtr dlsym(IntPtr handle, IntPtr name);
        private const int RTLD_NOW = 2;
        private const int RTLD_LAZY = 1;
        #region implemented abstract members of NativeLibraryImplementation
        private IntPtr libC;
        private void LoadLibC()
        {
            libC = dlopen("libstdc++.so.6",DLOpenFlags.Global|DLOpenFlags.Lazy);
            if(libC == IntPtr.Zero)
            {
                IntPtr errPtr = dlerror();
                string msg = Marshal.PtrToStringAnsi(errPtr);
                if(string.IsNullOrEmpty(msg))
                    throw new BadImageFormatException();
                else
                    throw new BadImageFormatException(msg);
            }
        }

        #region implemented abstract members of NativeLibraryImplementation


        public override void Dispose()
        {
            FreeLibrary(libC);
        }


        #endregion

        public override IntPtr LoadLibrary(string file)
        {
            //LoadLibC();
            var handle = dlopen(file,DLOpenFlags.Lazy);
            if(handle == IntPtr.Zero)
            {
                IntPtr errPtr = dlerror();
                string msg = Marshal.PtrToStringAnsi(errPtr);
                if(string.IsNullOrEmpty(msg))
                    throw new BadImageFormatException();
                else
                    throw new BadImageFormatException(msg);
            }
            return handle;
        }
        public override void FreeLibrary(IntPtr handle)
        {
            dlclose(handle);
        }
        public override IntPtr GetProcAddress(IntPtr handle, string funcName)
        {
            IntPtr ptr = Marshal.StringToHGlobalAnsi(funcName);
            var ret= dlsym(handle,ptr);

            Marshal.FreeHGlobal(ptr);
            return ret;
        }
        public override string FindLibraryFile(string name)
        {
            return "lib" + name + ".so";
        }

        #endregion
    }
    public class NativeLibraryMacImplementation : NativeLibraryImplementation
    {
        [DllImport("libdl.dylib")]
        private static extern IntPtr dlopen(String fileName, int flags);

        [DllImport("libdl.dylib")]
        private static extern IntPtr dlsym(IntPtr handle, String funcName);

        [DllImport("libdl.dylib")]
        private static extern int dlclose(IntPtr handle);

        [DllImport("libdl.dylib")]
        private static extern IntPtr dlerror();

        private const int RTLD_NOW = 2;
        private const int RTLD_LAZY = 1;
        #region implemented abstract members of NativeLibraryImplementation
        public override IntPtr LoadLibrary(string file)
        {
            var handle = dlopen(file,RTLD_LAZY);
            if(handle == IntPtr.Zero)
            {
                IntPtr errPtr = dlerror();
                string msg = Marshal.PtrToStringAnsi(errPtr);
                if(string.IsNullOrEmpty(msg))
                    throw new BadImageFormatException();
                else
                    throw new BadImageFormatException(msg);
            }
            return handle;
        }
        #region implemented abstract members of NativeLibraryImplementation

        public override void Dispose()
        {
        }

        #endregion
        public override void FreeLibrary(IntPtr handle)
        {
            dlclose(handle);
        }
        public override IntPtr GetProcAddress(IntPtr handle, string funcName)
        {
            return dlsym(handle,funcName);
        }
        public override string FindLibraryFile(string name)
        {
            return "lib" + name + ".dylib";
        }

        #endregion
    }
    public class NativeLibraryWindowsImplementation : NativeLibraryImplementation
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, BestFitMapping = false, SetLastError = true,EntryPoint="LoadLibrary")]
        private static extern IntPtr NativeLoadLibrary(String fileName);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport("kernel32.dll", SetLastError = true,EntryPoint="LoadLibrary")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool NativeFreeLibrary(IntPtr handle);

        [DllImport("kernel32.dll",EntryPoint="GetProcAddress")]
        private static extern IntPtr NativeGetProcAddress(IntPtr handle, String funcName);

        #region implemented abstract members of NativeLibraryImplementation
        public override IntPtr LoadLibrary(string file)
        {
            var handle = NativeLoadLibrary(file);
            if (handle == IntPtr.Zero){
                int hr = Marshal.GetHRForLastWin32Error();
                Exception innerException = Marshal.GetExceptionForHR(hr);
                if(innerException == null)
                    throw new BadImageFormatException();
                else
                    throw innerException;
            }
            return handle;
        }
        #region implemented abstract members of NativeLibraryImplementation

        public override void Dispose()
        {
        }

        #endregion
        public override void FreeLibrary(IntPtr handle)
        {
            NativeFreeLibrary(handle);
        }
        public override IntPtr GetProcAddress(IntPtr handle, string funcName)
        {
            return NativeGetProcAddress(handle,funcName);
        }

        public override string FindLibraryFile(string name)
        {
            return name + ".dll";
        }

        #endregion
    }
}

