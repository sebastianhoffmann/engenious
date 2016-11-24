using System;
using System.Linq;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Xml;

namespace ContentTool
{
    [Serializable()]
    public class ContentFile : ContentItem
    {
        public ContentFile(ContentFolder parent = null)
            : base(parent)
        {
        }

        public ContentFile(string name, ContentFolder parent = null)
            : this(parent)
        {
            Name = name;
            ProcessorName = null;
        }

        #region implemented abstract members of ContentItem

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            var props = base.GetProperties(attributes);
            var processorProps = TypeDescriptor.GetProperties(Processor?.Settings, true);
            var newProps = new PropertyDescriptor[props.Count + processorProps.Count];
            for (int i = 0; i < props.Count; i++)
                newProps[i] = props[i];
            for (int i = 0; i < processorProps.Count; i++)
            {
                newProps[i + props.Count] = new engenious.Pipeline.CustomPropertyDescriptor(processorProps[i],
                    Processor?.Settings, attributes);
            }
            return new PropertyDescriptorCollection(newProps);
        }

        #endregion

        public override string Name
        {
            get { return base.Name; }
            set
            {
                base.Name = value;
                Importer = PipelineHelper.CreateImporter(System.IO.Path.GetExtension(value));
            }
        }

        private static string GetProcessor(string name)
        {
            var tp = PipelineHelper.GetImporterType(System.IO.Path.GetExtension(name));
            if (tp != null)
            {
                foreach (
                    var attr in
                    tp.GetCustomAttributes(true).Select(x => x as engenious.Content.Pipeline.ContentImporterAttribute))
                {
                    if (attr == null)
                        continue;
                    return attr.DefaultProcessor;
                }
            }
            return "";
        }

        [Browsable(false)]
        public engenious.Content.Pipeline.ProcessorSettings Settings
        {
            get { return Processor?.Settings; }
            set
            {
                if (Processor == null)
                    return;
                Processor.Settings = value;
            }
        }

        [XmlIgnore()] private string _processorName;

        [XmlElement(IsNullable = true)]
        [Editor(typeof(Dialog.ProcessorEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string ProcessorName
        {
            get { return _processorName; }
            set
            {
                string old = _processorName;
                _processorName = value;
                if (string.IsNullOrWhiteSpace(_processorName))
                {
                    _processorName = GetProcessor(Name);
                }
                if (_processorName != old && !string.IsNullOrWhiteSpace(_processorName))
                {
                    Processor = PipelineHelper.CreateProcessor(Importer.GetType(), ProcessorName);
                }
            }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public engenious.Content.Pipeline.IContentImporter Importer { get; private set; }

        [Browsable(false)]
        [XmlIgnore()]
        public engenious.Content.Pipeline.IContentProcessor Processor { get; private set; }

        public override string ToString()
        {
            return Name;
        }

        #region implemented abstract members of ContentItem

        public override void ReadItem(XmlElement node)
        {
            switch (node.Name)
            {
                case "Name":
                    Name = node.ChildNodes.OfType<XmlText>().FirstOrDefault()?.InnerText;
                    break;
                case "Processor":
                    ProcessorName = node.ChildNodes.OfType<XmlText>().FirstOrDefault()?.InnerText;
                    break;
                case "Settings":
                    Settings?.Read(node.ChildNodes);
                    break;
            }
        }

        public override void WriteItems(XmlWriter writer)
        {
            writer.WriteElementString("Name", Name);
            writer.WriteElementString("Processor", ProcessorName);

            if (Settings != null)
            {
                writer.WriteStartElement("Settings");

                Settings.Write(writer);

                writer.WriteEndElement();
            }
        }

        #endregion
    }
}