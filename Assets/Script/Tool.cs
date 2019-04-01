using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
[RequireComponent(typeof(MeshRenderer))]
public class Tool : MonoBehaviour
{
    public int numberOfCut = 1;
    public Material fillerMaterial;
    public string PrefabName;

    private List<MeshThree> threeList;
    private MeshThree topThree;
    private Mesh toSave;
    private GameObject[] pieces;
    private GameObject[] preAllocation;
    private float minRange = -360;
    private float maxRange = 360;

    private void Awake()
    {
        toSave = new Mesh();
        pieces = new GameObject[2];
        threeList = new List<MeshThree>();
        //Preallocation of GameObject
        preAllocation = new GameObject[numberOfCut];
        for (int i = 0; i < preAllocation.Length; i++)
        {
            preAllocation[i] = new GameObject("EmptyObject");
            preAllocation[i].SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            GameObject root = new GameObject(PrefabName);

            //Active the first preallocated gameObject
            preAllocation[0].SetActive(true);

            //Cut the Mesh
            Cutter.Cut(ref pieces, this.gameObject, transform.position + GetComponent<MeshFilter>().mesh.bounds.center, new Vector3(Random.Range(minRange, maxRange), Random.Range(minRange, maxRange), Random.Range(minRange, maxRange)), fillerMaterial, preAllocation[0]);
            topThree = new MeshThree(this.gameObject, pieces[1]);
            threeList.Add(topThree);

            foreach (var piece in pieces)
                piece.transform.SetParent(root.transform);

            while (numberOfCut > 1)
            {
                for (int i = 0; i < threeList.Count; i++)
                {
                    if (threeList[i].LeftNode != null && !threeList[i].IsLeftNodeCut)
                    {
                        Cutting(threeList[i].LeftNode, root);
                        threeList[i].IsLeftNodeCut = true;
                        break;
                    }
                    else if (threeList[i].RightNode != null && !threeList[i].IsRightNodeCut)
                    {
                        Cutting(threeList[i].RightNode, root);
                        threeList[i].IsRightNodeCut = true;
                        break;
                    }
                }
                numberOfCut--;
            }


            //Create Folder for meshes
            string newFolder = Path.Combine(Application.dataPath, "Meshes");

            if (!Directory.Exists(newFolder))
                newFolder = AssetDatabase.CreateFolder("Assets", "Meshes");

            //Save the mesh for create prefab at runtime
            int n = root.GetComponentsInChildren<MeshFilter>().Length;
            int k = 0;
            for (int i = 0; i < n; i++)
            {
                if (root.GetComponentsInChildren<MeshFilter>()[i].mesh != null)
                {
                    toSave = root.GetComponentsInChildren<MeshFilter>()[i].mesh;

                    AssetDatabase.CreateAsset(toSave, "Assets/Meshes/Mesh" + k.ToString() + ".asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                k++;
            }

            string path = EditorUtility.SaveFilePanelInProject("Save Prefab", PrefabName, "prefab", "Save Sliced Mesh");

            if (!string.IsNullOrEmpty(path))
                PrefabUtility.CreatePrefab(path, root);

        }

        //}

        //void Update()
        //{
        //    if (Input.GetKeyUp(KeyCode.A) || Slice)
        //    {
        //        GameObject root = new GameObject(PrefabName);

        //        //Active the first preallocated gameObject
        //        preAllocation[0].SetActive(true);

        //        //Cut the Mesh
        //        Cutter.Cut(ref pieces, this.gameObject, transform.position + GetComponent<MeshFilter>().mesh.bounds.center, new Vector3(Random.Range(minRange, maxRange), Random.Range(minRange, maxRange), Random.Range(minRange, maxRange)), fillerMaterial, preAllocation[0]);
        //        topThree = new MeshThree(this.gameObject, pieces[1]);
        //        threeList.Add(topThree);

        //        foreach (var piece in pieces)
        //            piece.transform.SetParent(root.transform);

        //        while (numberOfCut > 1)
        //        {
        //            for (int i = 0; i < threeList.Count; i++)
        //            {
        //                if (threeList[i].LeftNode != null && !threeList[i].IsLeftNodeCut)
        //                {
        //                    Cutting(threeList[i].LeftNode, root);
        //                    threeList[i].IsLeftNodeCut = true;
        //                    break;
        //                }
        //                else if (threeList[i].RightNode != null && !threeList[i].IsRightNodeCut)
        //                {
        //                    Cutting(threeList[i].RightNode,root);
        //                    threeList[i].IsRightNodeCut = true;
        //                    break;
        //                }
        //            }
        //            numberOfCut--;
        //        }


        //        //Create Folder for meshes
        //        string newFolder = Path.Combine(Application.dataPath, "Meshes");

        //        if (!Directory.Exists(newFolder))
        //            newFolder = AssetDatabase.CreateFolder("Assets", "Meshes");

        //        //Save the mesh for create prefab at runtime
        //        int n = root.GetComponentsInChildren<MeshFilter>().Length;
        //        int k = 0;
        //        for (int i = 0; i < n; i++)
        //        {
        //            if (root.GetComponentsInChildren<MeshFilter>()[i].mesh != null)
        //            {
        //                toSave = root.GetComponentsInChildren<MeshFilter>()[i].mesh;

        //                AssetDatabase.CreateAsset(toSave, "Assets/Meshes/Mesh" + k.ToString() + ".asset");
        //                AssetDatabase.SaveAssets();
        //                AssetDatabase.Refresh();
        //            }
        //            k++;
        //        }

        //        string path = EditorUtility.SaveFilePanelInProject("Save Prefab", PrefabName , "prefab", "Save Sliced Mesh");

        //        if (!string.IsNullOrEmpty(path))
        //            PrefabUtility.CreatePrefab(path, root);

        //        Slice = false;
        //    }

        //}
    }
    private void Cutting(GameObject node, GameObject root)
    {
        Vector3 cutPosition = node.transform.position + node.GetComponent<MeshFilter>().mesh.bounds.center;

        for (int j = 0; j < preAllocation.Length; j++)
        {
            if (!preAllocation[j].activeSelf)
            {
                preAllocation[j].SetActive(true);
                Cutter.Cut(ref pieces, node, cutPosition, new Vector3(Random.Range(minRange, maxRange), Random.Range(minRange, maxRange), Random.Range(minRange, maxRange)), fillerMaterial, preAllocation[j]);

                foreach (GameObject piece in this.pieces)
                {
                    piece.transform.SetParent(root.transform);
                    piece.transform.localPosition = Vector3.zero;
                }

                break;
            }
        }

        MeshThree another = new MeshThree(pieces[0], pieces[1]);
        threeList.Add(another);
    }
}

internal class Cutter
{
    private static MeshCreator leftSide = new MeshCreator();
    private static MeshCreator rightSide = new MeshCreator();

    private static Plane plane;
    private static Mesh toBeCut;

    // capping stuff
    private static List<Vector3> cuppingVertices = new List<Vector3>();
    private static List<Vector3> capVertTracker = new List<Vector3>();
    private static List<Vector3> capVertpolygon = new List<Vector3>();
    private static int capMatSub = 1;

    public static void CutTest(GameObject toCut, Vector3 anchorPoint, Vector3 normalDirection, Material material, GameObject blended)
    {
        // set the blade relative to victim, from world space to local space
        plane = new Plane(normalDirection, toCut.transform.InverseTransformPoint(anchorPoint));
        Debug.Log("Direction normalized : " + normalDirection.normalized);
        Debug.Log("Anchor point : " + toCut.transform.InverseTransformPoint(anchorPoint));
        // get the victims mesh
        toBeCut = toCut.GetComponent<MeshFilter>().mesh;

        // reset values
        cuppingVertices.Clear();

        leftSide = new MeshCreator();
        rightSide = new MeshCreator();

        bool[] sides = new bool[3];
        int[] indices;
        int p1;
        int p2;
        int p3;

        // go throught the submeshes
        for (int sub = 0; sub < toBeCut.subMeshCount; sub++)
        {
            indices = toBeCut.GetTriangles(sub);
            //Calculate if all vertex of triangle are on one side of the plane, or if the plane cut the triangle
            for (int i = 0; i < indices.Length; i += 3)
            {
                p1 = indices[i];
                p2 = indices[i + 1];
                p3 = indices[i + 2];

                sides[0] = plane.GetSide(toBeCut.vertices[p1]);
                sides[1] = plane.GetSide(toBeCut.vertices[p2]);
                sides[2] = plane.GetSide(toBeCut.vertices[p3]);

                // whole triangle is on one side
                if (sides[0] == sides[1] && sides[0] == sides[2])
                {
                    if (sides[0])
                    {
                        // left side of plane
                        leftSide.AddTriangle(
                            new Vector3[] { toBeCut.vertices[p1], toBeCut.vertices[p2], toBeCut.vertices[p3] },
                            new Vector3[] { toBeCut.normals[p1], toBeCut.normals[p2], toBeCut.normals[p3] },
                            new Vector2[] { toBeCut.uv[p1], toBeCut.uv[p2], toBeCut.uv[p3] },
                            new Vector4[] { toBeCut.tangents[p1], toBeCut.tangents[p2], toBeCut.tangents[p3] },
                            sub);
                    }
                    else
                    {
                        // right side of plane
                        rightSide.AddTriangle(
                            new Vector3[] { toBeCut.vertices[p1], toBeCut.vertices[p2], toBeCut.vertices[p3] },
                            new Vector3[] { toBeCut.normals[p1], toBeCut.normals[p2], toBeCut.normals[p3] },
                            new Vector2[] { toBeCut.uv[p1], toBeCut.uv[p2], toBeCut.uv[p3] },
                            new Vector4[] { toBeCut.tangents[p1], toBeCut.tangents[p2], toBeCut.tangents[p3] },
                            sub);
                    }
                }
                else
                {
                    // cut the triangle
                    CutMesh(
                        new Vector3[] { toBeCut.vertices[p1], toBeCut.vertices[p2], toBeCut.vertices[p3] },
                        new Vector3[] { toBeCut.normals[p1], toBeCut.normals[p2], toBeCut.normals[p3] },
                        new Vector2[] { toBeCut.uv[p1], toBeCut.uv[p2], toBeCut.uv[p3] },
                        new Vector4[] { toBeCut.tangents[p1], toBeCut.tangents[p2], toBeCut.tangents[p3] },
                        sub);
                }
            }
        }

        // The capping Material will be at the end
        Material[] materials;

        materials = toCut.GetComponent<MeshRenderer>().sharedMaterials;


        if (materials[materials.Length - 1].name != material.name)
        {
            Material[] newMats = new Material[materials.Length + 1];
            materials.CopyTo(newMats, 0);
            newMats[materials.Length] = material;
            materials = newMats;
        }

        capMatSub = materials.Length - 1; // for later use

        // cap the openings
        Capping();

        // Left Mesh
        Mesh left_HalfMesh = leftSide.GetMesh();
        left_HalfMesh.name = "Split Mesh Left";

        // Right Mesh
        Mesh right_HalfMesh = rightSide.GetMesh();
        right_HalfMesh.name = "Split Mesh Right";

        // assign the game objects
        toCut.name = "Left Side";

        toCut.GetComponent<MeshFilter>().mesh = left_HalfMesh;

        GameObject leftSideObj = toCut;

        if (leftSideObj.GetComponent<MeshCollider>() == null)
            leftSideObj.AddComponent<MeshCollider>();
        else
            leftSideObj.GetComponent<MeshCollider>().sharedMesh = left_HalfMesh;

        leftSideObj.GetComponent<MeshCollider>().sharedMesh = left_HalfMesh;
        leftSideObj.GetComponent<MeshCollider>().convex = true;

        blended.name = "Right Side";

        if (blended.GetComponent<MeshFilter>() == null && blended.GetComponent<MeshRenderer>() == null)
        {
            blended.AddComponent<MeshFilter>();
            blended.AddComponent<MeshRenderer>();
        }

        // if (blended.GetComponent<Rigidbody>() == null)
        //     blended.AddComponent<Rigidbody>();

        if (blended.GetComponent<MeshCollider>() == null)
            blended.AddComponent<MeshCollider>();

        blended.GetComponent<MeshCollider>().sharedMesh = right_HalfMesh;
        blended.GetComponent<MeshCollider>().convex = true;

        blended.transform.position = toCut.transform.position;
        blended.transform.rotation = toCut.transform.rotation;

        blended.GetComponent<MeshFilter>().mesh = right_HalfMesh;


        if (toCut.transform.parent != null)
            blended.transform.parent = toCut.transform.parent;

        blended.transform.localScale = toCut.transform.localScale;

        // assign materials
        leftSideObj.GetComponent<MeshRenderer>().materials = materials;
        blended.GetComponent<MeshRenderer>().materials = materials;

    }

    public static GameObject[] CutTwo(ref GameObject[] array, GameObject toCut, Vector3 anchorPoint, Vector3 normalDirection, Material material, GameObject toFill)
    {
        // set the blade relative to victim, from world space to local space
        plane = new Plane(normalDirection.normalized, toCut.transform.InverseTransformPoint(anchorPoint));

        // get the victims mesh
        toBeCut = toCut.GetComponent<MeshFilter>().mesh;

        // reset values
        cuppingVertices.Clear();

        leftSide = new MeshCreator();
        rightSide = new MeshCreator();

        bool[] sides = new bool[3];
        int[] indices;
        int p1;
        int p2;
        int p3;

        // go throught the submeshes
        for (int sub = 0; sub < toBeCut.subMeshCount; sub++)
        {
            indices = toBeCut.GetTriangles(sub);
            //Calculate if all vertex of triangle are on one side of the plane, or if the plane cut the triangle
            for (int i = 0; i < indices.Length; i += 3)
            {
                p1 = indices[i];
                p2 = indices[i + 1];
                p3 = indices[i + 2];

                sides[0] = plane.GetSide(toBeCut.vertices[p1]);
                sides[1] = plane.GetSide(toBeCut.vertices[p2]);
                sides[2] = plane.GetSide(toBeCut.vertices[p3]);

                // whole triangle is one one side
                if (sides[0] == sides[1] && sides[0] == sides[2])
                {
                    if (sides[0])
                    {
                        // left side of plane
                        leftSide.AddTriangle(
                            new Vector3[] { toBeCut.vertices[p1], toBeCut.vertices[p2], toBeCut.vertices[p3] },
                            new Vector3[] { toBeCut.normals[p1], toBeCut.normals[p2], toBeCut.normals[p3] },
                            new Vector2[] { toBeCut.uv[p1], toBeCut.uv[p2], toBeCut.uv[p3] },
                            new Vector4[] { toBeCut.tangents[p1], toBeCut.tangents[p2], toBeCut.tangents[p3] },
                            sub);
                    }
                    else
                    {
                        // right side of plane
                        rightSide.AddTriangle(
                            new Vector3[] { toBeCut.vertices[p1], toBeCut.vertices[p2], toBeCut.vertices[p3] },
                            new Vector3[] { toBeCut.normals[p1], toBeCut.normals[p2], toBeCut.normals[p3] },
                            new Vector2[] { toBeCut.uv[p1], toBeCut.uv[p2], toBeCut.uv[p3] },
                            new Vector4[] { toBeCut.tangents[p1], toBeCut.tangents[p2], toBeCut.tangents[p3] },
                            sub);
                    }
                }
                else
                {
                    // cut the triangle
                    CutMesh(
                        new Vector3[] { toBeCut.vertices[p1], toBeCut.vertices[p2], toBeCut.vertices[p3] },
                        new Vector3[] { toBeCut.normals[p1], toBeCut.normals[p2], toBeCut.normals[p3] },
                        new Vector2[] { toBeCut.uv[p1], toBeCut.uv[p2], toBeCut.uv[p3] },
                        new Vector4[] { toBeCut.tangents[p1], toBeCut.tangents[p2], toBeCut.tangents[p3] },
                        sub);
                }
            }
        }

        // The capping Material will be at the end
        Material[] materials;

        materials = toCut.GetComponent<MeshRenderer>().sharedMaterials;


        if (materials[materials.Length - 1].name != material.name)
        {
            Material[] newMats = new Material[materials.Length + 1];
            materials.CopyTo(newMats, 0);
            newMats[materials.Length] = material;
            materials = newMats;
        }

        capMatSub = materials.Length - 1; // for later use

        // cap the opennings
        Capping();

        // Left Mesh
        Mesh left_HalfMesh = leftSide.GetMesh();
        left_HalfMesh.name = "Split Mesh Left";

        // Right Mesh
        Mesh right_HalfMesh = rightSide.GetMesh();
        right_HalfMesh.name = "Split Mesh Right";

        // assign the game objects
        toCut.name = "Left Side";

        toCut.GetComponent<MeshFilter>().mesh = left_HalfMesh;

        GameObject leftSideObj = toCut;

        if (leftSideObj.GetComponent<MeshCollider>() == null)
            leftSideObj.AddComponent<MeshCollider>();
        else
            leftSideObj.GetComponent<MeshCollider>().sharedMesh = left_HalfMesh;

        leftSideObj.GetComponent<MeshCollider>().sharedMesh = left_HalfMesh;
        leftSideObj.GetComponent<MeshCollider>().convex = true;

        toFill.name = "Right Side";

        if (toFill.GetComponent<MeshFilter>() == null && toFill.GetComponent<MeshRenderer>() == null)
        {
            toFill.AddComponent<MeshFilter>();
            toFill.AddComponent<MeshRenderer>();
        }

        if (toFill.GetComponent<Rigidbody>() == null)
            toFill.AddComponent<Rigidbody>();

        if (toFill.GetComponent<MeshCollider>() == null)
            toFill.AddComponent<MeshCollider>();

        toFill.GetComponent<MeshCollider>().sharedMesh = right_HalfMesh;
        toFill.GetComponent<MeshCollider>().convex = true;

        toFill.transform.position = toCut.transform.position;
        toFill.transform.rotation = toCut.transform.rotation;

        toFill.GetComponent<MeshFilter>().mesh = right_HalfMesh;


        if (toCut.transform.parent != null)
            toFill.transform.parent = toCut.transform.parent;

        toFill.transform.localScale = toCut.transform.localScale;

        // assign materials
        leftSideObj.GetComponent<MeshRenderer>().materials = materials;
        toFill.GetComponent<MeshRenderer>().materials = materials;

#if DEBUG
        if (leftSideObj.GetComponent<Tester>() == null)
            leftSideObj.AddComponent<Tester>();

        if (toFill.GetComponent<Tester>() == null)
            toFill.AddComponent<Tester>();
#endif

        array[0] = leftSideObj;
        array[1] = toFill;

        return array;
    }

    public static GameObject[] CutRefact(ref GameObject[] array, GameObject toCut, Vector3 anchorPoint, Vector3 normalDirection, Material material, GameObject toFill)
    {
        // set the blade relative to victim, from world space to local space
        plane = new Plane(toCut.transform.InverseTransformDirection(-normalDirection), toCut.transform.InverseTransformPoint(anchorPoint));

        // get the victims mesh
        toBeCut = toCut.GetComponent<MeshFilter>().mesh;


        // reset values
        cuppingVertices.Clear();

        leftSide = new MeshCreator();
        rightSide = new MeshCreator();


        bool[] sides = new bool[3];
        int[] indices;
        int p1;
        int p2;
        int p3;

        // go throught the submeshes
        for (int sub = 0; sub < toBeCut.subMeshCount; sub++)
        {
            indices = toBeCut.GetTriangles(sub);

            //Calculate if all vertex of triangle are on one side of the plane, or if the plane cut the triangle
            for (int i = 0; i < indices.Length; i += 3)
            {
                p1 = indices[i];
                p2 = indices[i + 1];
                p3 = indices[i + 2];

                sides[0] = plane.GetSide(toBeCut.vertices[p1]);
                sides[1] = plane.GetSide(toBeCut.vertices[p2]);
                sides[2] = plane.GetSide(toBeCut.vertices[p3]);


                // whole triangle is on one side
                if (sides[0] == sides[1] && sides[0] == sides[2] && sides[1] == sides[2])
                {
                    //Left side is the positive side
                    leftSide.AddTriangle(
                        new Vector3[] { toBeCut.vertices[p1], toBeCut.vertices[p2], toBeCut.vertices[p3] },
                        new Vector3[] { toBeCut.normals[p1], toBeCut.normals[p2], toBeCut.normals[p3] },
                        new Vector2[] { toBeCut.uv[p1], toBeCut.uv[p2], toBeCut.uv[p3] },
                        new Vector4[] { toBeCut.tangents[p1], toBeCut.tangents[p2], toBeCut.tangents[p3] },
                        sub);
                }
                else // The triangle need to be cut 
                {
                    CutMesh(
                        new Vector3[] { toBeCut.vertices[p1], toBeCut.vertices[p2], toBeCut.vertices[p3] },
                        new Vector3[] { toBeCut.normals[p1], toBeCut.normals[p2], toBeCut.normals[p3] },
                        new Vector2[] { toBeCut.uv[p1], toBeCut.uv[p2], toBeCut.uv[p3] },
                        new Vector4[] { toBeCut.tangents[p1], toBeCut.tangents[p2], toBeCut.tangents[p3] },
                        sub);
                }
            }
        }

        // The capping Material will be at the end
        Material[] materials;

        materials = toCut.GetComponent<MeshRenderer>().sharedMaterials;


        if (materials[materials.Length - 1].name != material.name)
        {
            Material[] newMats = new Material[materials.Length + 1];
            materials.CopyTo(newMats, 0);
            newMats[materials.Length] = material;
            materials = newMats;
        }

        capMatSub = materials.Length - 1; // for later use

        // cap the openings
        Capping();

        // Left Mesh
        Mesh left_HalfMesh = leftSide.GetMesh();
        left_HalfMesh.name = "Split Mesh Left";

        // Right Mesh
        Mesh right_HalfMesh = rightSide.GetMesh();
        right_HalfMesh.name = "Split Mesh Right";

        // assign the game objects
        toCut.name = "Left Side";

        toCut.GetComponent<MeshFilter>().mesh = left_HalfMesh;

        GameObject leftSideObj = toCut;

        if (leftSideObj.GetComponent<MeshCollider>() == null)
            leftSideObj.AddComponent<MeshCollider>();
        else
            leftSideObj.GetComponent<MeshCollider>().sharedMesh = left_HalfMesh;

        leftSideObj.GetComponent<MeshCollider>().sharedMesh = left_HalfMesh;
        leftSideObj.GetComponent<MeshCollider>().convex = true;

        toFill.name = "Right Side";

        if (toFill.GetComponent<MeshFilter>() == null && toFill.GetComponent<MeshRenderer>() == null)
        {
            toFill.AddComponent<MeshFilter>();
            toFill.AddComponent<MeshRenderer>();
        }

        if (toFill.GetComponent<Rigidbody>() == null)
            toFill.AddComponent<Rigidbody>();

        if (toFill.GetComponent<MeshCollider>() == null)
            toFill.AddComponent<MeshCollider>();

        toFill.GetComponent<MeshCollider>().sharedMesh = right_HalfMesh;
        toFill.GetComponent<MeshCollider>().convex = true;

        toFill.transform.position = toCut.transform.position;
        toFill.transform.rotation = toCut.transform.rotation;

        toFill.GetComponent<MeshFilter>().mesh = right_HalfMesh;


        if (toCut.transform.parent != null)
            toFill.transform.parent = toCut.transform.parent;

        toFill.transform.localScale = toCut.transform.localScale;

        // assign materials
        leftSideObj.GetComponent<MeshRenderer>().materials = materials;
        toFill.GetComponent<MeshRenderer>().materials = materials;

#if DEBUG
        if (leftSideObj.GetComponent<Tester>() == null)
            leftSideObj.AddComponent<Tester>();

        if (toFill.GetComponent<Tester>() == null)
            toFill.AddComponent<Tester>();
#endif

        array[0] = leftSideObj;
        array[1] = toFill;

        return array;
    }

    /// Cut the specified mesh
    public static GameObject[] Cut(ref GameObject[] array, GameObject toCut, Vector3 anchorPoint, Vector3 normalDirection, Material material, GameObject toFill)
    {
        // set the blade relative to victim, from world space to local space
        plane = new Plane(toCut.transform.InverseTransformDirection(-normalDirection), toCut.transform.InverseTransformPoint(anchorPoint));

        // get the victims mesh
        toBeCut = toCut.GetComponent<MeshFilter>().mesh;

        // reset values
        cuppingVertices.Clear();

        leftSide = new MeshCreator();
        rightSide = new MeshCreator();


        bool[] sides = new bool[3];
        int[] indices;
        int p1;
        int p2;
        int p3;

        // go throught the submeshes
        for (int sub = 0; sub < toBeCut.subMeshCount; sub++)
        {
            indices = toBeCut.GetTriangles(sub);

            //Calculate if all vertex of triangle are on one side of the plane, or if the plane cut the triangle
            for (int i = 0; i < indices.Length; i += 3)
            {
                p1 = indices[i];
                p2 = indices[i + 1];
                p3 = indices[i + 2];

                sides[0] = plane.GetSide(toBeCut.vertices[p1]);
                sides[1] = plane.GetSide(toBeCut.vertices[p2]);
                sides[2] = plane.GetSide(toBeCut.vertices[p3]);


                // whole triangle is on one side
                if (sides[0] == sides[1] && sides[0] == sides[2] && sides[1] == sides[2])
                {
                    //p1 is on positive side of the plane
                    if (sides[0])
                    {
                        // left side of plane
                        leftSide.AddTriangle(
                            new Vector3[] { toBeCut.vertices[p1], toBeCut.vertices[p2], toBeCut.vertices[p3] },
                            new Vector3[] { toBeCut.normals[p1], toBeCut.normals[p2], toBeCut.normals[p3] },
                            new Vector2[] { toBeCut.uv[p1], toBeCut.uv[p2], toBeCut.uv[p3] },
                            new Vector4[] { toBeCut.tangents[p1], toBeCut.tangents[p2], toBeCut.tangents[p3] },
                            sub);
                    }
                    else
                    {
                        // right side of plane
                        rightSide.AddTriangle(
                            new Vector3[] { toBeCut.vertices[p1], toBeCut.vertices[p2], toBeCut.vertices[p3] },
                            new Vector3[] { toBeCut.normals[p1], toBeCut.normals[p2], toBeCut.normals[p3] },
                            new Vector2[] { toBeCut.uv[p1], toBeCut.uv[p2], toBeCut.uv[p3] },
                            new Vector4[] { toBeCut.tangents[p1], toBeCut.tangents[p2], toBeCut.tangents[p3] },
                            sub);
                    }

                }
                else // The triangle need to be cut 
                {

                    CutMesh(
                        new Vector3[] { toBeCut.vertices[p1], toBeCut.vertices[p2], toBeCut.vertices[p3] },
                        new Vector3[] { toBeCut.normals[p1], toBeCut.normals[p2], toBeCut.normals[p3] },
                        new Vector2[] { toBeCut.uv[p1], toBeCut.uv[p2], toBeCut.uv[p3] },
                        new Vector4[] { toBeCut.tangents[p1], toBeCut.tangents[p2], toBeCut.tangents[p3] },
                        sub);
                }
            }
        }

        // The capping Material will be at the end
        Material[] materials;

        materials = toCut.GetComponent<MeshRenderer>().sharedMaterials;


        if (materials[materials.Length - 1].name != material.name)
        {
            Material[] newMats = new Material[materials.Length + 1];
            materials.CopyTo(newMats, 0);
            newMats[materials.Length] = material;
            materials = newMats;
        }

        capMatSub = materials.Length - 1; // for later use

        // cap the opennings
        Capping();

        // Left Mesh
        Mesh left_HalfMesh = leftSide.GetMesh();
        left_HalfMesh.name = "Split Mesh Left";

        // Right Mesh
        Mesh right_HalfMesh = rightSide.GetMesh();
        right_HalfMesh.name = "Split Mesh Right";

        // assign the game objects
        toCut.name = "Left Side";

        toCut.GetComponent<MeshFilter>().mesh = left_HalfMesh;

        GameObject leftSideObj = toCut;

        if (leftSideObj.GetComponent<MeshCollider>() == null)
            leftSideObj.AddComponent<MeshCollider>();

        if (leftSideObj.GetComponent<MeshCollider>().sharedMesh == null)
            leftSideObj.GetComponent<MeshCollider>().sharedMesh = left_HalfMesh;

        leftSideObj.GetComponent<MeshCollider>().sharedMesh = left_HalfMesh;
        leftSideObj.GetComponent<MeshCollider>().convex = true;

        toFill.name = "Right Side";

        if (toFill.GetComponent<MeshFilter>() == null && toFill.GetComponent<MeshRenderer>() == null)
        {
            toFill.AddComponent<MeshFilter>();
            toFill.AddComponent<MeshRenderer>();
        }

        if (toFill.GetComponent<Rigidbody>() == null)
            toFill.AddComponent<Rigidbody>();

        if (toFill.GetComponent<MeshCollider>() == null)
            toFill.AddComponent<MeshCollider>();

        toFill.GetComponent<Rigidbody>().useGravity = true;
        toFill.GetComponent<MeshCollider>().sharedMesh = right_HalfMesh;
        toFill.GetComponent<MeshCollider>().convex = true;

        toFill.transform.position = toCut.transform.position;
        toFill.transform.rotation = toCut.transform.rotation;

        toFill.GetComponent<MeshFilter>().mesh = right_HalfMesh;


        if (toCut.transform.parent != null)
            toFill.transform.parent = toCut.transform.parent;

        toFill.transform.localScale = toCut.transform.localScale;

        // assign materials
        leftSideObj.GetComponent<MeshRenderer>().materials = materials;
        toFill.GetComponent<MeshRenderer>().materials = materials;

#if DEBUG
        if (leftSideObj.GetComponent<Tester>() == null)
            leftSideObj.AddComponent<Tester>();

        if (toFill.GetComponent<Tester>() == null)
            toFill.AddComponent<Tester>();
#endif

        array[0] = leftSideObj;
        array[1] = toFill;

        return array;
    }

    private static void CutMesh(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, Vector4[] tangents, int submesh)
    {
        bool[] sides = new bool[3];
        // true  = left => positiveSide
        sides[0] = plane.GetSide(vertices[0]);
        sides[1] = plane.GetSide(vertices[1]);
        sides[2] = plane.GetSide(vertices[2]);

        //Left => positive
        Vector3[] leftPoints = new Vector3[2];
        Vector3[] leftNormals = new Vector3[2];
        Vector2[] leftUvs = new Vector2[2];
        Vector4[] leftTangents = new Vector4[2];
        //Right = negative
        Vector3[] rightPoints = new Vector3[2];
        Vector3[] rightNormals = new Vector3[2];
        Vector2[] rightUvs = new Vector2[2];
        Vector4[] rightTangents = new Vector4[2];

        bool setLeftOneTime = false;
        bool setRightOneTime = false;

        for (int i = 0; i < 3; i++)
        {
            if (sides[i])
            {
                if (!setLeftOneTime)
                {
                    setLeftOneTime = true;

                    leftPoints[0] = vertices[i];
                    leftUvs[0] = uvs[i];
                    leftNormals[0] = normals[i];
                    leftTangents[0] = tangents[i];

                    leftPoints[1] = leftPoints[0];
                    leftUvs[1] = leftUvs[0];
                    leftNormals[1] = leftNormals[0];
                    leftTangents[1] = leftTangents[0];
                }
                else
                {
                    leftPoints[1] = vertices[i];
                    leftUvs[1] = uvs[i];
                    leftNormals[1] = normals[i];
                    leftTangents[1] = tangents[i];
                }
            }
            else
            {
                if (!setRightOneTime)
                {
                    setRightOneTime = true;

                    rightPoints[0] = vertices[i];
                    rightUvs[0] = uvs[i];
                    rightNormals[0] = normals[i];
                    rightTangents[0] = tangents[i];

                    rightPoints[1] = rightPoints[0];
                    rightUvs[1] = rightUvs[0];
                    rightNormals[1] = rightNormals[0];
                    rightTangents[1] = rightTangents[0];
                }
                else
                {
                    rightPoints[1] = vertices[i];
                    rightUvs[1] = uvs[i];
                    rightNormals[1] = normals[i];
                    rightTangents[1] = tangents[i];
                }
            }
        }


        float normalizedDistance = 0.0f;
        float distance = 0;
        // This function sets enter to the distance along the ray, where it intersects the plane. 
        // If the ray is parallel to the plane, function returns false and sets enter to zero. 
        // If the ray is pointing in the opposite direction than the plane, function returns false
        // and sets enter to the distance along the ray (negative value).
        plane.Raycast(new Ray(leftPoints[0], (rightPoints[0] - leftPoints[0]).normalized), out distance);

        normalizedDistance = distance / (rightPoints[0] - leftPoints[0]).magnitude;

        Vector3 newVertex1 = Vector3.Lerp(leftPoints[0], rightPoints[0], normalizedDistance);
        Vector2 newUv1 = Vector2.Lerp(leftUvs[0], rightUvs[0], normalizedDistance);
        Vector3 newNormal1 = Vector3.Lerp(leftNormals[0], rightNormals[0], normalizedDistance);
        Vector4 newTangent1 = Vector3.Lerp(leftTangents[0], rightTangents[0], normalizedDistance);

        cuppingVertices.Add(newVertex1);

        plane.Raycast(new Ray(leftPoints[1], (rightPoints[1] - leftPoints[1]).normalized), out distance);

        normalizedDistance = distance / (rightPoints[1] - leftPoints[1]).magnitude;
        Vector3 newVertex2 = Vector3.Lerp(leftPoints[1], rightPoints[1], normalizedDistance);
        Vector2 newUv2 = Vector2.Lerp(leftUvs[1], rightUvs[1], normalizedDistance);
        Vector3 newNormal2 = Vector3.Lerp(leftNormals[1], rightNormals[1], normalizedDistance);
        Vector4 newTangent2 = Vector3.Lerp(leftTangents[1], rightTangents[1], normalizedDistance);


        cuppingVertices.Add(newVertex2);

        #region First triangle

        Vector3[] finalVerts = new Vector3[] { leftPoints[0], newVertex1, newVertex2 };
        Vector3[] finalNorms = new Vector3[] { leftNormals[0], newNormal1, newNormal2 };
        Vector2[] finalUvs = new Vector2[] { leftUvs[0], newUv1, newUv2 };
        Vector4[] finalTangents = new Vector4[] { leftTangents[0], newTangent1, newTangent2 };

        if (finalVerts[0] != finalVerts[1] && finalVerts[0] != finalVerts[2])
        {
            //Dot product   = prodotto scalare => angolo tra due vettori
            //Cross Product = prodotto vettoriale => normale al piano dei vettori di partenza
            if (Vector3.Dot(Vector3.Cross(finalVerts[1] - finalVerts[0], finalVerts[2] - finalVerts[0]), finalNorms[0]) < 0)
            {
                FlipFace(finalVerts, finalNorms, finalUvs, finalTangents);
            }

            leftSide.AddTriangle(finalVerts, finalNorms, finalUvs, finalTangents, submesh);
        }
        #endregion

        #region Second triangle

        finalVerts = new Vector3[] { leftPoints[0], leftPoints[1], newVertex2 };
        finalNorms = new Vector3[] { leftNormals[0], leftNormals[1], newNormal2 };
        finalUvs = new Vector2[] { leftUvs[0], leftUvs[1], newUv2 };
        finalTangents = new Vector4[] { leftTangents[0], leftTangents[1], newTangent2 };

        if (finalVerts[0] != finalVerts[1] && finalVerts[0] != finalVerts[2])
        {

            if (Vector3.Dot(Vector3.Cross(finalVerts[1] - finalVerts[0], finalVerts[2] - finalVerts[0]), finalNorms[0]) < 0)
            {
                FlipFace(finalVerts, finalNorms, finalUvs, finalTangents);
            }

            leftSide.AddTriangle(finalVerts, finalNorms, finalUvs, finalTangents, submesh);
        }
        #endregion

        #region Third triangle

        finalVerts = new Vector3[] { rightPoints[0], newVertex1, newVertex2 };
        finalNorms = new Vector3[] { rightNormals[0], newNormal1, newNormal2 };
        finalUvs = new Vector2[] { rightUvs[0], newUv1, newUv2 };
        finalTangents = new Vector4[] { rightTangents[0], newTangent1, newTangent2 };

        if (finalVerts[0] != finalVerts[1] && finalVerts[0] != finalVerts[2])
        {

            if (Vector3.Dot(Vector3.Cross(finalVerts[1] - finalVerts[0], finalVerts[2] - finalVerts[0]), finalNorms[0]) < 0)
            {
                FlipFace(finalVerts, finalNorms, finalUvs, finalTangents);
            }

            rightSide.AddTriangle(finalVerts, finalNorms, finalUvs, finalTangents, submesh);
        }

        #endregion

        #region Fourth triangle

        finalVerts = new Vector3[] { rightPoints[0], rightPoints[1], newVertex2 };
        finalNorms = new Vector3[] { rightNormals[0], rightNormals[1], newNormal2 };
        finalUvs = new Vector2[] { rightUvs[0], rightUvs[1], newUv2 };
        finalTangents = new Vector4[] { rightTangents[0], rightTangents[1], newTangent2 };

        if (finalVerts[0] != finalVerts[1] && finalVerts[0] != finalVerts[2])
        {

            if (Vector3.Dot(Vector3.Cross(finalVerts[1] - finalVerts[0], finalVerts[2] - finalVerts[0]), finalNorms[0]) < 0)
            {
                FlipFace(finalVerts, finalNorms, finalUvs, finalTangents);
            }

            rightSide.AddTriangle(finalVerts, finalNorms, finalUvs, finalTangents, submesh);
        }
        #endregion
    }

    private static void FlipFace(Vector3[] verts, Vector3[] norms, Vector2[] uvs, Vector4[] tangents)
    {
        //Switch
        Vector3 temp = verts[2];
        verts[2] = verts[0];
        verts[0] = temp;

        temp = norms[2];
        norms[2] = norms[0];
        norms[0] = temp;

        Vector2 temp2 = uvs[2];
        uvs[2] = uvs[0];
        uvs[0] = temp2;

        Vector4 temp3 = tangents[2];
        tangents[2] = tangents[0];
        tangents[0] = temp3;

    }

    static void Capping()
    {
        capVertTracker.Clear();

        for (int i = 0; i < cuppingVertices.Count; i++)
        {
            if (!capVertTracker.Contains(cuppingVertices[i]))
            {
                capVertpolygon.Clear();
                capVertpolygon.Add(cuppingVertices[i]);
                capVertpolygon.Add(cuppingVertices[i + 1]);

                capVertTracker.Add(cuppingVertices[i]);
                capVertTracker.Add(cuppingVertices[i + 1]);

                bool isDone = false;

                while (!isDone)
                {
                    isDone = true;

                    for (int k = 0; k < cuppingVertices.Count; k += 2)
                    {
                        // go through the pairs
                        if (cuppingVertices[k] == capVertpolygon[capVertpolygon.Count - 1] && !capVertTracker.Contains(cuppingVertices[k + 1]))
                        {
                            // if so add the other
                            isDone = false;
                            capVertpolygon.Add(cuppingVertices[k + 1]);
                            capVertTracker.Add(cuppingVertices[k + 1]);

                        }
                        else if (cuppingVertices[k + 1] == capVertpolygon[capVertpolygon.Count - 1] && !capVertTracker.Contains(cuppingVertices[k]))
                        {
                            // if so add the other
                            isDone = false;
                            capVertpolygon.Add(cuppingVertices[k]);
                            capVertTracker.Add(cuppingVertices[k]);
                        }
                    }
                }
                FillCap(capVertpolygon);
            }
        }

    }

    static void FillCap(List<Vector3> vertices)
    {
        // center of the cap
        Vector3 center = Vector3.zero;
        foreach (Vector3 point in vertices)
        {
            center += point;
        }

        //Get the center
        center /= vertices.Count;

        // you need an axis based on the cap
        Vector3 upward = Vector3.zero;
        // 90 degree turn
        upward.x = plane.normal.y;
        upward.y = -plane.normal.x;
        upward.z = plane.normal.z;
        Vector3 left = Vector3.Cross(plane.normal, upward);

        Vector3 displacement = Vector3.zero;
        Vector3 newUV1 = Vector3.zero;
        Vector3 newUV2 = Vector3.zero;

        for (int i = 0; i < vertices.Count; i++)
        {
            displacement = vertices[i] - center;
            newUV1.x = 0.5f + Vector3.Dot(displacement, left);
            newUV1.y = 0.5f + Vector3.Dot(displacement, upward);

            displacement = vertices[(i + 1) % vertices.Count] - center;
            newUV2.x = 0.5f + Vector3.Dot(displacement, left);
            newUV2.y = 0.5f + Vector3.Dot(displacement, upward);

            Vector3[] final_verts = new Vector3[] { vertices[i], vertices[(i + 1) % vertices.Count], center };
            Vector3[] final_norms = new Vector3[] { -plane.normal, -plane.normal, -plane.normal };
            Vector2[] final_uvs = new Vector2[] { newUV1, newUV2, new Vector2(0.5f, 0.5f) };
            Vector4[] final_tangents = new Vector4[] { Vector4.zero, Vector4.zero, Vector4.zero };

            if (Vector3.Dot(Vector3.Cross(final_verts[1] - final_verts[0], final_verts[2] - final_verts[0]), final_norms[0]) < 0)
            {
                FlipFace(final_verts, final_norms, final_uvs, final_tangents);
            }

            leftSide.AddTriangle(final_verts, final_norms, final_uvs, final_tangents, capMatSub);

            final_norms = new Vector3[] { plane.normal, plane.normal, plane.normal };

            if (Vector3.Dot(Vector3.Cross(final_verts[1] - final_verts[0], final_verts[2] - final_verts[0]), final_norms[0]) < 0)
            {
                FlipFace(final_verts, final_norms, final_uvs, final_tangents);
            }

            rightSide.AddTriangle(final_verts, final_norms, final_uvs, final_tangents,
                capMatSub);
        }

    }
}


