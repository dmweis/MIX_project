using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerScript : MonoBehaviour
{

   public GameObject Bullet;
   public GameObject Fireball;
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
      //if (Time.time - _timeOfLastShot > 0.150f && controller.GetHairTrigger())
      if (controller.GetHairTriggerDown())
      {
            NewShoot();
      }
   }

    private void OldShoot()
    {
        _timeOfLastShot = Time.time;
        print("Triggered");
        GameObject newBullet = Instantiate(Bullet, transform.position, Quaternion.identity);
        newBullet.GetComponent<Rigidbody>().AddForce(Quaternion.AngleAxis(-30, transform.right) * transform.up * -100, ForceMode.VelocityChange);
        Destroy(newBullet, 6);
    }

    private void NewShoot()
    {
        _timeOfLastShot = Time.time;
        //print("Player shooting");
        GameObject newBullet = Instantiate(Fireball, transform.position, transform.rotation * Quaternion.Euler(Vector3.right * 30));
        newBullet.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    }

   void Update()
   {
      if (controller.GetPressDown(EVRButtonId.k_EButton_Grip))
      {
         TeleporterBeam.SetActive(true);
      }
      else if (controller.GetPressUp(EVRButtonId.k_EButton_Grip))
      {
         Transform newTransform = TeleporterBeam.GetComponent<TeleporterBeamScript>().GetAnchor();
         if (newTransform != null)
         {
            print("Teleporting");
            transform.root.gameObject.transform.position = newTransform.position;
            //transform.root.gameObject.transform.rotation = newTransform.rotation;
            Vector2 rowCol = GameObject.Find("Board").GetComponent<Board>().CalculateRowCol(newTransform.position);
            int row = (int) rowCol.x;
            int col = (int) rowCol.y;
            GameObject.Find("NetworkManager").GetComponent<NetworkHandler>().SendMessage(new LocationUpdate(-1, row, col), "location_update");
         }
         else
         {
            print("No targets");
         }
         TeleporterBeam.SetActive(false);
      }
   }
}
[Serializable]
class LocationUpdate
{
   public int Id;
   public int NewRowLocation;
   public int NewColumnLocation;

   public LocationUpdate(int id, int row, int column)
   {
      Id = id;
      NewRowLocation = row;
      NewColumnLocation = column;
   }
}
