﻿using System;

namespace Sample
{
    internal class MainClass
    {
        [STAThread()]
        public static void Main(string[] args)
        {
            using (var test = new TestGame())
                test.Run();
        }
    }
}