using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FracturableObject : MonoBehaviour
{
    [SerializeField]
    private GameObject fracturedModel;

    private Rigidbody[] rigidbodies;

    public void DoFracturing(float impactForce, Vector3 impactPosition, float explosionRadius)
    {
        GameObject toInsta = Instantiate(fracturedModel, this.transform.position, this.transform.rotation);

        rigidbodies = toInsta.GetComponentsInChildren<Rigidbody>();

        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].AddExplosionForce(impactForce,impactPosition, explosionRadius);
        }

        Destroy(this.gameObject);
    }

}
