using System;

namespace engenious.Content.Pipeline
{
    public abstract class ContentImporter<T> : IContentImporter
    {
        private static Type _exportType;
        public static Type ExportType => _exportType ?? (_exportType = typeof(T));

        public abstract T Import(string filename, ContentImporterContext context);

        object IContentImporter.Import(string filename, ContentImporterContext context)
        {
            return Import(filename, context);
        }
    }
}