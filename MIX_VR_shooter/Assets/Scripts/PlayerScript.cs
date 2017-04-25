using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

   public GameObject NetworkManager;

   private NetworkHandler _networkHandler;
   private int _hp = 100;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

   public void TakeDamage(int damage)
   {
      print("Player hit");
      _hp -= damage;
      if (_hp <= 0)
      {
         print("Player dead");
         _networkHandler.SendMessage(null, string.Empty);
      }
   }
}
