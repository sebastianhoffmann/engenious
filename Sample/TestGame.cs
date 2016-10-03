using System;
using engenious.Graphics;
using engenious;
using OpenTK.Graphics.OpenGL4;

namespace Sample
{
    public class TestGame : engenious.Game
    {
        private Texture2D texture;
        private SpriteBatch spriteBatch;
        private SpriteFont font;

        private RenderTarget2D target;

        public TestGame()
        {
            texture = Content.Load<Texture2D>("brick");
            font = Content.Load<SpriteFont>("test");


            spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        protected override void OnResize(object sender, EventArgs e)
        {
            if (GraphicsDevice.Viewport.Width != 0 && GraphicsDevice.Viewport.Height != 0)
            {
                if (target != null && !target.IsDisposed)
                    target.Dispose();
                target = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, engenious.PixelInternalFormat.Rgba8);
            }
            base.OnResize(sender, e);
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //var state = engenious.Input.Mouse.GetState();
            System.Threading.Thread.Sleep(100);

            spriteBatch.Begin();
            spriteBatch.Draw(texture,new Rectangle(0,0,100,100),Color.White);
            spriteBatch.DrawString(font,"Taxi.\nTT\nTx\nTe\nTA" ,new Vector2(),Color.Black);
            spriteBatch.End();
        }
    }
}

