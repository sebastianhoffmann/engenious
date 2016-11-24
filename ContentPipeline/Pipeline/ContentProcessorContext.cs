using engenious.Graphics;
using OpenTK;
using OpenTK.Graphics;

namespace engenious.Content.Pipeline
{
    public class ContentProcessorContext : ContentContext
    {
        private readonly INativeWindow _window;

        public ContentProcessorContext(string workingDirectory = "")
        {
            WorkingDirectory = workingDirectory;
            _window = new NativeWindow();

            ThreadingHelper.Initialize(_window.WindowInfo, 3, 1, GraphicsContextFlags.Debug);
            GraphicsDevice = new GraphicsDevice(null, ThreadingHelper.context);
        }

        public GraphicsDevice GraphicsDevice { get; private set; }

        public string WorkingDirectory { get; private set; }

        public override void Dispose()
        {
            GraphicsDevice.Dispose();
            _window.Dispose();
        }
    }
}