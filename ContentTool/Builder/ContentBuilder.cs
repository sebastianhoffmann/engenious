using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using engenious.Content.Pipeline;

namespace ContentTool.Builder
{
    public class ContentBuilder
    {
        private const string DESTINATION_EXT = ".ego";

        public delegate void ItemProgressDelegate(object sender, ItemProgressEventArgs e);

        public delegate void BuildStatusChangedDelegate(object sender, BuildStep buildStep);

        public event BuildMessageDel BuildMessage;
        public event ItemProgressDelegate ItemProgress;
        public event BuildStatusChangedDelegate BuildStatusChanged;

        private Dictionary<string, ContentFile> builtFiles;
        private BuildStep currentBuild = BuildStep.Finished;

        private System.Threading.Thread buildingThread;
        private BuildCache cache;

        private ContentProject project;

        public ContentProject Project
        {
            get { return project; }
            private set
            {
                project = value;
                if (project == null)
                    cache = null;
                else
                    cache = BuildCache.Load(GetCacheFile());
            }
        }

        public bool IsBuilding { get; private set; }

        public bool CanClean => cache.CanClean(GetOutputDir());

        public bool CanBuild => true;

        public int FailedBuilds { get; private set; }

        public ContentBuilder(ContentProject project)
        {
            Project = project;
            builtFiles = new Dictionary<string, ContentFile>();
        }

        #region File Helper

        internal static void CreateFolderIfNeeded(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return;
            string folder = Path.GetDirectoryName(filename);
            if (string.IsNullOrEmpty(folder))
                return;
            if (!Directory.Exists(folder))
            {
                CreateFolderIfNeeded(folder);
                Directory.CreateDirectory(folder);
            }
        }

        private string GetDestinationFile(ContentFile contentFile)
        {
            return Path.Combine(Path.GetDirectoryName(contentFile.GetPath()),
                Path.GetFileNameWithoutExtension(contentFile.Name) + DESTINATION_EXT);
        }

        private string GetDestinationFileAbsolute(ContentFile contentFile)
        {
            //string importFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Project.File), contentFile.getPath());
            return Path.Combine(GetOutputDir(), GetDestinationFile(contentFile));
        }

        private string GetOutputDir()
        {
            string relOut = string.Format(Project.OutputDir.Replace("{Configuration}", "{0}"),
                Project.Configuration.ToString());
            return Path.Combine(Path.GetDirectoryName(Project.File), relOut);
        }

        private string GetObjectDir()
        {
            return Path.Combine(Path.GetDirectoryName(Project.File), "obj");
        }

        private string GetCacheFile()
        {
            return Path.Combine(GetObjectDir(), "obj", Path.GetFileNameWithoutExtension(Project.File) + ".dat");
        }

        #endregion

        private void RaiseBuildMessage(object sender, BuildMessageEventArgs e)
        {
            if (e.MessageType == BuildMessageEventArgs.BuildMessageType.Error)
                FailedBuilds++;
            BuildMessage?.Invoke(sender, e);
        }


        /// <summary>
        /// Builds a File based on the given File, ImporterContext and Processor Context
        /// </summary>
        /// <param name="contentFile"></param>
        /// <param name="importerContext"></param>
        /// <param name="processorContext"></param>
        private void BuildFile(ContentFile contentFile, ContentImporterContext importerContext,
            ContentProcessorContext processorContext)
        {
            string importDir = Path.GetDirectoryName(Project.File);
            string importFile = Path.Combine(importDir, contentFile.GetPath());
            string destFile = GetDestinationFileAbsolute(contentFile);
            string outputPath = GetOutputDir();

            CreateFolderIfNeeded(destFile);

            if (!cache.NeedsRebuild(importDir, outputPath, contentFile.GetPath()))
            {
                RaiseBuildMessage(this,
                    new BuildMessageEventArgs(contentFile.Name, "skipped!",
                        BuildMessageEventArgs.BuildMessageType.Information));
                return;
            }
            BuildInfo cacheInfo = new BuildInfo(importDir, contentFile.GetPath(), GetDestinationFile(contentFile));
            var importer = contentFile.Importer;
            if (importer == null)
                return;

            object importerOutput = importer.Import(importFile, importerContext);
            if (importerOutput == null)
                return;

            cacheInfo.Dependencies.AddRange(importerContext.Dependencies);
            cache.AddDependencies(importDir, importerContext.Dependencies);

            IContentProcessor processor = contentFile.Processor;
            if (processor == null)
                return;

            object processedData = processor.Process(importerOutput, importFile, processorContext);

            if (processedData == null)
                return;
            cacheInfo.Dependencies.AddRange(processorContext.Dependencies);
            cache.AddDependencies(importDir, processorContext.Dependencies);

            engenious.Content.Serialization.IContentTypeWriter typeWriter =
                engenious.Content.Serialization.SerializationManager.Instance.GetWriter(processedData.GetType());
            engenious.Content.ContentFile outputFileWriter =
                new engenious.Content.ContentFile(typeWriter.RuntimeReaderName);

            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(destFile, FileMode.Create, FileAccess.Write))
            {
                formatter.Serialize(fs, outputFileWriter);
                engenious.Content.Serialization.ContentWriter writer =
                    new engenious.Content.Serialization.ContentWriter(fs);
                writer.WriteObject(processedData, typeWriter);
            }
            cacheInfo.BuildDone(outputPath);
            cache.AddBuildInfo(cacheInfo);
            builtFiles[contentFile.GetPath()] = contentFile;
            ItemProgress?.BeginInvoke(this, new ItemProgressEventArgs(BuildStep.Build, contentFile), null, null);
        }

