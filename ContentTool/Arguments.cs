﻿using System;

namespace ContentTool
{
    public class Arguments
    {
        public string OutputDirectory { get; private set; }
        public string ContentProject { get; private set; }
        public string ProjectDir { get; private set; }
        public bool Hidden { get; private set; }
        public Configuration? Configuration { get; set; }

        public Arguments()
        {
            Configuration = null;
            OutputDirectory = null;
            ProjectDir = Environment.CurrentDirectory;
        }

        private static string ParsePath(string dir)
        {
            if (dir.Length < 2) return null;
            if (dir[0] == '"')
                dir = dir.Substring(1);
            if (dir[dir.Length - 1] == '"')
                dir = dir.Substring(0, dir.Length - 1);
            return dir;
        }

        public void ParseArguments(string[] args)
        {
            foreach (var arg in args)
            {
                if (arg.StartsWith("/hidden:"))
                {
                    bool hidden;
                    if (bool.TryParse(arg.Substring("/hidden:".Length), out hidden))
                        Hidden = hidden;
                }
                else if (arg.StartsWith("/outputDir:"))
                {
                    string dir = arg.Substring("/outputDir:".Length).Trim();
                    OutputDirectory = ParsePath(dir);
                }
                else if (arg.StartsWith("/@:"))
                {
                    string dir = arg.Substring("/@:".Length).Trim();
                    ContentProject = ParsePath(dir);
                }
                else if (arg.StartsWith("/projectDir:"))
                {
                    string dir = arg.Substring("/projectDir:".Length).Trim();
                    ProjectDir = ParsePath(dir);
                }
                else if (arg.StartsWith("/configuration:"))
                {
                    Configuration configuration;
                    if (Enum.TryParse<Configuration>(arg.Substring("/configuration:".Length), out configuration))
                        Configuration = configuration;
                }
            }
        }
    }
}