using UnityEngine;

namespace Assets._Source.View
{
    internal class Particle : MonoBehaviour
    {
        private Model.Particle _model;
        private Transform _transform;

        internal void Init(Model.Particle particle)
        {
            _model = particle;
            _transform = transform;
            _transform.localScale = Vector3.one * 2 * (float)_model._radius;
        }

        internal void DoUpdate()
        {
            _transform.localPosition = new Vector3((float)_model._rx, (float)_model._radius, (float)_model._ry);
        }
    }
}
