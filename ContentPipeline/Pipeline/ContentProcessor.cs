using System;

namespace engenious.Content.Pipeline
{
    public abstract class ContentProcessor<TInput, TOutput, TSettings> : IContentProcessor
        where TSettings : ProcessorSettings, new()
    {
        protected ContentProcessor()
        {
            _settings = new TSettings();
        }

        private static Type _importType;
        public static Type ImportType => _importType ?? (_importType = typeof(TInput));
        private static Type _exportType;
        public static Type ExportType => _exportType ?? (_exportType = typeof(TOutput));

        /*public TSettings Settings
        {
            get;set;
        }*/
        [System.Xml.Serialization.XmlIgnore()] protected TSettings _settings;

        public ProcessorSettings Settings
        {
            get { return _settings; }
            set { _settings = (TSettings) value; }
        }

        public abstract TOutput Process(TInput input, string filename, ContentProcessorContext context);

        object IContentProcessor.Process(object input, string filename, ContentProcessorContext context)
        {
            return Process((TInput) input, filename, context);
        }
    }

    public abstract class ContentProcessor<TInput, TOutput> : ContentProcessor<TInput, TOutput, ProcessorSettings>
    {
        protected ContentProcessor()
        {
            Settings = null;
        }
    }
}