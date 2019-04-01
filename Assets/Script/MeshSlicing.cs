using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Text;

[RequireComponent(typeof(Rigidbody))]
public class MeshSlicing : MonoBehaviour
{
    //Preallocare gameobject in base al numero di tagli nello start
    public int numberOfCut = 1;
    [SerializeField]
    private bool skinnedMesh;

    
    private MeshThree topThree;
    private GameObject[] pieces;
    private Rigidbody body;
    private List<MeshThree> threeList;
    private GameObject[] preAllocation;
    private float minRange = -360;
    private float maxRange = 360;

    private Stopwatch stopwatch = new Stopwatch();

    private void Awake()
    {
        preAllocation = new GameObject[numberOfCut];
        for (int i = 0; i < preAllocation.Length; i++)
        {
            preAllocation[i] = new GameObject("Pippo");
            preAllocation[i].SetActive(false);
        }
    }

    private void Start()
    {
       
        threeList = new List<MeshThree>();
        body = GetComponent<Rigidbody>();
        //  for (int i = 0; i < preAllocation.Length; i++)
        //  {
        //      if (!preAllocation[i].activeSelf)
        //      {
        //          preAllocation[i].SetActive(true);
        //          pieces = MeshCut.Cut(this.gameObject, body.transform.position, new Vector3(Random.Range(minRange, maxRange), Random.Range(minRange, maxRange), Random.Range(minRange, maxRange)), skinnedMesh, preAllocation[0]);
             //   pieces = MeshCut.Cut(this.gameObject, body.transform.position, body.transform.right, skinnedMesh,preAllocation[i]);
        //          break;
        //      }
        //  }
        //  topThree = new MeshThree(pieces[0], pieces[1]);
        //  threeList.Add(topThree);

    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            stopwatch.Start();

            preAllocation[0].SetActive(true);
            pieces = MeshCut.Cut(this.gameObject, body.transform.position, new Vector3(Random.Range(minRange, maxRange), Random.Range(minRange, maxRange), Random.Range(minRange, maxRange)), skinnedMesh,preAllocation[0]);
            topThree = new MeshThree(this.gameObject, pieces[1]);
            threeList.Add(topThree);

            while (numberOfCut > 1)
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

                        for (int j = 0; j < preAllocation.Length; j++)
                        {
                            if (!preAllocation[j].activeSelf)
                            {
                                preAllocation[j].SetActive(true);
                                pieces = MeshCut.Cut(threeList[i].LeftNode, cutPosition, new Vector3(Random.Range(minRange, maxRange), Random.Range(minRange, maxRange), Random.Range(minRange, maxRange)), skinnedMesh, preAllocation[j]);
                                break;
                            }
                        }

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


                        for (int j = 0; j < preAllocation.Length; j++)
                        {
                            if (!preAllocation[j].activeSelf)
                            {
                                preAllocation[j].SetActive(true);
                                pieces = MeshCut.Cut(threeList[i].RightNode, cutPosition, new Vector3(Random.Range(minRange, maxRange), Random.Range(minRange, maxRange), Random.Range(minRange, maxRange)), skinnedMesh, preAllocation[j]);
                                break;
                            }
                        }

                        MeshThree another = new MeshThree(pieces[0], pieces[1]);
                        threeList.Add(another);
                        threeList[i].IsRightNodeCut = true;
                        break;
                    }
                }
                numberOfCut--;
            }

            stopwatch.Stop();

            //string toWrite = "Benchmark.txt";
            //using (FileStream stream = new FileStream(toWrite, FileMode.OpenOrCreate))
            //{
            //    byte[] data = Encoding.ASCII.GetBytes(stopwatch.ElapsedMilliseconds.ToString());
            //    stream.Seek(0, SeekOrigin.End);
            //    stream.Write(data, 0, data.Length);
            //}

            UnityEngine.Debug.Log(stopwatch.ElapsedMilliseconds);
        }
    }



}


