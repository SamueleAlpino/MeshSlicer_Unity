using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class MeshCutter : MonoBehaviour
{
    #region Test
    //private Mesh mesh;

    //private Rigidbody body;
    //private List<Vector3> positiveSide = new List<Vector3>();
    //private List<Vector3> negativeSide = new List<Vector3>();
    //private Plane divisor;
    //private Triangle[] triangles;
    //void Start()
    //{
    //    body = GetComponent<Rigidbody>();
    //    mesh = GetComponent<MeshFilter>().mesh;
    //    divisor = new Plane(Vector3.one, Vector3.one, Vector3.one);
    //    //   divisor.SetNormalAndPosition(Vector3.one, body.centerOfMass);
    //    triangles = new Triangle[mesh.triangles.Length];
    //}

    //void Update()
    //{
    //    for (int submesh = 0; submesh < mesh.subMeshCount; submesh++)
    //    {
    //        int[] indices = mesh.GetTriangles(submesh);

    //        for (int n = 0; n < indices.Length; n += 3)
    //        {
    //            triangles[n] = new Triangle();
    //            triangles[n].index = n;

    //            for (int v = 0; v < 3; v++)
    //            {
    //                int index = indices[n + v];
    //                triangles[n].vertex = mesh.vertices[index];
    //                triangles[n].uv = mesh.uv[index];
    //                triangles[n].normal = mesh.normals[index];
    //            }
    //        }
    //    }
    //}
    #endregion


    //Preallocare gameobject in base al numero di tagli nello start
    public Material material;
    public int NumberOfCut = 1;
    public GameObject Prefab;
    public bool skinnedMesh;

    private GameObject[] pieces;
    private Rigidbody body;
    private MeshThree first;
    private List<MeshThree> threeList;

    private void Start()
    {

        body = GetComponent<Rigidbody>();
        pieces = MeshCut.Cut(this.gameObject, body.transform.position, body.transform.right, material, skinnedMesh);

        //ONLY FOR DEBUG
        //foreach (GameObject item in pieces)
        //{
        //    if (item.GetComponent<Rigidbody>() == null)
        //    {
        //        item.AddComponent<Rigidbody>();

        //        Vector3 position;
        //        if (!skinnedMesh)
        //            position = item.transform.position + item.GetComponent<MeshFilter>().mesh.bounds.center;
        //        else
        //            position = item.transform.position + item.GetComponent<SkinnedMeshRenderer>().sharedMesh.bounds.center;

        //        Instantiate(Prefab, position, Quaternion.identity);
        //    }

        //}

        first = new MeshThree(pieces[0], pieces[1]);
        threeList = new List<MeshThree>();
        threeList.Add(first);
    }
    void Update()
    {
        #region Blade
        //if (Input.GetMouseButtonDown(0))
        //{
        //    RaycastHit hit;

        //    if (Physics.Raycast(transform.position, transform.forward, out hit))
        //    {

        //        GameObject victim = hit.collider.gameObject;

        //        GameObject[] pieces = MeshCut.Cut(victim, transform.position, transform.right, material);

        //        if (!pieces[1].GetComponent<Rigidbody>())
        //            pieces[1].AddComponent<Rigidbody>();

        //        Destroy(pieces[1], 1);
        //    }

        //}
        #endregion

        if (Input.GetKeyUp(KeyCode.A))
        {
            while (NumberOfCut > 1)
            {
                for (int i = 0; i < threeList.Count; i++)
                {
                    if (threeList[i].LeftNode != null && !threeList[i].IsLeftNodeCut)
                    {
                        Vector3 cutPosition;
                        if (!skinnedMesh)
                            cutPosition = threeList[i].LeftNode.transform.position + threeList[i].LeftNode.GetComponent<MeshFilter>().mesh.bounds.center;
                        else
                            cutPosition = threeList[i].LeftNode.transform.position + threeList[i].LeftNode.GetComponent<SkinnedMeshRenderer>().sharedMesh.bounds.center;


                        pieces = MeshCut.Cut(threeList[i].LeftNode, cutPosition, new Vector3(2f, -5f, 0.2f) /* -threeList[i].LeftNode.transform.forward*/, material, skinnedMesh);

                        MeshThree another = new MeshThree(pieces[0], pieces[1]);
                        threeList.Add(another);
                        threeList[i].IsLeftNodeCut = true;
                        break;
                    }
                    else if (threeList[i].RightNode != null && !threeList[i].IsRightNodeCut)
                    {
                        Vector3 cutPosition;
                        if (!skinnedMesh)
                            cutPosition = threeList[i].RightNode.transform.position + threeList[i].RightNode.GetComponent<MeshFilter>().mesh.bounds.center;
                        else
                            cutPosition = threeList[i].RightNode.transform.position + threeList[i].RightNode.GetComponent<SkinnedMeshRenderer>().sharedMesh.bounds.center;

                        pieces = MeshCut.Cut(threeList[i].RightNode, cutPosition, new Vector3(0.5f, 0.2f - 1f) /*-threeList[i].RightNode.transform.right*/, material, skinnedMesh);

                        MeshThree another = new MeshThree(pieces[0], pieces[1]);
                        threeList.Add(another);
                        threeList[i].IsRightNodeCut = true;
                        break;
                    }
                }
                NumberOfCut--;
            }
        }
    }

    //void OnDrawGizmosSelected()
    //{

    //    Gizmos.color = Color.green;

    //    Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5.0f);
    //    Gizmos.DrawLine(transform.position + transform.up * 0.5f, transform.position + transform.up * 0.5f + transform.forward * 5.0f);
    //    Gizmos.DrawLine(transform.position + -transform.up * 0.5f, transform.position + -transform.up * 0.5f + transform.forward * 5.0f);

    //    Gizmos.DrawLine(transform.position, transform.position + transform.up * 0.5f);
    //    Gizmos.DrawLine(transform.position, transform.position + -transform.up * 0.5f);

    //}

}

