using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

[Category("Components")]
public class TestVoronoi
{
    GameObject newGameObject;

    [SetUp]
    public void SetUpGameObject()
    {
        newGameObject = new GameObject();
        newGameObject.transform.position = Vector3.zero;
    }

    [TearDown]
    public void TeardDownObject()
    {
        GameObject.Destroy(newGameObject);
    }

    [Test]
    public void TestNumberOfPieces()
    {
        MeshSlicing slicer = newGameObject.AddComponent<MeshSlicing>();
        if (Input.GetKeyDown(KeyCode.A))
        {
            int n = Object.FindObjectsOfType<Tester>().Length;
            Assert.That(n, Is.EqualTo(slicer.numberOfCut));
        }

    }


    [Test]
    public void TestCreationMesh()
    {
        MeshSlicing slicer = newGameObject.AddComponent<MeshSlicing>();
        if (Input.GetKeyDown(KeyCode.A))
        {
            int n = Object.FindObjectsOfType<Tester>().Length;
            Assert.That(n, Is.GreaterThan(0));
        }
    }


    [Test]
    public void TestVelocityNonRef()
    {
        MeshSlicing slicer = newGameObject.AddComponent<MeshSlicing>();

        if (Input.GetKeyDown(KeyCode.A))
        {
            int n = Object.FindObjectsOfType<Tester>().Length;
            float time = Time.deltaTime * 0.2f;
            //Bho
            if (time > Time.deltaTime * 0.21f)
                Assert.That(n, Is.GreaterThan(0));
        }
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator TestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;
    }
}
