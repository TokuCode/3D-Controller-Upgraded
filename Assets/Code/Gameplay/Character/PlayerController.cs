using Movement3D.Helpers;
using UnityEngine;

namespace Movement3D.Gameplay
{
    public class PlayerController : Controller
    {
        [SerializeField] private Transform _orientation;
        public Transform Orientation => _orientation;
        [SerializeField] private Transform _playerObj;
        public Transform PlayerObj => _playerObj;
        [SerializeField] private Transform _combatLookAt;
        public Transform CombatLookAt => _combatLookAt;
        [SerializeField] private Transform _cameraPosition;
        public Transform CameraPosition => _cameraPosition;
        [SerializeField] private Transform _lookAt;
        public Transform LookAt => _lookAt;
        
        public static PlayerController Singleton { get; private set; }
        private IControls _controls;
        public Pipeline<InputPayload> InputPipeline { get; private set; } = new();
        public Invoker Invoker { get; private set; }

        protected override void Awake()
        {
            SetSingleton();
            _controls = InputReader.Instance;
            Invoker = new Invoker(this);
            
            base.Awake();
        }

        private void SetSingleton()
        {
            if (Singleton != null && Singleton != this)
            {
                Destroy(gameObject);
            }
            else Singleton = this;
        }

        protected override void Update()
        {
            base.Update();
            ReadInput(UpdateContext.Update);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            ReadInput(UpdateContext.FixedUpdate);
        }

        private void ReadInput(UpdateContext context)
        {
            InputPayload input = new()
            {
                MouseDelta = _controls.MouseDelta,
                MoveDirection = _controls.MoveDirection,
                Jump = _controls.Jump,
                Run = _controls.Run,
                Crouch = _controls.Crouch,
                Context = context
            };
            
            InputPipeline.Process(ref input);
        }
    }
}