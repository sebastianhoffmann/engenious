﻿using engenious.Graphics;
using System.Linq;

namespace engenious.Content.Serialization
{
    [ContentTypeReaderAttribute(typeof(Effect))]
    public class EffectTypeReader : ContentTypeReader<Effect>
    {
        public override Effect Read(ContentManager manager, ContentReader reader)
        {
            var effect = new Effect(manager.GraphicsDevice);

            int techniqueCount = reader.ReadInt32();
            for (int techniqueIndex = 0; techniqueIndex < techniqueCount; techniqueIndex++)
            {
                EffectTechnique technique = new EffectTechnique(reader.ReadString());
                int passCount = reader.ReadInt32();
                for (int passIndex = 0; passIndex < passCount; passIndex++)
                {
                    var pass = new EffectPass(reader.ReadString());

                    pass.BlendState = reader.Read<BlendState>(manager);
                    pass.DepthStencilState = reader.Read<DepthStencilState>(manager);
                    pass.RasterizerState = reader.Read<RasterizerState>(manager);
                    int shaderCount = reader.ReadByte();

                    for (int shaderIndex = 0; shaderIndex < shaderCount; shaderIndex++)
                    {
                        var shader = new Shader((ShaderType) reader.ReadUInt16(), reader.ReadString());
                        shader.Compile();
                        pass.AttachShader(shader);
                    }

                    int attrCount = reader.ReadByte();
                    for (int attrIndex = 0; attrIndex < attrCount; attrIndex++) //TODO: perhaps needs to be done later?
                    {
                        var usage = (VertexElementUsage) reader.ReadByte();
                        pass.BindAttribute(usage, reader.ReadString());
                    }
                    pass.Link();
                    technique.Passes.Add(pass);
                }
                effect.Techniques.Add(technique);
            }

            effect.CurrentTechnique = effect.Techniques.FirstOrDefault();
            effect.Initialize();
            return effect;
        }
    }
}