﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace ContentTool
{
    [System.Serializable]
    [System.Xml.Serialization.XmlRoot("Configuration")]
    public class ToolConfiguration
    {
        [System.NonSerialized]
        public static string ConfigurationLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".engenious");

        public string LastFile { get; set; }

        public void Save()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ToolConfiguration));
                using (StreamWriter w = new StreamWriter(ConfigurationLocation, false))
                {
                    serializer.Serialize(w, this);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An Error occured while saving the configuration", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public static ToolConfiguration Load()
        {
            if (!File.Exists(ConfigurationLocation))
            {
                ToolConfiguration conf = new ToolConfiguration();
                conf.Save();
            }

            ToolConfiguration config;

            XmlSerializer serializer = new XmlSerializer(typeof(ToolConfiguration));
            using (StreamReader r = new StreamReader(ConfigurationLocation))
            {
                config = (ToolConfiguration)serializer.Deserialize(r);
            }

            return config;
        }
    }
}
