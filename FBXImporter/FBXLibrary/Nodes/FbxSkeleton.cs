using System;

namespace FBXImporter
{
    public class FbxSkeleton : FbxNode
    {
        private IntPtr skeletonPtr;
        public FbxSkeleton(IntPtr handle,FbxNode parent)
            :base(handle,NodeType.eSkeleton,parent)
        {
            skeletonPtr = FBXLibrary.get_skeleton(handle);
        }
    }
}

