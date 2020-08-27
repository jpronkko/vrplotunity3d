using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickMouseAndDrag : MonoBehaviour
{
    bool isMouseTracking = false;
    GameObject targetObject = null;
    Vector3 screenPoint = new Vector3();
    Vector3 offset = new Vector3();

    GameObject ReturnClickedObj(out RaycastHit hit)
    {
        GameObject targetObject = null;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray.origin, ray.direction * 10, out hit))
        {
            targetObject = hit.collider.gameObject;
        }
        return targetObject;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            targetObject = ReturnClickedObj(out RaycastHit raycastHit);
            if (targetObject != null)
            {
                isMouseTracking = true;
                Debug.Log("target position: " + targetObject.transform.position);
                screenPoint = Camera.main.WorldToScreenPoint(targetObject.transform.position);
                Debug.Log("screenPoint position: " + targetObject.transform.position);

            }
        }

        if(Input.GetMouseButtonUp(0))
        {
            isMouseTracking = false;
        }

        if(isMouseTracking)
        {
            var currentScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            var currentPosition = Camera.main.ScreenToWorldPoint(currentScreenPoint) + offset;
            targetObject.transform.position = currentPosition;
        }
    }
}
