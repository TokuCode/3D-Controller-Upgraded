using System.Collections;
using UnityEngine;

namespace Movement3D.Gameplay
{
    public class Run : Feature
    {
        public enum State
        {
            Walking,
            Crouching,
            Running
        }
        
        private Crouch crouch;
        private PhysicsCheck physics;

        [Header("Runtime")]
        [SerializeField] private float _currentMaxSpeed;
        public float MaxSpeed => _currentMaxSpeed;
        [SerializeField] private float _currentAcceleration;
        public float Acceleration => _currentAcceleration;
        [SerializeField] private float _desiredMaxSpeed;
        [SerializeField] private float _lastDesiredMaxSpeed;
        [SerializeField] private State _state;
        public State MoveState => _state;
        private Coroutine _speedControl;
        [SerializeField] private bool _isRunning;
        public bool IsRunning => _isRunning;

        [Header("Walking")]
        [SerializeField] private float _walkMaxSpeed;
        [SerializeField] private float _walkAcceleration;

        [Header("Running")]
        [SerializeField] private float _runMaxSpeed;
        [SerializeField] private float _runAcceleration;

        [Header("Crouching")]
        [SerializeField] private float _crouchMaxSpeed;
        [SerializeField] private float _crouchAcceleration;

        [Header("Additional Configurations")]
        [SerializeField] private float _timeSmoothing;
        [SerializeField] private float _smoothingThreshold;
        [SerializeField] private float _coyoteRunTime;

        public override void InitializeFeature(Controller controller)
        {
            base.InitializeFeature(controller);
            
            _dependencies.TryGetFeature(out crouch);
            _dependencies.TryGetFeature(out physics);
            
            _state = State.Walking;
            _currentMaxSpeed = _walkMaxSpeed;
            _currentAcceleration = _walkAcceleration;
        }

        public override void Apply(ref InputPayload @event)
        {
            if(@event.Context != UpdateContext.Update) return;
            
            TryRun(@event.Run);
        }

        private void TryRun(bool runInput)
        {
            if(runInput && !crouch.IsCrouching && Time.time - physics.LastGroundTime <= _coyoteRunTime) _isRunning = true;
            else if(!runInput) _isRunning = false;
        }

        public override void UpdateFeature()
        {
            SetDesiredMaxSpeedAndAcceleration();
        }

        private void SetDesiredMaxSpeedAndAcceleration()
        {
            float acceleration;
            bool enableTransition = false;

            if(crouch.IsCrouching)
            {
                _state = State.Crouching;
                
                _desiredMaxSpeed = _crouchMaxSpeed;
                acceleration = _crouchMaxSpeed;
                enableTransition = true;
            }

            else if(_isRunning)
            {
                _state = State.Running;
                
                _desiredMaxSpeed = _runMaxSpeed;
                acceleration = _runAcceleration;
                enableTransition = true;
            }

            else
            {
                _state = State.Walking;
                
                _desiredMaxSpeed = _walkMaxSpeed;
                acceleration = _walkAcceleration;
                enableTransition = true;
            }
            
            _currentAcceleration = acceleration;
            
            if(_lastDesiredMaxSpeed <= 0) _lastDesiredMaxSpeed = _desiredMaxSpeed;

            if(Mathf.Abs(_desiredMaxSpeed - _lastDesiredMaxSpeed) > _smoothingThreshold && enableTransition)
            {
                if(_speedControl != null) StopCoroutine(_speedControl);
                _speedControl = StartCoroutine(SmoothlyLerpMaxSpeed());
            } else _currentMaxSpeed = _desiredMaxSpeed;

            _lastDesiredMaxSpeed = _desiredMaxSpeed;
        }

        private IEnumerator SmoothlyLerpMaxSpeed()
        {
            float time = 0f;
            float difference = Mathf.Abs(_desiredMaxSpeed - _currentMaxSpeed) * _timeSmoothing;
            float startValue = _currentMaxSpeed;

            while(time < difference)
            {
                _currentMaxSpeed = Mathf.Lerp(startValue, _desiredMaxSpeed, time/difference);
                time += Time.deltaTime;

                yield return null;
            }

            _currentMaxSpeed = _desiredMaxSpeed;
        }
    }
}