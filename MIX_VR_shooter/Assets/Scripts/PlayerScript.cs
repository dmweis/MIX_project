using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

   public GameObject NetworkManager;

   private NetworkHandler _networkHandler;
   private int _hp = 200;

	// Use this for initialization
	void Start () {
        _networkHandler = NetworkManager.GetComponent<NetworkHandler>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

   public void TakeDamage(int damage)
   {
      print("Player hit");
      _hp -= damage;
      foreach (var hpDisplay in transform.root.GetComponentsInChildren<HealthDisplayScript>())
      {
            hpDisplay.SetText(_hp + " HP");      
      }
      _networkHandler.SendMessage(new HealthUpdate(-1, _hp), "health_update");
      if (_hp <= 0)
      {
         print("Player dead");
      }
   }
}
