using UnityEngine;

namespace FPSMover
{
    /// <summary>
    /// Put this script to the user game object to enable jump movement using a space bar.
    /// Add a rigidbody and a collider components to the user game object for this to work 
    /// and to implement collision detection. If you want the main camera to move with this
    /// user object, attach the main camera as a child of this game object.
    /// Note that this script also needs a gameobject to indicate the feet level for detecting
    /// collisions with the ground.
    /// 
    /// Also set the layer of the ground objects and the layer information of the variable
    /// ground layer.
    /// </summary>
    public class FPSJump : MonoBehaviour
    {
        Rigidbody rBody;
        [SerializeField] float jumpForce = 5f;
        [SerializeField] Transform groundChecker = null;
        [SerializeField] float checkRadius = 0.1f;
        [SerializeField] LayerMask groundLayer = new LayerMask();

        bool IsOnGround()
        {
            // Check from groundChecker game object transform position using a radius of checkRadius, whether
            // this object is on a surface.
            Collider[] colliders = Physics.OverlapSphere(groundChecker.position, checkRadius, groundLayer);

            if (colliders.Length > 0)
            {
                Debug.Log("On ground!");
                return true;
            }

            Debug.Log("Not On ground!");
            return false;
        }

        void Start()
        {
            rBody = GetComponent<Rigidbody>();
        }


        void Update()
        {
            // Let's jump if we are not in the air and the user presses the
            // space bar.
            if (Input.GetKeyDown(KeyCode.Space) && IsOnGround())
            {

                rBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }
}