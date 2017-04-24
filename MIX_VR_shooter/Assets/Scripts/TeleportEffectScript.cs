using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportEffectScript : MonoBehaviour {

    private ParticleSystem particleEffect;

	// Use this for initialization
	void Start () {
        particleEffect = gameObject.GetComponentInChildren<ParticleSystem>();
        StartCoroutine(Lifecycle());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator Lifecycle()
    {
        yield return new WaitForSeconds(3);
        particleEffect.Stop();
        yield return new WaitForSeconds(2);
        Destroy(this.gameObject);
    }
}
