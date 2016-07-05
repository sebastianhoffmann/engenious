using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace FBXImporter
{
    public class FbxContext : FbxResource
    {
        private static FBXLibrary fbxLibrary;
        private static object referenceLock=new object();
        private static int references=0;
       
        public FbxContext()
        {
            lock(referenceLock){
                if (references <= 0)
                {
                    references = 0;
                    fbxLibrary = new FBXLibrary();
                }
                references++;
            }
            Handle = FBXLibrary.create_fbxcontext();
        }

        #region IDisposable implementation

        public override void Dispose()
        {
            FBXLibrary.delete_fbxcontext(Handle);

            lock(referenceLock){
                references--;
                if (references <= 0 && fbxLibrary != null)
                {
                    fbxLibrary.Dispose();
                    fbxLibrary = null;
                }
            }
        }

        #endregion
    }
}

