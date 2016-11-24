using engenious.Graphics;
using engenious.Content.Pipeline;
using engenious.Content;
using System.IO;

namespace engenious.Pipeline
{
    [ContentImporterAttribute(".ego", DisplayName = "Model Importer", DefaultProcessor = "EgoModelProcessor")]
    public class EgoImporter : ContentImporter<ModelContent>
    {
        #region implemented abstract members of ContentImporter

        public override ModelContent Import(string filename, ContentImporterContext context)
        {
            var manager = new ContentManager(null, Path.GetDirectoryName(filename));
            return manager.Load<ModelContent>(Path.GetFileNameWithoutExtension(filename));
        }

        #endregion
    }
}