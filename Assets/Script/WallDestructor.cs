using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;

[Serializable]
public class BladePlane
{
    public List<Transform> PointsLeft;
    public List<Transform> PointsRight;
}

public class WallDestructor : MonoBehaviour
{
    public BladePlane CutPointsGroup;
    public Material FillerMaterial;

    private Mesh root;
    private List<GameObject> levelOne;
    private GameObject[] results;

    private void Awake()
    {
        results = new GameObject[2];
        levelOne = new List<GameObject>();
        root = GetComponent<MeshFilter>().mesh;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.S))
        {
            //Split the root 
            // Vector3 normal = Vector3.right + (Vector3.up / 2);
            // CutSide(CutPointsGroup.PointsLeft.ToArray(),normal );
            CutSide(CutPointsGroup.PointsLeft.ToArray(), Vector3.right, Vector3.up);
            CutSide(CutPointsGroup.PointsRight.ToArray(), Vector3.left, Vector3.up);
            //Split the fragment in tiny fragment

        }

    }

    private void CutSide(Transform[] CutPoints, Vector3 normal)
    {
        for (int i = 0; i < CutPoints.Length; i++)
        {
            GameObject toFill = new GameObject();
            Cutter.Cut(ref results, this.gameObject, CutPoints[i].position, normal, FillerMaterial, toFill);
            levelOne.Add(toFill);
        }
    }

    private void CutSide(Transform[] CutPoints, Vector3 normalX, Vector3 normalY)
    {
        for (int i = 0; i < CutPoints.Length; i++)
        {
            Vector3 normal = Vector3.one;
            GameObject toFill = new GameObject();

            if (i == 0)
            {
                normal = normalX + normalY ;
            }
            else if (i == 1)
            {
                normal = normalX + (normalY / 3);

            }
            else if (i == 2)
            {
                normal = normalX + (normalY / 7);

            }
                Cutter.Cut(ref results, this.gameObject, CutPoints[i].position, normal, FillerMaterial, toFill);

        }
    }

    //private void CutSide(Transform[] CutPoints, Vector3 normalX, Vector3 normalY)
    //{
    //    int divider = 3;

    //    for (int i = 0; i < CutPoints.Length; i++)
    //    {
    //        Vector3 normal = Vector3.one;
    //        if (i == 0)
    //        {
    //            normal = normalX + (normalY / 2);
    //        }
    //        //else if(i == 1)
    //        //{

    //        //    Vector3 normal = normalX + (normalY / divider);
    //        //}

    //        GameObject toFill = new GameObject();
    //        Cutter.Cut(ref results, this.gameObject, CutPoints[i].position, normal, FillerMaterial, toFill);
    //        divider += 1;
    //    }
    //}

}

//public static GameObject[] Cut(ref GameObject[] array, GameObject toCut, Vector3 anchorPoint, Vector3 normalDirection, Material material, GameObject toFill)
//{
//    // set the blade relative to victim, from world space to local space
//    plane = new Plane(toCut.transform.InverseTransformDirection(-normalDirection), toCut.transform.InverseTransformPoint(anchorPoint));

//    // get the victims mesh
//    toBeCut = toCut.GetComponent<MeshFilter>().mesh;


//    // reset values
//    cuppingVertices.Clear();

//    leftSide = new MeshCreator();
//    rightSide = new MeshCreator();


//    bool[] sides = new bool[3];
//    int[] indices;
//    int p1;
//    int p2;
//    int p3;

//    // go throught the submeshes
//    for (int sub = 0; sub < toBeCut.subMeshCount; sub++)
//    {
//        indices = toBeCut.GetTriangles(sub);

//        //Calculate if all vertex of triangle are on one side of the plane, or if the plane cut the triangle
//        for (int i = 0; i < indices.Length; i += 3)
//        {
//            p1 = indices[i];
//            p2 = indices[i + 1];
//            p3 = indices[i + 2];

//            sides[0] = plane.GetSide(toBeCut.vertices[p1]);
//            sides[1] = plane.GetSide(toBeCut.vertices[p2]);
//            sides[2] = plane.GetSide(toBeCut.vertices[p3]);


//            // whole triangle is one one side
//            if (sides[0] == sides[1] && sides[0] == sides[2])
//            {

