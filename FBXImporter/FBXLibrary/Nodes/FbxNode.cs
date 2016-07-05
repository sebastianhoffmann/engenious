using System;
using System.Collections.Generic;
using engenious;

namespace FBXImporter
{
    public class FbxNode : FbxResource
    {
        internal static FbxNode CreateNode(IntPtr handle,FbxNode parent=null)
        {
            var nodeType = (NodeType)FBXLibrary.get_attribute_type(handle);
            switch(nodeType)
            {
                case NodeType.eMesh:
                    return new FbxMesh(handle,parent);
                case NodeType.eSkeleton:
                    return new FbxSkeleton(handle,parent);
                default:
                    return new FbxNode(handle,nodeType,parent);
            }
        }
        internal FbxNode(IntPtr handle,NodeType nodeType,FbxNode parent=null)
            :this(parent)
        {
            Handle = handle;
            NodeType=nodeType;
            Name = FBXLibrary.get_name(handle);

            Transform = FBXLibrary.get_node_transform(handle);

            int childCount=FBXLibrary.get_child_count(handle);
            for (int i=0;i<childCount;i++)
            {
                IntPtr childHandle=FBXLibrary.get_child(handle,i);
                ChildNodes.Add(CreateNode(childHandle,this));
            }

            //if (parent != null)
            //    Transform = Matrix.Invert(parent.Transform)* Transform;

        }
        public FbxNode(FbxNode parent=null)
        {
            Parent = parent;
            ChildNodes = new List<FbxNode>();
        }
        public string Name{get;private set;}
        public NodeType NodeType{get;private set;}
        public List<FbxNode> ChildNodes{get;private set;}
        public FbxNode Parent{get;private set;}

        public bool IsRoot{get{return Parent==null;}}

        public Matrix Transform{get;private set;}
    }
}

