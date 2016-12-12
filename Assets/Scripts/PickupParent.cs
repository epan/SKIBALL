using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteamVR_TrackedObject))]

public class PickupParent : MonoBehaviour {

    public float throwForce = 2;
    public SteamVR_TrackedObject trackedObj;
    public SteamVR_Controller.Device device;

    public bool objectHeld = false;

    //Used to manually reset a specific ball's position for testing purposes
    //public Transform ball;


    // Use this for initialization
	// sl note: awake happens before start();
    void Awake () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
	
	}
	
	// Update is called once per frame
	void Update () {

        device = SteamVR_Controller.Input((int)trackedObj.index);

        ////Reset ball position for testing
        //if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        //{
        //    ball.transform.position = Vector3.zero;
        //    ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //    ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        //}

    }


	// OnTriggerStay is a Unity function 
    void OnTriggerStay(Collider col)
    {
        //if (col.CompareTag("Throwable"))
        //{
            //Debug.Log("Is colliding with hand");
            if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                //Debug.Log("You are touching down the trigger on an object");
                col.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                col.gameObject.transform.SetParent(gameObject.transform);

                //center held object same location every time
                //col.gameObject.transform.position = gameObject.transform.position;

                objectHeld = true; //is pretty much redundant
            }
            if (objectHeld == true)
            {
                if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
                {
                    Debug.Log("You have released the trigger");

                    col.gameObject.transform.SetParent(null);
                    col.attachedRigidbody.isKinematic = false;

        
                    objectHeld = false;

                    TossObject(col.attachedRigidbody);
                //col.gameObject.GetComponent<Rigidbody>().velocity = device.velocity;
                //col.gameObject.GetComponent<Rigidbody>().angularVelocity = device.angularVelocity;



                }
            }
        //}                     
    }

   //applies force to an object equal to the controller velocity * a multiplier constant
    void TossObject(Rigidbody rigidBody)
    {
 
         rigidBody.velocity = device.velocity * throwForce;
         rigidBody.angularVelocity = device.angularVelocity;
      
    }
}
