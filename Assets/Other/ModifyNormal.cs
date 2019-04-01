using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyNormal : MonoBehaviour
{
    private GameObject Plane;

    private Vector3[] array = new Vector3[121];
    void Start ()
    {
        Plane = this.gameObject;
        for (int i = 0; i < Plane.GetComponent<MeshFilter>().mesh.normals.Length; i++)
        {
            array[i] = new Vector3(0.9f, 0.5f, -0.1f);
           
        }
       Plane.GetComponent<MeshFilter>().mesh.normals = array;
        Plane.GetComponent<MeshFilter>().mesh.RecalculateBounds();
        Plane.GetComponent<MeshFilter>().mesh.RecalculateTangents();


        // Debug.Log( Plane.GetComponent<MeshFilter>().mesh.normals.Length); //= new Vector3[2] { new Vector3(0.9f,0.5f,-0.1f), new Vector3(-0.9f, -0.5f, 0.1f) }; 
    }

}
