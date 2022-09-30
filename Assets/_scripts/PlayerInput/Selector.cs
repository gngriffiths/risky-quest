using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{



    private Camera cam;


    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame

    
    public Ray GetRayFromCamera()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }



        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = cam.farClipPlane; // Change according to your needs
        Vector3 worldPoint = cam.ScreenToWorldPoint(screenPoint);
        Vector3 origin = cam.transform.position;
        Vector3 direction = (worldPoint - origin).normalized;

        return new Ray(origin, direction);



    }

    public Ray GetRayToPoint(Vector3 _point)
    {
        if (cam == null)
        {
            cam = Camera.main;
        }



        Vector3 origin = cam.transform.position;
        Vector3 direction = (_point - origin).normalized;

        return new Ray(origin, direction);



    }

   


}
