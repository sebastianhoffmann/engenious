namespace engenious.Graphics
{
    public class DirectionalLight
    {
        internal EffectParameter DirectionParameter, DiffuseColorParameter, SpecularColorParameter;

        public DirectionalLight(DirectionalLight clone)
            : this(clone.DirectionParameter, clone.DiffuseColorParameter, clone.SpecularColorParameter)
        {
            DiffuseColor = clone.DiffuseColor;
            Direction = clone.Direction;
            SpecularColor = clone.SpecularColor;
            Enabled = clone.Enabled;
        }

        public DirectionalLight(EffectParameter directionParameter, EffectParameter diffuseColorParameter,
            EffectParameter specularColorParameter)
        {
            this.DirectionParameter = directionParameter;
            this.DirectionParameter = diffuseColorParameter;
            this.SpecularColorParameter = specularColorParameter;
        }

        Vector3 DiffuseColor { get; set; }
        Vector3 Direction { get; set; }
        Vector3 SpecularColor { get; set; }
        bool Enabled { get; set; }
    }
}