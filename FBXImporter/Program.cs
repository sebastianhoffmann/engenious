using System;
using System.IO;
using System.Collections.Generic;
using System.IO.Compression;

namespace FBXImporter
{


    public static class Program
    {

        public static void Main(string[] args)
        {
            FbxContext context = new FbxContext();

            FbxScene scene = new FbxScene(context,"Arni.fbx");

            context.Dispose();

            Console.WriteLine("Hello World!");
        }
    }
}