        /// <summary>
        /// Builds a Directory based on the given folder, importerContext and processorContext
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="importerContext"></param>
        /// <param name="processorContext"></param>
        private void BuildDir(ContentFolder folder, ContentImporterContext importerContext,
            ContentProcessorContext processorContext)
        {
            foreach (var item in folder.Contents)
            {
                if (item is ContentFile)
                {
                    BuildFile(item as ContentFile, importerContext, processorContext);
                }
                else if (item is ContentFolder)
                {
                    BuildDir(item as ContentFolder, importerContext, processorContext);
                }
            }
        }

        public void Build()
        {
            if (Project == null)
                return;

            currentBuild = BuildStep.Build;
            BuildStatusChanged?.BeginInvoke(this, BuildStep.Build, null, null);

            buildingThread = new System.Threading.Thread(new System.Threading.ThreadStart(BuildThread));
            buildingThread.Start();
        }

        public void Rebuild()
        {
            if (Project == null)
                return;
            var thread = new System.Threading.Thread(new System.Threading.ThreadStart(delegate()
            {
                Clean();
                buildingThread.Join();
                Build();
            }));
            thread.Start();
        }

        public void Clean()
        {
            if (Project == null)
                return;

            currentBuild = BuildStep.Clean;
            BuildStatusChanged?.BeginInvoke(this, BuildStep.Clean, null, null);

            buildingThread = new System.Threading.Thread(new System.Threading.ThreadStart(CleanThread));
            buildingThread.Start();
        }

        public void Abort()
        {
            if (!IsBuilding)
                return;
            buildingThread.Abort(); //TODO: better solution?
            IsBuilding = false;
            currentBuild = BuildStep.Abort;
            BuildStatusChanged?.Invoke(this, currentBuild | BuildStep.Abort);
        }

        public void Join()
        {
            buildingThread.Join();
        }

        #region Thread Methods

        private void CleanThread()
        {
            IsBuilding = true;
            foreach (var cachedItem in cache.Files)
            {
                var item = Project.GetElement(cachedItem.Value.InputFile) as ContentFile;
                if (item != null)
                {
                    ItemProgress?.BeginInvoke(this, new ItemProgressEventArgs(BuildStep.Clean, item), null, null);
                    if (File.Exists(GetDestinationFileAbsolute(item)))
                        File.Delete(GetDestinationFileAbsolute(item));
                }
            }
            IsBuilding = false;
            BuildStatusChanged?.Invoke(this, BuildStep.Clean | BuildStep.Finished);
        }

        private void BuildThread()
        {
            FailedBuilds = 0;

            IsBuilding = true;
            string outputDir = GetOutputDir();
            CreateFolderIfNeeded(outputDir);
            PipelineHelper.PreBuilt(Project);
            using (ContentImporterContext importerContext = new ContentImporterContext())
            using (ContentProcessorContext processorContext = new ContentProcessorContext())
            {
                importerContext.BuildMessage += RaiseBuildMessage;
                processorContext.BuildMessage += RaiseBuildMessage;

                BuildDir(Project, importerContext, processorContext);
            }
            //System.Threading.Thread.Sleep(8000);
            cache.Save(GetCacheFile());
            IsBuilding = false;

            BuildStatusChanged?.BeginInvoke(this, BuildStep.Build | BuildStep.Finished, null, null);
        }

        #endregion
    }
}