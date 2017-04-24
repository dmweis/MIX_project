using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballScript : MonoBehaviour {

    public float forceItIsThrown;
    public float destroyInXsec;
    Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        //force = 200;
        rb.AddRelativeForce(new Vector3(0, 0, forceItIsThrown));
        Destroy(this.gameObject, destroyInXsec);
	}
	
	// Update is called once per frame
	void Update () {
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy" | other.gameObject.tag == "Player")
        {
            Destroy(this.gameObject);
        }
    }
}
