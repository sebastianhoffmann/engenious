using System;
using engenious.Graphics;

namespace FBXImporter
{
    public class FbxMesh : FbxNode
    {
        private IntPtr meshPtr;
        public FbxMesh(IntPtr handle,FbxNode parent)
            :base(handle,NodeType.eMesh,parent)
        {
            meshPtr = FBXLibrary.get_mesh(handle);

            int vertexCount = FBXLibrary.get_vertexcount(meshPtr);
            Vertices = new VertexPositionNormalTexture[vertexCount];
            if(!FBXLibrary.getVertices(handle,Vertices)){
                throw new Exception("could not load vertices");//TODO
            }
        }

        public VertexPositionNormalTexture[] Vertices{get;private set;}
    }
}

