using MiningGame.Core;
using MiningGame.Core.Interfaces;
using MiningGame.Services;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MiningGame.Player
{
    public class Controller : MonoBehaviour
    {
        private IAudioService _audioService;

        [Header("Input Settings")]
        public InputActionAsset InputActions;

        [Header("Movement Settings")]
        [SerializeField] private float speed;
        [SerializeField] private float jumpForce;
        [SerializeField, Range(0, 0.5f)] private float airControl;
        [SerializeField] private float climbSpeed;
        [SerializeField, Tooltip("The maximum angle of the slope the player can walk on."), Range(0, 90)] private float maxSlopeAngle;
        [SerializeField] private float jumpCooldown;
        [SerializeField] private LayerMask climbableLayer; //drabina
        [Header("Audio")]
        [SerializeField] private AudioClip jumpAudio;
        [Header("Runtime variables")]
        private bool _isJumping;
        private bool _isFacingRight = true;
        private bool _wasGrounded = true;
        private float _jumpCooldownTimer = 0f;
        private bool _isClimbing;

        [Header("References")]
        private Rigidbody2D _rb;
        private SpriteRenderer _sr;
        private Animator _animator;
        private InputAction _moveAction;
        private InputAction _jumpAction;
        private Vector2 _moveAmount;
        private float _jumpDirectionX = 0f;

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
            _audioService = ServiceLocator.Get<IAudioService>();
            _rb = GetComponent<Rigidbody2D>();
            _sr = GetComponentInChildren<SpriteRenderer>();
            _animator = GetComponentInChildren<Animator>();
            _moveAction = InputSystem.actions.FindAction("Move");
            _jumpAction = InputSystem.actions.FindAction("Jump");
        }

        private void Update()
        {
            ReadInput();
            UpdateJumpCooldownAndState();
            HandleLadderState();
            HandleFlip();
            HandleAnimations();
        }

        private void ReadInput()
        {
            _moveAmount = _moveAction.ReadValue<Vector2>();

            if (_jumpAction.WasPressedThisFrame() && IsGrounded())
                Jump();
        }

        private void UpdateJumpCooldownAndState()
        {
            if (_jumpCooldownTimer > 0f)
                _jumpCooldownTimer -= Time.deltaTime;

            bool grounded = IsGrounded();

            if (_jumpCooldownTimer <= 0f)
            {
                if (_wasGrounded && !grounded)
                {
                    _isJumping = true;
                    _jumpDirectionX = (_rb != null && speed != 0f) ? _rb.linearVelocity.x / speed : _moveAmount.x;
                }
                else
                {
                    _isJumping = !grounded;
                }
            }

            if (!_isJumping && grounded)
                _jumpDirectionX = 0f;

            _wasGrounded = grounded;
        }

        private void HandleLadderState()
        {
            if (IsTouchingLadder() && Mathf.Abs(_moveAmount.y) > 0.1f)
            {
                _isClimbing = true;
                _rb.gravityScale = 0.2f;
            }
            else if (!IsTouchingLadder())
            {
                _isClimbing = false;
                _rb.gravityScale = 1f;
            }
        }

        private void HandleFlip()
        {
            if (_moveAmount.x > 0f && !_isFacingRight)
                FlipSprite();
            else if (_moveAmount.x < 0f && _isFacingRight)
                FlipSprite();
        }

        private void FixedUpdate()
        {
            if (_isClimbing)
            {
                Vector2 climbVelocity = new Vector2(_moveAmount.x * speed, _moveAmount.y * climbSpeed);
                _rb.linearVelocity = climbVelocity;
            }
            else
            {
                MovePlayer();

                if (IsNearWall() && _moveAmount.y > 0f)
                    Climb();
            }
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
            if (_isJumping)
            {
                _jumpDirectionX = Mathf.Lerp(_jumpDirectionX, _moveAmount.x, airControl);
                _rb.linearVelocity = new Vector2(_jumpDirectionX * speed, _rb.linearVelocity.y);
            }
            else
            {
                _rb.linearVelocity = new Vector2(_moveAmount.x * speed, _rb.linearVelocity.y);
            }

            _rb.gravityScale = 1f;
        }

        private void Jump()
        {
            _isJumping = true;
            _audioService.PlaySfx(jumpAudio);
            _jumpCooldownTimer = jumpCooldown;
            _rb.linearVelocityY = 0f;
            _rb.AddForceAtPosition(Vector2.up * jumpForce, _rb.position, ForceMode2D.Impulse);
            _jumpDirectionX = _moveAmount.x;
        }

        private void Climb()
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _moveAmount.y * climbSpeed);
        }

        private bool IsGrounded()
        {
            return Physics2D.Raycast(transform.position, Vector2.down, 1.2f, LayerMask.GetMask("Ground")) ||
                   Physics2D.Raycast(new Vector2(transform.position.x - 0.5f, transform.position.y), Vector2.down, 1.2f, LayerMask.GetMask("Ground")) ||
                   Physics2D.Raycast(new Vector2(transform.position.x + 0.5f, transform.position.y), Vector2.down, 1.2f, LayerMask.GetMask("Ground"));
        }

        private bool IsNearWall()
        {
            return Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - .4f), Vector2.left, .7f, LayerMask.GetMask("Ground")) ||
            Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - .4f), Vector2.right, .7f, LayerMask.GetMask("Ground"));
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

        private bool IsTouchingLadder()
        {
            Vector2 checkPosition = (Vector2)transform.position + Vector2.up * 0.5f;
            float radius = 0.4f;

            Collider2D col = Physics2D.OverlapCircle(checkPosition, radius, climbableLayer);
            Debug.DrawRay(checkPosition, Vector2.up * 0.1f, col ? Color.green : Color.red, 0.1f);
            return col != null;
        }

        private void FlipSprite()
        {
            _isFacingRight = !_isFacingRight;
            _sr.flipX = !_sr.flipX;
        }

        private void HandleAnimations()
        {
            if (_rb.linearVelocityX != 0)
                _animator.SetBool("isRunning", true);
            else
                _animator.SetBool("isRunning", false);

            if (_rb.linearVelocityY > 0 && _isJumping)
                _animator.SetBool("isJumping", true);

            if (_rb.linearVelocityY < 0 && _isJumping)
            {
                _animator.SetBool("isJumping", false);
                _animator.SetBool("isFalling", true);
            }

            if (_rb.linearVelocityY == 0 && IsGrounded() || IsOnSlope(out Vector2 slopeDirection))
            {
                _animator.SetBool("isJumping", false);
                _animator.SetBool("isFalling", false);
            }
        }
        private float speedDefault;
        private float jumpDefault;
        public void saveDefaults()
        {
            speedDefault = speed;
            jumpDefault = jumpForce;
        }

        public void disableMovement()
        {
            speed = 0;
            jumpForce = 0;
        }

        public void enableMovement()
        {
            speed = speedDefault;
            jumpForce = jumpDefault;
        }
    }
}
