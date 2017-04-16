using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerScript : MonoBehaviour
{

   public GameObject Bullet;
   public GameObject TeleporterBeam;

   private SteamVR_TrackedObject trackedObject;
   private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObject.index); } }
   private float _timeOfLastShot;

   // Use this for initialization
   void Start()
   {
      TeleporterBeam.SetActive(false);
      trackedObject = GetComponent<SteamVR_TrackedObject>();
      _timeOfLastShot = Time.time;
   }

   // Update is called once per frame
   void FixedUpdate()
   {
      if (Time.time - _timeOfLastShot > 0.150f && controller.GetHairTrigger())
      {
         _timeOfLastShot = Time.time;
         print("Triggered");
         GameObject newBullet = Instantiate(Bullet, transform.position, Quaternion.identity);
         newBullet.GetComponent<Rigidbody>().AddForce(Quaternion.AngleAxis(-30, transform.right) * transform.up * -100, ForceMode.VelocityChange);
         Destroy(newBullet, 6);
      }
   }

   void Update()
   {
      if (controller.GetPressDown(EVRButtonId.k_EButton_Grip))
      {
         TeleporterBeam.SetActive(true);
      }
      else if (controller.GetPressUp(EVRButtonId.k_EButton_Grip))
      {
         TeleporterBeam.SetActive(false);
      }
   }
}
