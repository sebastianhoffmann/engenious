using System.Collections.Generic;
using System.Collections;

namespace engenious.Graphics
{
    public sealed class EffectPassParameterCollection : IEnumerable<EffectPassParameter>
    {
        private readonly List<EffectPassParameter> _parameterList;
        private readonly Dictionary<string, EffectPassParameter> _parameters;
        private readonly EffectPass _pass;

        internal EffectPassParameterCollection(EffectPass pass)
        {
            _pass = pass;
            _parameterList = new List<EffectPassParameter>();
            _parameters = new Dictionary<string, EffectPassParameter>();
        }

        internal void Add(EffectPassParameter parameter)
        {
            _parameterList.Add(parameter);
            _parameters.Add(parameter.Name, parameter);
        }

        private EffectPassParameter CacheParameter(string name)
        {
            EffectPassParameter param = null;
            ThreadingHelper.BlockOnUIThread(() =>
            {
                int location = _pass.GetUniformLocation(name);
                param = new EffectPassParameter(_pass, name, location);
            });
            return param;
        }

        public EffectPassParameter this[int index] => _parameterList[index];

        public EffectPassParameter this[string name]
        {
            get
            {
                EffectPassParameter val;
                if (!_parameters.TryGetValue(name, out val))
                {
                    val = CacheParameter(name);
                    _parameters.Add(name, val);
                }
                return val;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _parameterList.GetEnumerator();
        }

        public IEnumerator<EffectPassParameter> GetEnumerator()
        {
            return _parameterList.GetEnumerator();
        }
    }
}