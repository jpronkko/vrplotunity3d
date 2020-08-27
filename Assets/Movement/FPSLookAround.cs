using UnityEngine;

namespace FPSMover
{
    /// <summary>
    /// Put this script to the user game object to enable view direction rotation using WASD keys.
    /// If you want the main camera to move with the
    /// user object, attach the main camera as a child of this game object.
    /// </summary>
    public class FPSLookAround : MonoBehaviour
    {
        [SerializeField] Transform cam = null;
        // how strongly the mouse movement affects the viewpoint direction
        [SerializeField] float sensitivity = 100f;
        // Clamp head tilt between [-headRotationLimit, headRotationLimit]
        [SerializeField] float headRotationLimit = 90f;
        float headRotation = 0f;
       
        void Start()
        {
            // Cursor.visible = false;
            // Cursor.lockState = CursorLockMode.Locked;
        }

        // Update is called once per frame
        void Update()
        {
            var rightMouseIsDown = Input.GetMouseButton(1);
            if (!rightMouseIsDown)
                return;

            var x = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            var y = -Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

            //Debug.Log("Mouse X" + x);
            transform.Rotate(0f, x, 0f);
            headRotation += y;
            headRotation = Mathf.Clamp(headRotation, -headRotationLimit, headRotationLimit);
            cam.localEulerAngles = new Vector3(headRotation, 0f, 0f);
        }
    }
}