using Unity.Cinemachine;
using UnityEngine;
using Movement3D.Helpers;

namespace Movement3D.Gameplay
{
    public class ThirdPersonCamera : Feature
    {
        public enum CameraStyle
        {
            Exploration,
            Combat,
            Strategy,
            Immersive
        }
        
        private Transform _cameraTransform;
        private FirstPersonCamera immersiveCamera;
        
        
        [SerializeField] private CameraStyle _cameraStyle;
        public CameraStyle CurrentCamera => _cameraStyle;
        [SerializeField] private float _rotationSpeed;
        private Vector2 _lastMoveInput;
        
        [Header("Cinemachine References")]
        [SerializeField] private CinemachineCamera _exploration;
        [SerializeField] private CinemachineCamera _combat;
        [SerializeField] private CinemachineCamera _strategy;
        [SerializeField] private CinemachineCamera _immersive;
        
        [Header("Cursor")]
        [SerializeField] private bool _cursorVisible;
        [SerializeField] private CursorLockMode _cursorLockMode;

        public override void InitializeFeature(Controller controller)
        {
            base.InitializeFeature(controller);
            _dependencies.TryGetFeature(out immersiveCamera);
            _cameraTransform = Camera.main?.transform;
        }

        private void Start()
        {
            SetCameraStyle(_cameraStyle);
        }

        public override void UpdateFeature()
        {
            DebugCameraStyles();
            SetCursor();
        }

        public override void Apply(ref InputPayload @event)
        {
            if (@event.Context != UpdateContext.Update) return;
            
            RotatePlayer(@event.MoveDirection);
        }

        private void SetCursor()
        {
            Cursor.visible = _cursorVisible;
            Cursor.lockState = _cursorLockMode;
        }
        
        private void RotatePlayer(Vector2 moveDirection)
        {
            if(moveDirection != Vector2.zero) _lastMoveInput = moveDirection;

            if (_cameraStyle is CameraStyle.Exploration or CameraStyle.Strategy)
            {
                RotateCameraFreeLookAt(moveDirection);
            }
            else if (_cameraStyle == CameraStyle.Combat)
            {
                RotateCameraHardLookAt();
            }
        }

        private void RotateCameraFreeLookAt(Vector2 moveDirection)
        {
            var playerPosition = _invoker.PlayerPosition.Get();
            var viewDirection = (playerPosition - _cameraTransform.position).With(y: 0).normalized;
            _invoker.Forward.Execute(viewDirection);
            
            var forward = _invoker.Forward.Get();
            var right = _invoker.Right.Get();
            var moveIntent = (forward * moveDirection.y + right * moveDirection.x).normalized;
            var lastMoveIntent = (forward * _lastMoveInput.y + right * _lastMoveInput.x).normalized; 
            Vector3 playerForward = _invoker.PlayerForward.Get();

            if (moveIntent != Vector3.zero)
            {
                _invoker.PlayerForward.Execute(Vector3.Slerp(playerForward, moveIntent, _rotationSpeed * Time.deltaTime));
            }
            else if (lastMoveIntent != Vector3.zero)
            {
                _invoker.PlayerForward.Execute(Vector3.Slerp(playerForward, lastMoveIntent, _rotationSpeed * Time.deltaTime));
            }
        }

        private void RotateCameraHardLookAt()
        {
            var combatLookAt = _invoker.CombatLookAtPosition.Get();
            Vector3 dirToCombatLookAt = (combatLookAt - _cameraTransform.position).With(y: 0).normalized;
            _invoker.Forward.Execute(dirToCombatLookAt);
                
            _invoker.PlayerForward.Execute(dirToCombatLookAt); 
        }

        public void SetCameraStyle(CameraStyle cameraStyle)
        {
            _cameraStyle = cameraStyle;
            
            _exploration.Priority = _cameraStyle == CameraStyle.Exploration ? 100 : 0;
            _combat.Priority = _cameraStyle == CameraStyle.Combat ? 100 : 0;
            _strategy.Priority = _cameraStyle == CameraStyle.Strategy ? 100 : 0;
            _immersive.Priority = _cameraStyle == CameraStyle.Immersive ? 100 : 0;
            immersiveCamera.enabled = cameraStyle == CameraStyle.Immersive;
            if(immersiveCamera.enabled) immersiveCamera.UpdateCoordinates();
        }

        private void DebugCameraStyles()
        {
            #if UNITY_EDITOR
            
            if(Input.GetKeyDown(KeyCode.Alpha1)) SetCameraStyle(CameraStyle.Exploration);
            else if(Input.GetKeyDown(KeyCode.Alpha2)) SetCameraStyle(CameraStyle.Combat);
            else if(Input.GetKeyDown(KeyCode.Alpha3)) SetCameraStyle(CameraStyle.Strategy);
            else if(Input.GetKeyDown(KeyCode.Alpha4)) SetCameraStyle(CameraStyle.Immersive);
            
            #endif
        }
    }
}