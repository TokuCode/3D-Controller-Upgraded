using UnityEngine;

namespace Movement3D.Gameplay
{
    public class Invoker
    {
        private readonly PlayerController player;

        private Transform _transform;
        private Transform _orientation;
        private Transform _playerObj;
        private Transform _combatLookAt;
        private Transform _cameraPosition;
        private Transform _lookAt;
        private Rigidbody _rigidbody;
        private CapsuleCollider _collider;

        public RequestCenterPosition CenterPosition { get; }
        public LocalScaleHandler LocalScale { get; }
        public HeightHandler Height { get; }
        public RadiusHanlder Radius { get; }
        public VelocityHandlder Velocity { get; }
        public AddForceCommand AddForce { get; }
        public AddRigidbodyPosition AddRigidbodyPositionAdd { get; }
        public ForwardHandler Forward { get; }
        public RightHandler Right { get; }
        public RotationHandler OrientationRotation { get; }
        public RotationHandler PlayerRotation { get; }
        public ForwardHandler PlayerForward { get; }
        public UseGravityHandler UseGravity { get; }
        public PositionHandler PlayerPosition { get; }
        public PositionHandler CombatLookAtPosition { get; }
        public PositionHandler CameraPosition { get; }
        public PositionHandler LookAtPosition { get; }
        public FullRotationHandler LookAtRotation { get; }

        public Invoker(Controller controller)
        {
            if(controller is not PlayerController playerController) return;
            
            player = playerController;
            
            _transform = player.transform;
            _orientation = player.Orientation;
            _playerObj = player.PlayerObj;
            _combatLookAt = player.CombatLookAt;
            _cameraPosition = player.CameraPosition;
            _lookAt = player.LookAt;
            _rigidbody = player.GetComponent<Rigidbody>();
            _collider = player.GetComponent<CapsuleCollider>();

            CenterPosition = new(_transform, _collider);
            LocalScale = new(_transform);
            Height = new(_collider);
            Radius = new(_collider);
            Velocity = new(_rigidbody);
            AddForce = new(_rigidbody);
            AddRigidbodyPositionAdd = new(_rigidbody);
            Forward = new(_orientation);
            Right = new(_orientation);
            OrientationRotation = new(_orientation);
            PlayerRotation = new(_playerObj);
            PlayerForward = new(_playerObj);
            UseGravity = new(_rigidbody);
            PlayerPosition = new(_playerObj);
            CombatLookAtPosition = new(_combatLookAt);
            CameraPosition = new(_cameraPosition);
            LookAtPosition = new(_lookAt);
            LookAtRotation = new(_lookAt);
        }
    }
}