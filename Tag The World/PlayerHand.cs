using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerHand : MonoBehaviour
{

    public SteamVR_Action_Boolean grab; //The action used to grab the item

    private SteamVR_Behaviour_Pose controller; //Reference to the controller that this is attached to
    private FixedJoint joint; //The fixed joint holding the object to the hand
    private S_InteractableObject heldObject; //Currently held object
    private List<S_InteractableObject> nearbyObjects = new List<S_InteractableObject>(); //A list of the nearby objects that are in the controller's trigger


    void Awake()
    {
        controller = GetComponent<SteamVR_Behaviour_Pose>(); //Get the reference to the controller
        joint = GetComponent<FixedJoint>(); //Gets reference to the joint on the controller
    }

    void Update()
    {
        if (grab.GetLastStateDown(controller.inputSource)) //If the grab action is used, try to pickup the closest object
        {
            PickupItem();
        }
        if (grab.GetLastStateUp(controller.inputSource)) //If it is released, drop the held object
        {
            DropItem();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.GetComponent<S_InteractableObject>()) //If it is an interactable add it to the list when it is in our trigger
            return;
        nearbyObjects.Add(other.GetComponent<S_InteractableObject>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.GetComponent<S_InteractableObject>()) //If it leaves the trigger and is an interactable, remove it from the list
            return;
        nearbyObjects.Remove(other.GetComponent<S_InteractableObject>());
    }

    public void PickupItem() //Attempts to pick up the closest item
    {
        heldObject = GetClosestInteractable(); //Gets the closest item
        if (!heldObject) //If there is not an item in the trigger, abort
            return;
        if (heldObject.heldHand) //If the object that we are trying to pickup is already held, drop it first
            heldObject.heldHand.DropItem();

        heldObject.transform.position = transform.position; //Set the object to the position of the controller

        Rigidbody targetBody = heldObject.GetComponent<Rigidbody>(); //Attach the object to the joint
        joint.connectedBody = targetBody;

        heldObject.heldHand = this; //Set the object's held hand

    }

    public void DropItem()
    {
        if (!heldObject) //If we are not holding an object, we don't need to drop it
            return;

        Rigidbody targetBody = heldObject.GetComponent<Rigidbody>(); //Apply the controller's current velocity to the object
        targetBody.velocity = controller.GetVelocity();
        targetBody.angularVelocity = controller.GetAngularVelocity();

        joint.connectedBody = null; //Reset variables
        heldObject.heldHand = null;
        heldObject = null;

    }

    private S_InteractableObject GetClosestInteractable() //Finds the closest interactable to the player 
    {
        S_InteractableObject closest = null;
        float minDistance = float.MaxValue; //The distance of the closest interactable
        float distance = 0; //Distance variable for checking if closer

        foreach (S_InteractableObject interactable in nearbyObjects)
        {
            distance = (interactable.transform.position - transform.position).sqrMagnitude; //Imperfect but faster check for distance
            if (distance < minDistance) //If its closer than the closest, it takes the title
            {
                closest = interactable; //Sets the closest interactable and its distance
                minDistance = distance;
            }
        }

        return closest;
    }
}
