using UnityEngine;
using UnityEngine.Rendering;

namespace MiningGame.Player
{
    public class GrappleHook : MonoBehaviour
    {
        [SerializeField] private float _grappleLenght = 10f;
        [SerializeField] private LayerMask grappleLayer;
        [SerializeField] private LineRenderer rope;

        private Vector2 _grapplePos;
        private DistanceJoint2D joint;
        private Camera cam;

        void Start()
        {
            joint = gameObject.GetComponent<DistanceJoint2D>();
            joint.enabled = false;
            rope.enabled = false;

            cam = Camera.main;
        }

        void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                    Vector2 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);

                    RaycastHit2D hit = Physics2D.Raycast(
                    origin: mouseWorldPos,
                    direction: Vector2.zero,
                    distance: _grappleLenght,
                    layerMask: grappleLayer
                );

                if (hit.collider != null)
                {
                    float distanceToHit = Vector2.Distance(transform.position, hit.point);

                    if (distanceToHit <= _grappleLenght)
                    {
                        _grapplePos = hit.collider.ClosestPoint(mouseWorldPos);
                        joint.connectedAnchor = _grapplePos;
                        joint.enabled = true;
                        joint.distance = Vector2.Distance(transform.position, _grapplePos);
                        rope.SetPosition(0, _grapplePos);
                        rope.SetPosition(1, transform.position);
                        rope.enabled = true;
                    }
                }
            }

            if(Input.GetMouseButtonUp(0))
            {
                joint.enabled = false;
                rope.enabled = false;
            }

            if(rope.enabled == true)
            {
                rope.SetPosition(1, transform.position);
            }

            if (joint.enabled)
            {
                joint.distance = Mathf.MoveTowards(joint.distance, 0f, Time.deltaTime * 5f);
            }
        }
    }
}
