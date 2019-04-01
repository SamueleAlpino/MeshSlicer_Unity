using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryThreePartitioning
{
    private Mesh root;

    private BinaryThreePartitioning left;
    private BinaryThreePartitioning right;

    public BinaryThreePartitioning(Mesh[] meshes) : this(meshes, 0) { }

    BinaryThreePartitioning(Mesh[] allMeshes, int index)
    {
        Load(this, allMeshes, index);
    }


    void Load(BinaryThreePartitioning tree, Mesh[] currentRoot, int index)
    {
        this.root = currentRoot[index];

        if (index * 2 + 1 < currentRoot.Length)
        {
            this.left = new BinaryThreePartitioning(currentRoot, index * 2 + 1);
        }
        if (index * 2 + 2 < currentRoot.Length)
        {
            this.right = new BinaryThreePartitioning(currentRoot, index * 2 + 2);
        }
    }

}

[RequireComponent(typeof(Rigidbody))]
public class Voronoi : MonoBehaviour
{
    private Rigidbody myRigidbody;
    
    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }
    void BuildMeshTree(Mesh sourceMesh, int numberOfFragments)
    {
        //Instantiate the mesh tree and assign the source
        Mesh[] array = new Mesh[numberOfFragments];
        array[0]     = sourceMesh;
        BinaryThreePartitioning three = new BinaryThreePartitioning(array);
        //mesh in the root;
        SplitNode(array[0], numberOfFragments);

    }


   private void SplitNode(Mesh toSplit, int numberOfFragments)
    {
        //  Instantiate random plane that goes through the
        //  center of mass of the mesh in the input mesh
        //  tree node;
        Plane plane = new Plane();
        plane.SetNormalAndPosition(Vector3.one, myRigidbody.centerOfMass);
        
       // Mesh[] slicedResultMeshes = SliceMesh(plane , toSplit);
        //Create child node to the input node for each
        //slice result mesh ;

     //   for (int i = 0; i < slicedResultMeshes.Length; i++)
     //   {
     //    //   if (i < numberOfFragments)
     //           //SplitNode(childNode);
     //   }
    }

   // private  Mesh[] SliceMesh(Plane plane , Mesh toSlice )
    //  {
        //I triangoli che non intersecano il piano sono organizzati
        //in base a quale lato del piano si trovano;
        //I triangoli che intersecano con il piano sono divisi in
        //tre triangoli e sono organizzati in base a quale lato del piano 
        //si trovano;
      //  MeshBuilder builder = new MeshBuilder();
      //
      //  for (int i = 0; i < toSlice.vertexCount ; i++)
      //  {
      //      Vector3 toCheck = toSlice.vertices[i];
      //
      //      if (plane.GetSide(toCheck))
      //          builder.PositiveSize.Add(toCheck);
      //      else
      //          builder.NegativeSize.Add(toCheck);
      //  }
      
      //    Faces are generated to cover gaps;
      //    In case the slice results in more than two hulls,
      //    those hulls are separated so that they are
      //    each contained in their own mesh;
       //   return sliceResultMeshes;
//      }
}
public class MeshBuilder
{
    public List<Vector3> NegativeSize = new List<Vector3>();
    public List<Vector3> PositiveSize = new List<Vector3>();
}
