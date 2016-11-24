﻿using System.Collections.Generic;

namespace engenious.Graphics
{
    public class ModelContent
    {
        public ModelContent()
        {
            Animations = new List<AnimationContent>();
        }

        public MeshContent[] Meshes { get; set; }

        internal NodeContent RootNode { get; set; }

        internal List<NodeContent> Nodes { get; set; }

        internal List<AnimationContent> Animations { get; set; }
    }
}