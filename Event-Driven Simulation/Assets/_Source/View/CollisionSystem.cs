using UnityEngine;

namespace Assets._Source.View
{
    internal class CollisionSystem : MonoBehaviour
    {
        [SerializeField] private Particle _particlePrefab;
        [SerializeField] private int _particlesToCreate = 100;
        [SerializeField] private double _hz = 0.5;

        private Particle[] _particleViews;
        private Model.CollisionSystem _model;

        private void Awake()
        {
            var particleModels = new Model.Particle[_particlesToCreate];
            _particleViews = new Particle[_particlesToCreate];
            Transform myTransform = transform;
            var random = new System.Random();
            for (int i = 0; i < _particlesToCreate; i++)
            {
                var particleModel = new Model.Particle(random);
                var particleView = Instantiate(_particlePrefab, myTransform);
                particleView.Init(particleModel);

                particleModels[i] = particleModel;
                _particleViews[i] = particleView;
            }


            _model = new Model.CollisionSystem(particleModels);
        }

        private void FixedUpdate()
        {
            _model.Simulate(10000, _hz);
            for (int i = 0; i < _particleViews.Length; ++i)
            {
                _particleViews[i].DoUpdate();
            }
        }
    }
}