//                if (sides[0])
//                {
//                    // left side of plane
//                    leftSide.AddTriangle(
//                        new Vector3[] { toBeCut.vertices[p1], toBeCut.vertices[p2], toBeCut.vertices[p3] },
//                        new Vector3[] { toBeCut.normals[p1], toBeCut.normals[p2], toBeCut.normals[p3] },
//                        new Vector2[] { toBeCut.uv[p1], toBeCut.uv[p2], toBeCut.uv[p3] },
//                        new Vector4[] { toBeCut.tangents[p1], toBeCut.tangents[p2], toBeCut.tangents[p3] },
//                        sub);
//                }
//                else
//                {
//                    // right side of plane
//                    rightSide.AddTriangle(
//                        new Vector3[] { toBeCut.vertices[p1], toBeCut.vertices[p2], toBeCut.vertices[p3] },
//                        new Vector3[] { toBeCut.normals[p1], toBeCut.normals[p2], toBeCut.normals[p3] },
//                        new Vector2[] { toBeCut.uv[p1], toBeCut.uv[p2], toBeCut.uv[p3] },
//                        new Vector4[] { toBeCut.tangents[p1], toBeCut.tangents[p2], toBeCut.tangents[p3] },
//                        sub);
//                }

//            }
//            else
//            { // cut the triangle

//                CutMesh(
//                    new Vector3[] { toBeCut.vertices[p1], toBeCut.vertices[p2], toBeCut.vertices[p3] },
//                    new Vector3[] { toBeCut.normals[p1], toBeCut.normals[p2], toBeCut.normals[p3] },
//                    new Vector2[] { toBeCut.uv[p1], toBeCut.uv[p2], toBeCut.uv[p3] },
//                    new Vector4[] { toBeCut.tangents[p1], toBeCut.tangents[p2], toBeCut.tangents[p3] },
//                    sub);
//            }
//        }
//    }

//    // The capping Material will be at the end
//    Material[] materials;

//    materials = toCut.GetComponent<MeshRenderer>().sharedMaterials;


//    if (materials[materials.Length - 1].name != material.name)
//    {
//        Material[] newMats = new Material[materials.Length + 1];
//        materials.CopyTo(newMats, 0);
//        newMats[materials.Length] = material;
//        materials = newMats;
//    }

//    capMatSub = materials.Length - 1; // for later use

//    // cap the opennings
//    Capping();

//    // Left Mesh
//    Mesh left_HalfMesh = leftSide.GetMesh();
//    left_HalfMesh.name = "Split Mesh Left";

//    // Right Mesh
//    Mesh right_HalfMesh = rightSide.GetMesh();
//    right_HalfMesh.name = "Split Mesh Right";

//    // assign the game objects
//    toCut.name = "Left Side";

//    toCut.GetComponent<MeshFilter>().mesh = left_HalfMesh;

//    GameObject leftSideObj = toCut;

//    if (leftSideObj.GetComponent<MeshCollider>() == null)
//        leftSideObj.AddComponent<MeshCollider>();
//    else
//        leftSideObj.GetComponent<MeshCollider>().sharedMesh = left_HalfMesh;

//    leftSideObj.GetComponent<MeshCollider>().sharedMesh = left_HalfMesh;
//    leftSideObj.GetComponent<MeshCollider>().convex = true;

//    toFill.name = "Right Side";

//    if (toFill.GetComponent<MeshFilter>() == null && toFill.GetComponent<MeshRenderer>() == null)
//    {
//        toFill.AddComponent<MeshFilter>();
//        toFill.AddComponent<MeshRenderer>();
//    }

//    if (toFill.GetComponent<Rigidbody>() == null)
//        toFill.AddComponent<Rigidbody>();

//    if (toFill.GetComponent<MeshCollider>() == null)
//        toFill.AddComponent<MeshCollider>();

//    toFill.GetComponent<MeshCollider>().sharedMesh = right_HalfMesh;
//    toFill.GetComponent<MeshCollider>().convex = true;

//    toFill.transform.position = toCut.transform.position;
//    toFill.transform.rotation = toCut.transform.rotation;

//    toFill.GetComponent<MeshFilter>().mesh = right_HalfMesh;


//    if (toCut.transform.parent != null)
//        toFill.transform.parent = toCut.transform.parent;

//    toFill.transform.localScale = toCut.transform.localScale;

//    // assign materials
//    leftSideObj.GetComponent<MeshRenderer>().materials = materials;
//    toFill.GetComponent<MeshRenderer>().materials = materials;

//#if DEBUG
//    if (leftSideObj.GetComponent<Tester>() == null)
//        leftSideObj.AddComponent<Tester>();

//    if (toFill.GetComponent<Tester>() == null)
//        toFill.AddComponent<Tester>();
//#endif

//    array[0] = leftSideObj;
//    array[1] = toFill;

//    return array;
//}
