using System;

namespace FBXImporter
{
    public abstract class FbxResource : IDisposable
    {
        virtual internal IntPtr Handle{ get;set;}

        #region IDisposable implementation

        public virtual void Dispose(){}

        #endregion
    }
}

