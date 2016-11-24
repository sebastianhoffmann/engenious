using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using engenious.Graphics;

namespace engenious.Content.Pipeline
{
    [ContentImporterAttribute(".glsl", DisplayName = "Effect Importer", DefaultProcessor = "EffectProcessor")]
    public class EffectImporter : ContentImporter<EffectContent>
    {
        #region implemented abstract members of ContentImporter

        private static ShaderType ParseShaderType(string type)
        {
            switch (type)
            {
                case "PixelShader":
                case "FragmentShader":
                    return ShaderType.FragmentShader;
                case "VertexShader":
                    return ShaderType.VertexShader;
                case "GeometryShader":
                    return ShaderType.GeometryShader;
                case "TessControlShader":
                    return ShaderType.TessControlShader;
                case "TessEvaluationShader":
                    return ShaderType.TessEvaluationShader;
                case "ComputeShader":
                    return ShaderType.ComputeShader;
            }

            return (ShaderType) (-1);
        }

        private static float ParseColorPart(string value)
        {
            value = value.Trim();
            int tmp;
            if (int.TryParse(value, out tmp))
                return tmp / 255.0f;
            return float.Parse(value);
        }

        private static Color ParseColor(XmlElement el)
        {
            if (el.HasChildNodes)
            {
                float a = 1.0f, r = 0, g = 0, b = 0;
                foreach (var e in el.ChildNodes.OfType<XmlElement>())
                {
                    switch (e.Name)
                    {
                        case "A":
                            a = ParseColorPart(e.InnerText);
                            break;
                        case "R":
                            r = ParseColorPart(e.InnerText);
                            break;
                        case "G":
                            g = ParseColorPart(e.InnerText);
                            break;
                        case "B":
                            b = ParseColorPart(e.InnerText);
                            break;
                        default:
                            throw new Exception("'" + e.Name + "' is not an option for the Color element");
                    }
                }

                return new Color(r, g, b, a);
            }
            if (string.IsNullOrEmpty(el.InnerText.Trim()))
                throw new Exception("Empty value not allowed for Colors");
            try
            {
                var fI = typeof(Color).GetType().GetField(el.InnerText.Trim(), System.Reflection.BindingFlags.Static);
                return (Color) fI.GetValue(null);
            }
            catch
            {
            }
            {
                string value = el.InnerText.Trim();
                int a, r, g, b;
                if (value.Length == 4 || value.Length == 3)
                {
                    int index = 0;
                    if (value.Length == 4)
                        a = Convert.ToInt16(value[index++].ToString(), 16);
                    else
                        a = 0xF;
                    r = Convert.ToInt16(value[index++].ToString(), 16);
                    g = Convert.ToInt16(value[index++].ToString(), 16);
                    b = Convert.ToInt16(value[index].ToString(), 16);

                    a = a << 4 | a;
                    r = r << 4 | r;
                    g = g << 4 | g;
                    b = b << 4 | b;
                }
                else if (value.Length == 6 || value.Length == 8)
                {
                    int index = 0;
                    if (value.Length == 6)
                        a = Convert.ToInt16(value.Substring(index += 2, 2), 16);
                    else
                        a = 0xFF;
                    r = Convert.ToInt16(value.Substring(index += 2, 2), 16);
                    g = Convert.ToInt16(value.Substring(index += 2, 2), 16);
                    b = Convert.ToInt16(value.Substring(index, 2), 16);
                }
                else
                {
                    throw new Exception(
                        "Color must either use A/R/G/B Xml Elements or be a Hexadecimal value of Length 3/4/6/8");
                }


                return new Color(r, g, b, a);
            }
            //throw new Exception ("Unknown error while parsing color");
        }

