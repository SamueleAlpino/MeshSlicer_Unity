using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDelaunay : MonoBehaviour
{
    public GameObject Prefab;

    private Mesh mesh;

    private MeshConstructor positive;
    private MeshConstructor negative;

    private List<Vector3> toSplit;
    // Use this for initialization
    void Start()
    {
        mesh = Prefab.GetComponent<MeshFilter>().mesh;
        negative = new MeshConstructor();
        positive = new MeshConstructor();

    }

    // Update is called once per frame
    void Update()
    {
    }

    private void CheckTheVertexPosition()
    {
        Plane plane = new Plane(Vector3.up, Prefab.transform.position + mesh.bounds.center);

        for (int i = 0; i < mesh.vertices.Length; i += 3)
        {
            if (!plane.GetSide(mesh.vertices[i]) && !plane.GetSide(mesh.vertices[i + 1]) && !plane.GetSide(mesh.vertices[i + 2]))
            {
                //If are all on negative
                negative.vertex.Add(mesh.vertices[i]);
                negative.vertex.Add(mesh.vertices[i + 1]);
                negative.vertex.Add(mesh.vertices[i + 2]);
            }
            else if (plane.GetSide(mesh.vertices[i]) && plane.GetSide(mesh.vertices[i + 1]) && plane.GetSide(mesh.vertices[i + 2]))
            {
                //If are all on positive
                positive.vertex.Add(mesh.vertices[i]);
                positive.vertex.Add(mesh.vertices[i + 1]);
                positive.vertex.Add(mesh.vertices[i + 2]);
            }
            else
            {
                //If the triangle need to be split
                toSplit.Add(mesh.vertices[i]);
                toSplit.Add(mesh.vertices[i + 1]);
                toSplit.Add(mesh.vertices[i + 2]);
            }
        }

    }

    private void SplitTriangle()
    {

    }

    void SplitMesh()
    {
        CheckTheVertexPosition();
          // else if (plane.GetSide(mesh.vertices[i]) && !plane.GetSide(mesh.vertices[i + 1]) && !plane.GetSide(mesh.vertices[i + 2]))
          //     positive.vertex.Add(mesh.vertices[i]);
          // else if (!plane.GetSide(mesh.vertices[i]) && plane.GetSide(mesh.vertices[i + 1]) && !plane.GetSide(mesh.vertices[i + 2]))
          //     positive.vertex.Add(mesh.vertices[i + 1]);
          // else if (!plane.GetSide(mesh.vertices[i]) && !plane.GetSide(mesh.vertices[i + 1]) && plane.GetSide(mesh.vertices[i + 2]))
          //     positive.vertex.Add(mesh.vertices[i + 2]);
          // else if (!plane.GetSide(mesh.vertices[i]) && plane.GetSide(mesh.vertices[i + 1]) && plane.GetSide(mesh.vertices[i + 2]))
          //     negative.vertex.Add(mesh.vertices[i]);
          // else if (plane.GetSide(mesh.vertices[i]) && !plane.GetSide(mesh.vertices[i + 1]) && plane.GetSide(mesh.vertices[i + 2]))
          //     negative.vertex.Add(mesh.vertices[i + 1]);
          // else if (plane.GetSide(mesh.vertices[i]) && plane.GetSide(mesh.vertices[i + 1]) && !plane.GetSide(mesh.vertices[i + 2]))
          //     negative.vertex.Add(mesh.vertices[i + 2]);
    }
}
public class MeshConstructor
{
    public List<Vector3> border;
    public List<Vector3> vertex;
}
