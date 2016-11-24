using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace engenious.Graphics
{
    public sealed class SpriteFont
    {
        internal Dictionary<int, int> Kernings;
        internal Dictionary<char, FontCharacter> CharacterMap;
        internal Texture2D Texture;

        internal SpriteFont(Texture2D texture)
        {
            Texture = texture;
            Kernings = new Dictionary<int, int>();
            CharacterMap = new Dictionary<char, FontCharacter>();
        }

        internal static int GetKerningKey(char first, char second)
        {
            return (int) first << 16 | (int) second;
        }

        private ReadOnlyCollection<char> _characters;

        public ReadOnlyCollection<char> Characters
            => _characters ?? (_characters = new ReadOnlyCollection<char>(CharacterMap.Keys.ToList()));

        public char? DefaultCharacter { get; set; }

        public int LineSpacing { get; set; }

        public int BaseLine { get; internal set; }

        public float Spacing { get; set; }

        public Vector2 MeasureString(StringBuilder text)
        {
            return MeasureString(text.ToString());
        }

        public Vector2 MeasureString(string text)
        {
            float width = 0.0f;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                FontCharacter fontChar;
                if (!CharacterMap.TryGetValue(c, out fontChar))
                {
                    if (!DefaultCharacter.HasValue || !CharacterMap.TryGetValue(DefaultCharacter.Value, out fontChar))
                    {
                        continue;
                    }
                }

                if (fontChar == null)
                    continue;
                width += fontChar.Advance;
                if (i < text.Length - 1)
                {
                    int kerning = 0;
                    if (Kernings.TryGetValue(GetKerningKey(c, text[i + 1]), out kerning))
                        width += kerning;
                }
            }
            return new Vector2(width, LineSpacing); //TODO height?
        }
    }
}