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
            //effect = new BasicEffect(GraphicsDevice);
            //vb = new VertexBuffer(GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, 3);
            //vb.SetData<VertexPositionNormalTexture>(new VertexPositionNormalTexture[]{ new VertexPositionColor(new Vector3(),Color.AliceBlue });


            texture = Content.Load<Texture2D>("brick");
            font = Content.Load<SpriteFont>("test");


            spriteBatch = new SpriteBatch(GraphicsDevice);
            //engenious.Input.Keyboard.GetState();

            Matrix projection;
            float tangent = (float)System.Math.Tan(MathHelper.PiOver4 / 2); // tangent of half fovY
            float height = 1 * tangent;         // half height of near plane
            float width = height * 1.0f;          // half width of near plane
            Matrix.CreatePerspectiveOffCenter(-width, width, -height, height, 0.1f, 1f, out projection);


            Matrix world = Matrix.CreateRotationY(0.3f) * Matrix.CreateRotationX(0.3f / 2);

            Matrix res = world * projection;
        }

        protected override void OnResize(object sender, EventArgs e)
        {
            if (GraphicsDevice.Viewport.Width != 0 && GraphicsDevice.Viewport.Height != 0)
            {
                if (target != null && !target.IsDisposed)
                    target.Dispose();
                target = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, engenious.PixelInternalFormat.Rgba);
            }
            base.OnResize(sender, e);
        }

        float rot = 0.0f;

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);


            rot += 0.001f;
            GraphicsDevice.SetRenderTarget(target);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(samplerState: SamplerState.LinearWrap);

            spriteBatch.Draw(texture, new Vector2(), null, Color.White, rot, new Vector2(0.5f * texture.Width * 0.1f, 0.5f * texture.Height * 0.1f), 0.1f, SpriteBatch.SpriteEffects.None);
            spriteBatch.End();
            spriteBatch.Begin();

            spriteBatch.Draw(texture, new Rectangle(100, 500, 8000, 8000), new Rectangle(0, 0, 100, 100), Color.White);
           
            spriteBatch.Draw(texture, new Rectangle(101, 0, 100, 100), Color.White);

            spriteBatch.DrawString(font, 0.ToString(), new engenious.Vector2(0, 0), Color.Red);
            //spriteBatch.Draw(texture, new engenious.Vector2(50, 0), null, Color.White, rot, new engenious.Vector2(), 1.0f / texture.Width * 100, SpriteBatch.SpriteEffects.FlipHorizontally);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(samplerState: SamplerState.LinearWrap);

            spriteBatch.Draw(target, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.End();
        }
    }
}

