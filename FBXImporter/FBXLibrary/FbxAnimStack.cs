using System;
using System.Collections.Generic;

namespace FBXImporter
{
    public class FbxAnimStack : FbxResource
    {
        public FbxAnimStack(FbxNode rootNode,IntPtr handle)
        {
            Handle = handle;

            Name = FBXLibrary.get_name(handle);

            Layer = new List<FbxAnimLayer>();
            int layerCount = FBXLibrary.get_anim_layer_count(handle);
            for (int i=0;i<layerCount;i++)
            {
                Layer.Add(new FbxAnimLayer(rootNode,this,FBXLibrary.get_anim_layer(handle,i)));
            }
        }
        public List<FbxAnimLayer> Layer{get;private set;}

        public string Name{get;private set;}
    }
}