internal class MeshThree
{
    public GameObject LeftNode { get; private set; }
    public GameObject RightNode { get; private set; }

    public bool IsLeftNodeCut;
    public bool IsRightNodeCut;

    public MeshThree(GameObject left, GameObject right)
    {
        this.LeftNode = left;
        this.RightNode = right;
    }
}

internal class MeshCreator
{
    public int VertCount { get { return newVertices.Count; } }

    // Mesh Values
    private List<Vector3> newVertices = new List<Vector3>();
    private List<Vector3> newNormals = new List<Vector3>();
    private List<Vector2> newUv = new List<Vector2>();
    private List<Vector4> newTangents = new List<Vector4>();
    private List<List<int>> subIndices = new List<List<int>>();

    public void AddTriangle(Vector3[] vertices, Vector3[] normals, Vector2[] uv, int submesh)
    {

        int vertCount = newVertices.Count;

        newVertices.Add(vertices[0]);
        newVertices.Add(vertices[1]);
        newVertices.Add(vertices[2]);

        newNormals.Add(normals[0]);
        newNormals.Add(normals[1]);
        newNormals.Add(normals[2]);

        newUv.Add(uv[0]);
        newUv.Add(uv[1]);
        newUv.Add(uv[2]);

        if (subIndices.Count < submesh + 1)
        {
            for (int i = subIndices.Count; i < submesh + 1; i++)
            {
                subIndices.Add(new List<int>());
            }
        }

        subIndices[submesh].Add(vertCount);
        subIndices[submesh].Add(vertCount + 1);
        subIndices[submesh].Add(vertCount + 2);

    }

    public Mesh GetMesh()
    {
        Mesh shape = new Mesh();
        shape.name = "Generated Mesh";
        shape.SetVertices(newVertices);
        shape.SetNormals(newNormals);
        shape.SetUVs(0, newUv);
        shape.SetUVs(1, newUv);

        if (newTangents.Count > 1)
            shape.SetTangents(newTangents);

        shape.subMeshCount = subIndices.Count;

        for (int i = 0; i < subIndices.Count; i++)
            shape.SetTriangles(subIndices[i], i);

        return shape;
    }

    public void AddTriangle(Vector3[] vertices, Vector3[] normals, Vector2[] uv, Vector4[] tangents, int submesh)
    {
        int vertCount = newVertices.Count;

        newVertices.Add(vertices[0]);
        newVertices.Add(vertices[1]);
        newVertices.Add(vertices[2]);

        newNormals.Add(normals[0]);
        newNormals.Add(normals[1]);
        newNormals.Add(normals[2]);

        newUv.Add(uv[0]);
        newUv.Add(uv[1]);
        newUv.Add(uv[2]);

        newTangents.Add(tangents[0]);
        newTangents.Add(tangents[1]);
        newTangents.Add(tangents[2]);

        if (subIndices.Count < submesh + 1)
        {
            for (int i = subIndices.Count; i < submesh + 1; i++)
            {
                subIndices.Add(new List<int>());
            }
        }

        subIndices[submesh].Add(vertCount);
        subIndices[submesh].Add(vertCount + 1);
        subIndices[submesh].Add(vertCount + 2);

    }

#if UNITY_EDITOR
    // Creates and returns a new mesh with generated lightmap uvs (Editor Only)
    public Mesh GetMesh_GenerateSecondaryUVSet()
    {
        Mesh shape = GetMesh();
        // for light mapping
        UnityEditor.Unwrapping.GenerateSecondaryUVSet(shape);
        return shape;
    }

    // Creates and returns a new mesh with generated lightmap uvs (Editor Only)
    public Mesh GetMesh_GenerateSecondaryUVSet(UnityEditor.UnwrapParam param)
    {
        Mesh shape = GetMesh();
        // for light mapping
        UnityEditor.Unwrapping.GenerateSecondaryUVSet(shape, param);
        return shape;
    }

#endif

}

internal struct Triangle
{
    public int index;
    public Vector3 vertex;
    public Vector3 normal;
    public Vector2 uv;
}

