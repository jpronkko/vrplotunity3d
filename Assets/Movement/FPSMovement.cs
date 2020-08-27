using UnityEngine;

namespace FPSMover
{
    /// <summary>
    /// Put this script to the user game object to enable lateral movement using WASD keys.
    /// Add a rigidbody and a collider components to the user game object for this to work 
    /// and to implement collision detection. If you want the main camera to move with the
    /// user object, attach the main camera as a child of this game object.
    /// </summary>
    public class FPSMovement : MonoBehaviour
    {
        [SerializeField] float speed = 1f;
        Rigidbody rBody;

        void Start()
        {
            rBody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            var x = Input.GetAxisRaw("Horizontal");
            var z = Input.GetAxisRaw("Vertical");

            var moveVec = transform.right * x + transform.forward * z;
            rBody.MovePosition(transform.position + moveVec.normalized * speed * Time.deltaTime);
        }
    }
}