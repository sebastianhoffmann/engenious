using System;
using System.Collections.Generic;

namespace FBXImporter
{
    public class FbxAnimLayer:FbxResource
    {
        public FbxAnimLayer(FbxNode rootNode,FbxAnimStack parent,IntPtr handle)
        {
            Handle = handle;
            Channels = new List<FbxChannel>();
            
            AddChannelNode(rootNode);
        }
        private void AddChannelNode(FbxNode node)
        {
            Channels.Add(new FbxChannel(node,this));
            foreach(var child in node.ChildNodes)
            {
                AddChannelNode(child);
            }
        }
        public List<FbxChannel> Channels{get;private set;}
    }
}

