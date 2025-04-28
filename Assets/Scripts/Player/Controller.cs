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
        [SerializeField, Tooltip("The maximum angle of the slope the player can walk on."), Range(0, 90)] private float maxSlopeAngle;
        [SerializeField] private float jumpCooldown;

        [Header("Runtime variables")]
        private bool _isJumping;
        private float _jumpCooldownTimer = 0f;

        [Header("References")]
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
                Jump();

            if (_jumpCooldownTimer > 0f)
                _jumpCooldownTimer -= Time.deltaTime;

            if (_jumpCooldownTimer <= 0f)
                _isJumping = !IsGrounded();
        }

        private void FixedUpdate()
        {
            MovePlayer();

            if (IsNearWall() && _moveAmount.y > 0f)
                Climb();
        }

        private void MovePlayer()
        {
            if (IsOnSlope(out Vector2 slopeDirection))
                SlopeMovement(slopeDirection);
            else
                FlatMovement();
        }

        private void SlopeMovement(Vector2 slopeDirection)
        {
            if (!_isJumping)
            {
                _rb.AddForce(slopeDirection * 200f, ForceMode2D.Force);
                _rb.linearVelocity = new Vector2(_moveAmount.x * speed, 0f);
                _rb.gravityScale = 0f;
            }
        }

        private void FlatMovement()
        {
            _rb.linearVelocity = new Vector2(_moveAmount.x * speed, _rb.linearVelocity.y);
            _rb.gravityScale = 1f;
        }

        private void Jump()
        {
            _isJumping = true;
            _jumpCooldownTimer = jumpCooldown;
            _rb.linearVelocityY = 0f;
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

        private bool IsOnSlope(out Vector2 slopeDirection)
        {
            slopeDirection = Vector2.zero;
            RaycastHit2D slopeHit = Physics2D.Raycast(transform.position, Vector2.down, 1.2f, LayerMask.GetMask("Ground"));

            if (slopeHit)
            {
                float angle = Vector2.Angle(Vector2.up, slopeHit.normal);

                if (angle < maxSlopeAngle && angle > 0)
                {
                    slopeDirection = -slopeHit.normal;
                    return true;
                }
            }

            return false;
        }
    }
}
