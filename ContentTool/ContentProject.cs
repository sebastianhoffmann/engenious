﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace ContentTool
{
    public enum Configuration
    {
        Debug,
        Release
    }

    [Serializable()]
    [System.Xml.Serialization.XmlRoot("Content")]
    public class ContentProject : ContentFolder
    {
        public ContentProject()
            : this("Content")
        {
        }

        public ContentProject(string file)
            : base(Path.GetFileNameWithoutExtension(file))
        {
            File = file;
            OutputDir = "bin/{Configuration}/";
            References = null;
        }


        private string _name;

        [System.ComponentModel.DefaultValue("Content")]
        public override string Name
        {
            get { return _name; }
            set { _name = Path.GetFileNameWithoutExtension(value); }
        }

        [System.ComponentModel.DefaultValue(Configuration.Debug)]
        public Configuration Configuration { get; set; }

        [System.ComponentModel.DefaultValue("bin/{Configuration}/")]
        public string OutputDir { get; set; }

        [System.Xml.Serialization.XmlIgnore()]
        [System.ComponentModel.Browsable(false)]
        public string File { get; private set; }

        [System.ComponentModel.Editor(typeof(Dialog.ReferenceCollectionEditor),
             typeof(System.Drawing.Design.UITypeEditor))]
        public List<string> References { get; set; }


        private static void SearchParents(ContentFolder folder)
        {
            if (folder == null)
                return;
            foreach (var item in folder.Contents)
            {
                item.Parent = folder;
                SearchParents(item as ContentFolder);
            }
        }

        public static ContentProject Load(string filename)
        {
            var document = new XmlDocument();
            document.Load(filename);

            var root = document.ChildNodes.OfType<XmlElement>().FirstOrDefault(x => x.Name == "Content");
            if (root == null)
                return null;
            var project = new ContentProject(filename);
            foreach (var child in root.ChildNodes.OfType<XmlElement>())
            {
                switch (child.Name)
                {
                    case "References":
                    {
                        project.References = new List<string>();
                        foreach (var reference in child.ChildNodes.OfType<XmlElement>())
                        {
                            if (reference.Name == "Reference")
                            {
                                var val = reference.ChildNodes.OfType<XmlText>().FirstOrDefault()?.InnerText;
                                if (val != null)
                                    project.References.Add(val);
                            }
                        }
                    }
                        break;
                    case "Configuration":
                    {
                        var val = child.ChildNodes.OfType<XmlText>().FirstOrDefault()?.InnerText;
                        Configuration config;
                        if (val != null && Enum.TryParse<Configuration>(val, out config))
                            project.Configuration = config;
                    }
                        break;
                    case "OutputDir":
                    {
                        var val = child.ChildNodes.OfType<XmlText>().FirstOrDefault()?.InnerText;
                        if (val != null)
                            project.OutputDir = val;
                    }
                        break;
                    default:
                        project.ReadItem(child);
                        break;
                }
            }
            SearchParents(project);
            return project;
            /* using (var reader = new XmlTextReader(filename, System.Text.Encoding.UTF8))
             {
 
                 ContentProject proj = (ContentProject)serializer.Deserialize(fs.BaseStream);
                 proj.File = filename;
                 SearchParents(proj);
                 return proj;
             }*/
        }

        public static void Save(string filename, ContentProject project)
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            settings.NewLineOnAttributes = false;
            settings.OmitXmlDeclaration = true;
            settings.Encoding = System.Text.Encoding.UTF8;
            //System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(ContentProject));
            using (var writer = XmlWriter.Create(filename, settings))
            {
                Save(writer, project);
            }
        }

        public static void Save(XmlWriter writer, ContentProject project)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("Content");
            {
                writer.WriteStartElement("References");
                if (project.References != null)
                {
                    foreach (var reference in project.References)
                        writer.WriteElementString("Reference", reference);
                }
                writer.WriteEndElement();

                writer.WriteElementString("Configuration", project.Configuration.ToString());
                writer.WriteElementString("OutputDir", project.OutputDir);

                project.WriteItems(writer);
            }
            writer.WriteEndElement();
        }
    }
}