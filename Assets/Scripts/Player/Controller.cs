using UnityEngine;
using UnityEngine.InputSystem;

namespace MiningGame.Player
{
    public class Controller : MonoBehaviour
    {
        [Header("Input Settings")]
        public InputActionAsset InputActions;
        [Header("Movement Settings")]
        [SerializeField] private float speed;
        [SerializeField] private float jumpForce;
        [SerializeField] private float climbSpeed;
        [Tooltip("The maximum angle of the slope the player can walk on.")]
        [Range(0, 90)]
        [SerializeField] private float maxSlopeAngle;
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

            if (_jumpAction.WasPressedThisFrame() && IsGrounded())
            {
                Jump();
            }

            IsOnSlope();
        }

        private void FixedUpdate()
        {
            MovePlayer();

            if (IsNearWall() && _moveAmount.y > 0f)
            {
                Climb();
            }
        }
            
        private void MovePlayer()
        {
            _rb.linearVelocity = new Vector2(_moveAmount.x * speed, _rb.linearVelocity.y);
        }

        private void Jump()
        {
            _rb.AddForceAtPosition(Vector2.up * jumpForce, _rb.position, ForceMode2D.Impulse);
        }

        private void Climb()
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _moveAmount.y * climbSpeed);
        }

        private bool IsGrounded()
        {
            return Physics2D.Raycast(transform.position, Vector2.down, 1.2f, LayerMask.GetMask("Ground"));
        }

        private bool IsNearWall()
        {
            return Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - .9f), Vector2.left, .6f, LayerMask.GetMask("Ground")) ||
            Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - .9f), Vector2.right, .6f, LayerMask.GetMask("Ground"));
        }

        private bool IsOnSlope()
        {
            RaycastHit2D _slopeHit = Physics2D.Raycast(transform.position, Vector2.down, 1.2f, LayerMask.GetMask("Ground"));

            if (_slopeHit)
            {
                float angle = Vector2.Angle(Vector2.up, _slopeHit.normal);
                return angle < maxSlopeAngle;
            }

            return false;
        }
    }
}
