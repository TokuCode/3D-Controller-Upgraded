using UnityEngine;

namespace Movement3D.Gameplay
{
    public class Crouch : Feature
    {
        private PhysicsCheck physics;

        [Header("Runtime")]
        [SerializeField] private bool _isCrouching;
        public bool IsCrouching => _isCrouching;
        private float _startYScale;
        private float _startHeight;

        [Header("Parameters")]
        [SerializeField] private float _crouchHeightMultiplier;

        public override void InitializeFeature(Controller controller)
        {
            base.InitializeFeature(controller);

            _dependencies.TryGetFeature(out physics);
            
            _startYScale = _invoker.LocalScale.Get().y;
            _startHeight = _invoker.Height.Get();
        }

        public override void Apply(ref InputPayload @event)
        {
            if(@event.Context != UpdateContext.Update) return;
            
            CheckCrouching(@event.Crouch);
        }

        private void CheckCrouching(bool crouchInput)
        {
            if(crouchInput && !_isCrouching)
                CrouchAction();
            else if(!crouchInput && _isCrouching && !physics.BlockedHead)
                UncrouchAction();
        }

        private void CrouchAction()
        {
            Vector3 localScale = _invoker.LocalScale.Get();
            
            _invoker.LocalScale.Execute(new (localScale.x, _startYScale * _crouchHeightMultiplier, localScale.z));
            _invoker.Height.Execute(_startHeight * _crouchHeightMultiplier);

            _isCrouching = true;
        }

        private void UncrouchAction()
        {
            Vector3 localScale = _invoker.LocalScale.Get();
            
            _invoker.LocalScale.Execute(new (localScale.x, _startYScale, localScale.z));
            _invoker.Height.Execute(_startHeight);

            _isCrouching = false;
        }
    }
}