        private static BlendState ParseBlendState(XmlElement element)
        {
            if (!element.HasChildNodes || element.Name != "BlendState")
                return null;
            var blendState = new BlendState();
            foreach (var el in element.ChildNodes.OfType<XmlElement>())
            {
                switch (el.Name)
                {
                    case "AlphaBlendFunction":
                        blendState.AlphaBlendFunction =
                            (BlendEquationMode) Enum.Parse(typeof(BlendEquationMode), el.InnerText);
                        break;
                    case "AlphaDestinationBlend":
                        blendState.AlphaDestinationBlend =
                            (BlendingFactorDest) Enum.Parse(typeof(BlendingFactorDest), el.InnerText);
                        break;
                    case "AlphaSourceBlend":
                        blendState.AlphaSourceBlend =
                            (BlendingFactorSrc) Enum.Parse(typeof(BlendingFactorSrc), el.InnerText);
                        break;
                    case "BlendFactor":
                        blendState.BlendFactor = ParseColor(el);
                        break;
                    case "ColorBlendFunction":
                        blendState.ColorBlendFunction =
                            (BlendEquationMode) Enum.Parse(typeof(BlendEquationMode), el.InnerText);
                        break;
                    case "ColorDestinationBlend":
                        blendState.ColorDestinationBlend =
                            (BlendingFactorDest) Enum.Parse(typeof(BlendingFactorDest), el.InnerText);
                        break;
                    case "ColorSourceBlend":
                        blendState.ColorSourceBlend =
                            (BlendingFactorSrc) Enum.Parse(typeof(BlendingFactorSrc), el.InnerText);
                        break;
                    default:
                        throw new Exception("'" + el.Name + "' is not an option of the BlendState");
                }
            }
            return blendState;
        }

        private static DepthStencilState ParseDepthStencilState(XmlElement element)
        {
            if (!element.HasChildNodes || element.Name != "DepthStencilState")
                return null;
            var depthStencilState = new DepthStencilState();
            foreach (var el in element.ChildNodes.OfType<XmlElement>())
            {
                /*if (el.Name == "CounterClockwiseStencilDepthBufferFail")
                    depthStencilState.CounterClockwiseStencilDepthBufferFail = (StencilOp)Enum.Parse(typeof(StencilOp), el.InnerText);
                else if (el.Name == "CounterClockwiseStencilFail")
                    depthStencilState.CounterClockwiseStencilFail = (StencilOp)Enum.Parse(typeof(StencilOp), el.InnerText);
                else if (el.Name == "CounterClockwiseStencilFunction")
                    depthStencilState.CounterClockwiseStencilFunction = (CompareFunction)Enum.Parse(typeof(CompareFunction), el.InnerText);
                else if (el.Name == "CounterClockwiseStencilPass")
                    depthStencilState.CounterClockwiseStencilPass = (StencilOp)Enum.Parse(typeof(StencilOp), el.InnerText);*/
                if (el.Name == "DepthBufferEnable")
                    depthStencilState.DepthBufferEnable = bool.Parse(el.InnerText);
                else if (el.Name == "DepthBufferFunction")
                    depthStencilState.DepthBufferFunction =
                        (DepthFunction) Enum.Parse(typeof(DepthFunction), el.InnerText);
                else if (el.Name == "DepthBufferWriteEnable")
                    depthStencilState.DepthBufferWriteEnable = bool.Parse(el.InnerText);
                else if (el.Name == "ReferenceStencil")
                    depthStencilState.ReferenceStencil = int.Parse(el.InnerText);
                else if (el.Name == "DepthBufferFunction")
                    depthStencilState.StencilDepthBufferFail = (StencilOp) Enum.Parse(typeof(StencilOp), el.InnerText);
                else if (el.Name == "ReferenceStencil")
                    depthStencilState.StencilEnable = bool.Parse(el.InnerText);
                else if (el.Name == "StencilFail")
                    depthStencilState.StencilFail = (StencilOp) Enum.Parse(typeof(StencilOp), el.InnerText);
                else if (el.Name == "StencilFunction")
                    depthStencilState.StencilFunction =
                        (StencilFunction) Enum.Parse(typeof(StencilFunction), el.InnerText);
                else if (el.Name == "DepthBufferFunction")
                    depthStencilState.StencilMask = int.Parse(el.InnerText);
                else if (el.Name == "StencilPass")
                    depthStencilState.StencilPass = (StencilOp) Enum.Parse(typeof(StencilOp), el.InnerText);
                /*else if (el.Name == "DepthBufferFunction")
                    depthStencilState.StencilWriteMask = int.Parse(el.InnerText);
                else if (el.Name == "TwoSidedStencilMode")
                    depthStencilState.TwoSidedStencilMode = bool.Parse(el.InnerText);*/
                else
                    throw new Exception("'" + el.Name + "' is not an option of the DepthStencilState");
            }
            return depthStencilState;
        }

