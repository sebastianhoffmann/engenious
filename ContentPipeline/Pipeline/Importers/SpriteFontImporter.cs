﻿using System;
using engenious.Content.Pipeline;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace engenious.Pipeline
{
    [ContentImporterAttribute(".spritefont", DisplayName = "SpriteFontImporter",
         DefaultProcessor = "SpriteFontProcessor")]
    public class SpriteFontImporter : ContentImporter<SpriteFontContent>
    {
        #region implemented abstract members of ContentImporter

        public override SpriteFontContent Import(string filename, ContentImporterContext context)
        {
            return new SpriteFontContent(filename);
        }

        #endregion
    }

    public class SpriteFontContent
    {
        public SpriteFontContent(string fileName)
        {
            CharacterRegions = new List<CharacterRegion>();
            var doc = new XmlDocument();
            doc.Load(fileName);
            XmlElement rootNode = null;
            foreach (var node in doc.ChildNodes.OfType<XmlElement>())
            {
                if (node.Name == "EngeniousFont")
                {
                    rootNode = node;
                    break;
                }
            }
            if (rootNode == null)
                throw new FormatException("Not a valid Spritefont file");
            foreach (var element in rootNode.ChildNodes.OfType<XmlElement>())
            {
                switch (element.Name)
                {
                    case "FontName":
                        FontName = element.InnerText;
                        break;
                    case "Size":
                        Size = int.Parse(element.InnerText);
                        break;
                    case "Spacing":
                        Spacing = int.Parse(element.InnerText);
                        break;
                    case "UseKerning":
                        UseKerning = bool.Parse(element.InnerText);
                        break;
                    case "Style":
                        Style = parseStyle(element.InnerText);
                        break;
                    case "DefaultCharacter":
                        if (element.InnerText.Length > 1)
                            throw new FormatException("MultiChars not allowed");
                        DefaultCharacter = element.InnerText.ToCharArray().FirstOrDefault();
                        break;
                    case "CharacterRegions":
                        ParseCharacterRegion(element);
                        break;
                }
            }
        }

        private void ParseCharacterRegion(XmlElement rootNode)
        {
            foreach (var region in rootNode.ChildNodes.OfType<XmlElement>())
            {
                if (region.Name == "CharacterRegion")
                {
                    string start = null, end = null;
                    foreach (XmlElement element in region.ChildNodes.OfType<XmlElement>())
                    {
                        switch (element.Name)
                        {
                            case "Start":
                                start = element.InnerText;
                                break;
                            case "End":
                                end = element.InnerText;
                                break;
                        }
                    }
                    if (start != null && end != null)
                    {
                        CharacterRegions.Add(new CharacterRegion(start, end,
                                DefaultCharacter.HasValue ? DefaultCharacter.Value : '*'));
                            //TODO: default default character
                    }
                }
            }
        }


        private System.Drawing.FontStyle parseStyle(string styles)
        {
            var fontStyle = System.Drawing.FontStyle.Regular;
            foreach (var style in styles.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries))
            {
                switch (style)
                {
                    case "Regular":
                        fontStyle |= System.Drawing.FontStyle.Regular;
                        break;
                    case "Bold":
                        fontStyle |= System.Drawing.FontStyle.Bold;
                        break;
                    case "Italic":
                        fontStyle |= System.Drawing.FontStyle.Italic;
                        break;
                    case "Underline":
                        fontStyle |= System.Drawing.FontStyle.Underline;
                        break;
                    case "Strikeout":
                        fontStyle |= System.Drawing.FontStyle.Strikeout;
                        break;
                }
            }
            return fontStyle;
        }


        public string FontName { get; private set; }

        public int Size { get; private set; }

        public int Spacing { get; private set; }

        public bool UseKerning { get; private set; }

        public System.Drawing.FontStyle Style { get; private set; }

        public char? DefaultCharacter { get; private set; }

        public List<CharacterRegion> CharacterRegions { get; private set; }
    }

    public class CharacterRegion
    {
        private static int ParseAddress(string characterAddress)
        {
            return characterAddress.StartsWith("0x")
                ? Convert.ToInt32(characterAddress.Substring(2), 16)
                : int.Parse(characterAddress);
        }

        private static char ToChar(int characterAddress)
        {
            var value = System.Text.Encoding.Unicode.GetChars(BitConverter.GetBytes(characterAddress));

            return value[0];
        }

        private char _defaultChar;

        public CharacterRegion(string start, string end, char defaultChar)
            : this(ParseAddress(start), ParseAddress(end), defaultChar)
        {
        }

        public CharacterRegion(int start, int end, char defaultChar)
        {
            Start = start;
            End = end;
            _defaultChar = defaultChar;
        }

        public IEnumerable<char> GetChararcters()
        {
            for (int i = Start; i <= End; i++)
            {
                yield return ToChar(i);
            }
            yield break;
        }

        public int Start { get; private set; }

        public int End { get; private set; }
    }
}