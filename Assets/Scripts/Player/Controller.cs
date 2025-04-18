using UnityEngine;
using UnityEngine.InputSystem;

namespace MiningGame.Player
{
    public class Controller : MonoBehaviour
    {
        public InputActionAsset InputActions;
        [SerializeField] private float speed;
        [SerializeField] private float jumpForce;
        private Rigidbody2D _rb;
        private InputAction _moveAction;
        private InputAction _jumpAction;
        private Vector2 _moveAmount;

        private void OnEnable()
        {
            InputActions.FindActionMap("Player").Enable();
        }

        private void OnDisable()
        {
            InputActions.FindActionMap("Player").Disable();
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _moveAction = InputSystem.actions.FindAction("Move");
            _jumpAction = InputSystem.actions.FindAction("Jump");
        }

        private void Update()
        {
            _moveAmount = _moveAction.ReadValue<Vector2>();

            if (_jumpAction.WasPressedThisFrame())
            {
                Jump();
            }
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }

        private void MovePlayer()
        {
            _rb.linearVelocity = new Vector2(_moveAmount.x * speed, _rb.linearVelocity.y);
        }

        private void Jump()
        {
            _rb.AddForceAtPosition(Vector2.up * jumpForce, _rb.position, ForceMode2D.Impulse);
        }
    }
}