        private static RasterizerState ParseRasterizerState(XmlElement element)
        {
            if (!element.HasChildNodes || element.Name != "RasterizerState")
                return null;
            var rasterizerState = new RasterizerState();
            foreach (var el in element.ChildNodes.OfType<XmlElement>())
            {
                switch (el.Name)
                {
                    case "CullMode":
                        rasterizerState.CullMode = (CullMode) Enum.Parse(typeof(CullMode), el.InnerText);
                        break;
                    case "FillMode":
                        rasterizerState.FillMode = (PolygonMode) Enum.Parse(typeof(PolygonMode), el.InnerText);
                        break;
                    case "MultiSampleAntiAlias":
                        rasterizerState.MultiSampleAntiAlias = bool.Parse(el.InnerText);
                        break;
                    case "ScissorTestEnable":
                        rasterizerState.ScissorTestEnable = bool.Parse(el.InnerText);
                        break;
                    default:
                        throw new Exception("'" + el.Name + "' is not an option of the RasterizerState");
                }
            }
            return rasterizerState;
        }

        public override EffectContent Import(string filename, ContentImporterContext context)
        {
            try
            {
                var content = new EffectContent();

                var doc = new XmlDocument();
                doc.Load(filename);
                XmlElement effectElement;
                var current = doc.FirstChild;
                while (current != null && current.NodeType != XmlNodeType.Element)
                {
                    current = current.NextSibling;
                }
                effectElement = (XmlElement) current;
                foreach (var technique in effectElement.ChildNodes.OfType<XmlElement>())
                {
                    var info = new EffectTechnique();
                    info.Name = technique.GetAttribute("name");
                    foreach (var pass in technique.ChildNodes.OfType<XmlElement>())
                    {
                        var pi = new EffectPass();
                        pi.Name = pass.GetAttribute("name");
                        foreach (var sh in pass.ChildNodes.OfType<XmlElement>())
                        {
                            switch (sh.Name)
                            {
                                case "Shader":
                                    ShaderType type = ParseShaderType(sh.GetAttribute("type"));
                                    if ((int) type == -1)
                                        throw new Exception("Unsupported Shader type detected");
                                    string shaderFile = System.IO.Path.Combine(
                                        System.IO.Path.GetDirectoryName(filename), sh.GetAttribute("filename"));
                                    pi.Shaders.Add(type, shaderFile);
                                    context.Dependencies.Add(shaderFile);
                                    break;
                                case "BlendState":
                                    pi.BlendState = ParseBlendState(sh);
                                    break;
                                case "DepthStencilState":
                                    pi.DepthStencilState = ParseDepthStencilState(sh);
                                    break;
                                case "RasterizerState":
                                    pi.RasterizerState = ParseRasterizerState(sh);
                                    break;
                                case "Attributes":
                                    foreach (var attr in sh.ChildNodes.OfType<XmlElement>())
                                    {
                                        if (attr.Name != "attribute") continue;
                                        var usage =
                                            (VertexElementUsage) Enum.Parse(typeof(VertexElementUsage), attr.InnerText);
                                        string nm = attr.GetAttribute("name");
                                        if (nm.Length < 1)
                                            throw new Exception("Not a valid attribute name'" + nm + "'");
                                        pi.Attributes.Add(usage, nm);
                                    }
                                    break;
                                default:
                                    throw new Exception("'" + sh.Name + "' element not recognized");
                            }
                        }
                        info.Passes.Add(pi);
                    }
                    content.Techniques.Add(info);
                }

                return content;
            }
            catch (Exception ex)
            {
                context.RaiseBuildMessage(filename, ex.Message, BuildMessageEventArgs.BuildMessageType.Error);
            }
            return null;
        }

        #endregion
    }

    public class EffectContent
    {
        public EffectContent()
        {
            Techniques = new List<EffectTechnique>();
        }

        public List<EffectTechnique> Techniques { get; private set; }
    }

    public class EffectTechnique
    {
        public EffectTechnique()
        {
            Passes = new List<EffectPass>();
        }

        public string Name { get; internal set; }

        public List<EffectPass> Passes { get; private set; }
    }

    public class EffectPass
    {
        public EffectPass()
        {
            Shaders = new Dictionary<ShaderType, string>();
            Attributes = new Dictionary<VertexElementUsage, string>();
        }

        public string Name { get; internal set; }

        public BlendState BlendState { get; internal set; }

        public DepthStencilState DepthStencilState { get; internal set; }

        public RasterizerState RasterizerState { get; internal set; }

        public Dictionary<ShaderType, string> Shaders { get; private set; }

        public Dictionary<VertexElementUsage, string> Attributes { get; private set; }
    }
}