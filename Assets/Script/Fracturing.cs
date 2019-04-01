using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fracturing : MonoBehaviour
{
    public bool SkinnedMesh;
    public Vector3 ImpactPosition;


    private Mesh mesh;
    private Material[] materials = new Material[0];
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private SkinnedMeshRenderer skinnedMeshRenderer;

    void Start()
    {

        mesh = new Mesh();

        if (GetComponent<Collider>())
        {
            GetComponent<Collider>().enabled = false;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            StartCoroutine(SplitMesh(true));

    }

    public IEnumerator SplitMesh(bool destroy)
    {
        //Setup the mesh
        if (!SkinnedMesh)
        {
            meshFilter = GetComponent<MeshFilter>();
            mesh = meshFilter.mesh;

            meshRenderer = GetComponent<MeshRenderer>();
            materials = meshRenderer.materials;
        }
        else
        {
            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            mesh = skinnedMeshRenderer.sharedMesh;
             
            materials = skinnedMeshRenderer.sharedMaterials;
        }


        Vector3[] verts = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector2[] uvs = mesh.uv;

        int subMeshCount = mesh.subMeshCount;

        for (int submesh = 0; submesh < subMeshCount; submesh++)
        {
            int[] indices = mesh.GetTriangles(submesh);

            for (int i = 0; i < indices.Length; i += 6)
            {
                Vector3[] newVerts = new Vector3[6];
                Vector3[] newNormals = new Vector3[6];
                Vector2[] newUvs = new Vector2[6];

                for (int j = 0; j < 6; j++)
                {
                    int index = indices[i + j];
                    newVerts[j] = verts[index];
                    newUvs[j] = uvs[index];
                    newNormals[j] = normals[index];
                }

                Mesh mesh = new Mesh();
                mesh.vertices = newVerts;
                mesh.normals = newNormals;
                mesh.uv = newUvs;

                mesh.triangles = new int[]
                { 0, 1, 2,
                  2, 1, 0,
                  3, 4, 5,
                  5, 4, 3};

                GameObject GO = new GameObject("Square " + (i / 6));
                GO.layer = LayerMask.NameToLayer("Particle");
                GO.transform.position = transform.position;
             //   GO.transform.localScale = transform.localScale;
                GO.transform.rotation = transform.rotation;
                GO.AddComponent<MeshRenderer>().material = materials[submesh];
                GO.AddComponent<MeshFilter>().mesh = mesh;

              //  if (i % 50 == 0)
                    GO.AddComponent<BoxCollider>();

                if (Mathf.Approximately(ImpactPosition.x, 0f) && Mathf.Approximately(ImpactPosition.y, 0f) && Mathf.Approximately(ImpactPosition.z, 0f))
                    ImpactPosition = new Vector3(transform.position.x + Random.Range(-0.5f, 0.5f), transform.position.y + Random.Range(0f, 0.5f), transform.position.z + Random.Range(-0.5f, 0.5f));

                // If scale == 1 , range(500,800), esplosionRadius = 5;
                //  GO.AddComponent<Rigidbody>().AddExplosionForce(Random.Range(1500, 2000), ImpactPosition, 10000);
                GO.AddComponent<Rigidbody>().AddExplosionForce(Random.Range(400, 600), ImpactPosition, 5);
                Destroy(GO, 5 + Random.Range(0.0f, 5.0f));

            }
        }

        yield return new WaitForSeconds(0.1f);


        GetComponent<Renderer>().enabled = false;

        Destroy(gameObject);

    }

}

