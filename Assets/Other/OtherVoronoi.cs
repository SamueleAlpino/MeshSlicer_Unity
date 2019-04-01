using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Three
{
    public Mesh Root { get { return root; } }

    public Mesh LeftNode { get { return leftNode; } }
    public Mesh RightNode { get { return rightNode; } }

    private Mesh root;
    private Mesh leftNode;
    private Mesh rightNode;

    public Three(Mesh root, Mesh left, Mesh right)
    {
        this.root = root;
        this.leftNode = left;
        this.rightNode = right;
    }
}

public class OtherVoronoi : MonoBehaviour
{
    public bool SkinnedMesh;
    public Vector3 ImpactPosition;
    public int numberOfPieces;

    private List<Mesh> meshes = new List<Mesh>();
    private Mesh mesh;
    private Material[] materials = new Material[0];
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private List<CombineInstance> combineInstance;
    private int index;
    private bool oneTime;
    void Start()
    {

        mesh = new Mesh();

        oneTime = true;

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

                Mesh mesh = new Mesh
                {
                    vertices = newVerts,
                    normals  = newNormals,
                    uv       = newUvs,

                    triangles = new int[]
                    { 0, 1, 2,
                      2, 1, 0,
                      3, 4, 5,
                      5, 4, 3}
                };

                meshes.Add(mesh);



            }


            //prendere tutte le mesh e unirle in poche mesh e instanziarle, usando il combineMesh

            for (int i = 0; i < numberOfPieces; i++)
            {
                for (int j = 0; j < meshes.Count; j++)
                {
                    if (oneTime)
                    {
                        combineInstance = new List<CombineInstance>();
                        oneTime = false;
                    }
                    
                    CombineInstance newComb = new CombineInstance();
                    combineInstance.Add(newComb);

                    combineInstance.ToArray()[j].mesh = meshes[j];

                    if (j % numberOfPieces == 0)
                    {
                        Mesh toInsta = new Mesh();
                        toInsta.CombineMeshes(combineInstance.ToArray());

                        GameObject GO = new GameObject("Mesh " + (j / numberOfPieces));
                        //   GO.layer = LayerMask.NameToLayer("Particle");
                        GO.transform.position = transform.position;
                        GO.transform.rotation = transform.rotation;
                        GO.AddComponent<MeshRenderer>().material = materials[submesh];
                        GO.AddComponent<MeshFilter>().mesh = toInsta;

                        GO.AddComponent<BoxCollider>();

                        if (Mathf.Approximately(ImpactPosition.x, 0f) && Mathf.Approximately(ImpactPosition.y, 0f) && Mathf.Approximately(ImpactPosition.z, 0f))
                            ImpactPosition = new Vector3(transform.position.x + Random.Range(-0.5f, 0.5f), transform.position.y + Random.Range(0f, 0.5f), transform.position.z + Random.Range(-0.5f, 0.5f));

                        GO.AddComponent<Rigidbody>().AddExplosionForce(Random.Range(500, 800), ImpactPosition, 5);
                        Destroy(GO, 5 + Random.Range(0.0f, 5.0f));

                        
                    }

                    index++;
                }


            }

            //  GameObject GO = new GameObject("Square " + (i / 6));
            //  GO.layer = LayerMask.NameToLayer("Particle");
            //  GO.transform.position = transform.position;
            //  GO.transform.rotation = transform.rotation;
            //  GO.AddComponent<MeshRenderer>().material = materials[submesh];
            //  GO.AddComponent<MeshFilter>().mesh = mesh;
            //
            //   GO.AddComponent<BoxCollider>();


            //  if (Mathf.Approximately(ImpactPosition.x, 0f) && Mathf.Approximately(ImpactPosition.y, 0f) && Mathf.Approximately(ImpactPosition.z, 0f))
            //      ImpactPosition = new Vector3(transform.position.x + Random.Range(-0.5f, 0.5f), transform.position.y + Random.Range(0f, 0.5f), transform.position.z + Random.Range(-0.5f, 0.5f));
            //
            //  GO.AddComponent<Rigidbody>().AddExplosionForce(Random.Range(500, 800), ImpactPosition, 5);
            //  Destroy(GO, 5 + Random.Range(0.0f, 5.0f));

        }



        yield return new WaitForSeconds(0.1f);


        GetComponent<Renderer>().enabled = false;

        Destroy(gameObject);

    }
}
