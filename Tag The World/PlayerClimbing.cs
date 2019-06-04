using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PullPlayer : MonoBehaviour
{
    [Header("General")]
    public Rigidbody playerBody; //Reference to Player's Body / Camera Rig
    public GameObject playerHead; //Reference to Player's Head

    [Header("Climbing")]
    public S_PullPlayer otherHand; //Reference to the other hand, used to check if physics shjould be applied to the player
    public float SwingForce = 1F; //Force used to swing the player when they release
    public SteamVR_Action_Boolean climbAction; //The action that is used for climbing

    private bool canGrip; //If the controller is in the area of a climbable surface
    [HideInInspector]
    public bool isGripped; //Used for communicating between the controllers
    [HideInInspector]
    public Vector3 previousPos; //Where the controller was in the last physics tick, used to calculate how far to move the player


    private SteamVR_Behaviour_Pose controller; //Reference to the controller that this is attached to


    void Awake()
    {
        controller = GetComponent<SteamVR_Behaviour_Pose>(); //Sets up variables, such as controllers and actions
        previousPos = transform.localPosition;
        climbAction = SteamVR_Actions.default_InteractUI;
    }

    private void OnTriggerEnter(Collider other) //When we are in range of a climbable surface
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Climbable"))
            canGrip = true;
    }

    private void OnTriggerExit(Collider other) //When we leave the surface
    {
        canGrip = false;
    }


    void FixedUpdate()
    {
      
        if (canGrip && climbAction.GetState(controller.inputSource)) //If the climb action is used and we can hold a surface
        {
            playerBody.transform.position += (previousPos - transform.localPosition); //Move the player in relation to the position that the controller is
            playerBody.useGravity = false; //Disable phsics on the player
            playerBody.isKinematic = true;
            isGripped = true;
        }
        else if (canGrip && climbAction.GetStateUp(controller.inputSource)) //Swinging / Leaping to another point
        {
            ApplyPhysics();
            playerBody.velocity = (previousPos - transform.localPosition) / Time.deltaTime * SwingForce; //Add velocity to the player
            isGripped = false;
        }
        else //If they are not currently grabbing anything, apply physics to the player
        {
            ApplyPhysics();
            isGripped = false;
        }
        previousPos = controller.transform.localPosition;
    }

    private void ApplyPhysics() //If they are still holding on with the other hand, we shouldn't be applying physics to them
    {
        if (!otherHand.isGripped)
        {
            playerBody.useGravity = true;
            playerBody.isKinematic = false;
        }
    }
}
