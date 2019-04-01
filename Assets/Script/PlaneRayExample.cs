using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneRayExample : MonoBehaviour
{
    //Attach a cube GameObject in the Inspector before entering Play Mode
    public GameObject m_Cube;
    //This is the distance the clickable plane is from the camera. Set it in the Inspector before running.
    public float m_DistanceZ;

    Plane m_Plane;
    Vector3 m_DistanceFromCamera;

    void Start()
    {
        //This is how far away from the Camera the plane is placed
        m_DistanceFromCamera = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z - m_DistanceZ);
        //Create a new plane with normal (0,1,0) at the position away from the camera you define in the Inspector. This is the plane that you can click so make sure it is reachable.
        m_Plane = new Plane(Vector3.forward, m_DistanceFromCamera);
    }

    void Update()
    {
        //Detect when there is a mouse click
        if (Input.GetMouseButton(0))
        {
            //Create a ray from the Mouse click position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Initialise the enter variable
            float enter = 0.0f;

            if (m_Plane.Raycast(ray, out enter))
            {
                //Get the point that is clicked
                Vector3 hitPoint = ray.GetPoint(enter);
                //Move your cube GameObject to the point where you clicked
                m_Cube.transform.position = hitPoint;
                Debug.Log(hitPoint);
            }
        }
    }
}
