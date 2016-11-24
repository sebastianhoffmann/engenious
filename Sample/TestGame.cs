using System;
using engenious.Graphics;
using engenious;

namespace Sample
{
    public class TestGame : Game
    {
        private readonly Texture2D _texture;
        private readonly SpriteBatch _spriteBatch;
        private readonly SpriteFont _font;

        private RenderTarget2D _target;

        public TestGame()
        {
            _texture = Content.Load<Texture2D>("brick");
            _font = Content.Load<SpriteFont>("test");


            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void OnResize(object sender, EventArgs e)
        {
            if (GraphicsDevice.Viewport.Width != 0 && GraphicsDevice.Viewport.Height != 0)
            {
                if (_target != null && !_target.IsDisposed)
                    _target.Dispose();
                _target = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width,
                    GraphicsDevice.Viewport.Height, engenious.PixelInternalFormat.Rgba8);
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

            _spriteBatch.Begin();
            _spriteBatch.Draw(_texture, new Rectangle(0, 0, 100, 100), Color.White);
            _spriteBatch.DrawString(_font, "Taxi.\nTT\nTx\nTe\nTA", new Vector2(), Color.Black);
            _spriteBatch.End();
        }
    }
}