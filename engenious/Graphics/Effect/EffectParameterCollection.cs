using System.Collections.Generic;
using System.Collections;


namespace engenious.Graphics
{
    public sealed class EffectParameterCollection : IEnumerable<EffectParameter>
    {
        private Dictionary<string, EffectParameter> _parameters;
        private List<EffectParameter> _parameterList;

        public EffectParameterCollection(EffectTechniqueCollection techniques)
        {
            ThreadingHelper.BlockOnUIThread(() =>
            {
                _parameters = new Dictionary<string, EffectParameter>();
                _parameterList = new List<EffectParameter>();

                foreach (EffectTechnique technique in techniques)
                {
                    foreach (EffectPass pass in technique.Passes)
                    {
                        pass.CacheParameters();

                        foreach (EffectPassParameter param in pass.Parameters)
                        {
                            EffectParameter current = null;
                            if (!_parameters.TryGetValue(param.Name, out current))
                            {
                                current = new EffectParameter(param.Name);
                                Add(current);
                            }
                            current.Add(param);
                        }
                    }
                }
            });
        }

        internal void Add(EffectParameter parameter)
        {
            _parameterList.Add(parameter);
            _parameters.Add(parameter.Name, parameter);
        }

        public EffectParameter this[int index] => _parameterList[index];

        public EffectParameter this[string name] => _parameters[name];

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _parameterList.GetEnumerator();
        }

        public IEnumerator<EffectParameter> GetEnumerator()
        {
            return _parameterList.GetEnumerator();
        }
    }
}