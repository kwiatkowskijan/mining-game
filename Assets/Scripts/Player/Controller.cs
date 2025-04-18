using UnityEngine;
using UnityEngine.InputSystem;

namespace MiningGame.Player
{
    public class Controller : MonoBehaviour
    {
        public InputActionAsset InputActions;
        [SerializeField] private float speed;
        private Rigidbody2D _rb;
        private InputAction _moveAction;
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
        }

        private void Update()
        {
            _moveAmount = _moveAction.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }

        private void MovePlayer()
        {
            _rb.MovePosition(_rb.position + transform.right * _moveAmount * speed * Time.deltaTime);
        }
    }
}
