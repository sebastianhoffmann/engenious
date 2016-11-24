using System;
using System.ComponentModel;
using System.Xml;

namespace ContentTool
{
    [System.Xml.Serialization.XmlInclude(typeof(ContentFolder))]
    [System.Xml.Serialization.XmlInclude(typeof(ContentFile))]
    public abstract class ContentItem : INotifyPropertyChanged, System.Collections.Specialized.INotifyCollectionChanged,
        ICustomTypeDescriptor
    {
        #region ICustomTypeDescriptor implementation

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return GetProperties(null); //return TypeDescriptor.GetProperties(this, true);
        }

        public virtual PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(this, attributes, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion

        protected ContentItem(ContentFolder parent = null)
        {
            Parent = parent;
        }

        private string _name;

        [DisplayName("(Name)")]
        public virtual string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                }
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false)]
        public virtual ContentFolder Parent { get; internal set; }

        public string GetPath()
        {
            if (this is ContentProject || Parent == null)
                return "";
            if (Parent is ContentProject)
                return Name;
            return System.IO.Path.Combine(Parent.GetPath(), Name);
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region INotifyCollectionChanged implementation

        public event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        protected void InvokePropertyChange(object sender, PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(sender, args);
        }

        protected void InvokeCollectionChange(object sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(sender, args);
        }

        public abstract void ReadItem(XmlElement node);
        public abstract void WriteItems(XmlWriter writer);
    }
}