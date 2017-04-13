using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DomeScript : MonoBehaviour
{

   public Transform Anchor;
   public Material HitMaterial;
   private Material _originalMaterial;
   private float _lastDetect = 0f;
   private bool _Engaged = false;
   private const float CollisionTimeout = 0.1f;

   // Use this for initialization
   void Start ()
	{
	   _originalMaterial = GetComponent<Renderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
	   if (Time.time - _lastDetect > CollisionTimeout)
	   {
	      // not hit
	      if (_Engaged)
	      {
	         _Engaged = false;
	         GetComponent<Renderer>().material = _originalMaterial;
	      }
	   }
	   else
	   {
	      // is engaged
	      if (!_Engaged)
	      {
	         _Engaged = true;
	         GetComponent<Renderer>().material = HitMaterial;
	      }
	   }
	}

   void OnTriggerStay(Collider other)
   {
      if (other.gameObject.name == "TeleporterBeam")
      {
         _lastDetect = Time.time;
      }
   }
}
