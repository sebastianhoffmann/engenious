using System;
using System.Collections.Generic;

namespace FBXImporter
{
    public class FbxChannel
    {
        public FbxChannel(FbxNode node,FbxAnimLayer layer)
        {
            Node = node;
            Frames = new List<FbxKeyFrame>();
            int count = FBXLibrary.get_anim_curve_keyCount(node.Handle,layer.Handle);
            for (int i=0;i<count;i++)
            {
                var curve = FBXLibrary.get_anim_curve(node.Handle,layer.Handle,i);
                Frames.Add(new FbxKeyFrame(curve));
            }
        }
        public FbxNode Node{get;private set;}
        public List<FbxKeyFrame> Frames{get;private set;}
    }
}

