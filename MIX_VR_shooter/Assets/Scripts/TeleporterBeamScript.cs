﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterBeamScript : MonoBehaviour
{

   private GameObject _dome;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

   void OnDisable()
   {
      _dome = null;
   }

   public Transform GetAnchor()
   {
      if (_dome == null)
      {
            print("No dome selected");
          return null;
      }
      DomeScript domeScript = _dome.GetComponent<DomeScript>();
      if (domeScript == null)
      {
            print("No dome script found on object");
         return null;
      }
      Transform domeAnchor = domeScript.Anchor;
      return domeAnchor;
   }

   void OnTriggerEnter(Collider other)
   {
      if (other.gameObject.tag == "Respawn")
      {
         _dome = other.gameObject;
      }
   }

   void OnTriggerExit(Collider other)
   {
       if (other.gameObject.tag == "Respawn")
       {
           _dome = null;
       }
    }
}
