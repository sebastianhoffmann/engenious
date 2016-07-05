using System;
using engenious;
using System.Collections.Generic;

namespace FBXImporter
{
    public class FbxScene : FbxResource
    {
        public FbxScene(FbxContext context,string filename,string password=null)
        {
            Handle=FBXLibrary.load_model(context.Handle,filename,password);

            RootNode = FbxNode.CreateNode(FBXLibrary.get_root_node(Handle),null);

            AnimStack = new List<FbxAnimStack>();
            int animStackCount = FBXLibrary.get_anim_stack_count(Handle);
            for (int i=0;i<animStackCount;i++)
            {
                AnimStack.Add(new FbxAnimStack(RootNode,FBXLibrary.get_anim_stack(Handle,i)));
            }

        }

        public FbxNode RootNode{get;private set;}

        public List<FbxAnimStack> AnimStack{get;private set;}

        #region implemented abstract members of FbxResource

        public override void Dispose()
        {
            //TODO: no deletion needed?
        }

        #endregion
    }
}

