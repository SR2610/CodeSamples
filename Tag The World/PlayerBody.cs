using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerBody : MonoBehaviour
{
    [Header("General")]
    public Rigidbody playerBody; //Reference to Player's Body / Camera Rig
    public GameObject playerHead; //Reference to Player's Head
    public CapsuleCollider playerCollider; //Reference to Player's Collider

    [Header("Hands")]
    public GameObject leftHand; //References to the left and right hands of the player
    public GameObject rightHand;

    [Header("Walking")] //Settings for walking
    public float movementSpeed = 5;
    public float maxSpeed = 5;
    public WalkType currentWalk = WalkType.HEAD;
    private Vector2 movementAxis;
    private SteamVR_Input_Sources currentMovingController;
    private bool wasWalkingLastUpdate = false;
    public SteamVR_Action_Boolean armSwingButton; //Button used to make arm swinging active
    public float swingForceMultiplier = 1.5f;



    [Header("Jumping")]
    public float releaseWindowTime = 1f;
    public float velocityMultiplier = 5.0f;
    public float velocityMax = 8.0f;
    public SteamVR_Action_Boolean jumpAction;
    protected Vector3 leftStartJumpPos;
    protected Vector3 leftReleaseJumpPos;
    protected bool leftIsAiming;
    protected Vector3 rightStartJumpPos;
    protected Vector3 rightReleaseJumpPos;
    protected bool rightIsAiming;
    protected bool leftButtonReleased;
    protected bool rightButtonReleased;
    protected float countDownEndTime;

    public enum WalkType { HEAD, CONTROLLER, ARMSWING };


    public SteamVR_Action_Boolean checkSet;
    public SteamVR_Action_Boolean checkReset;
    Vector3 resetPos;

    private void Start()
    {
        resetPos = transform.position;
        StartCoroutine(playWalk());
    }
    private void Awake()
    {
        leftController = leftHand.GetComponent<SteamVR_Behaviour_Pose>();

        rightController = rightHand.GetComponent<SteamVR_Behaviour_Pose>();
    }

    private void Update()
    {
        playerCollider.height = playerHead.transform.localPosition.y; //Update the collider size to match that of the player
        playerCollider.center = new Vector3(playerHead.transform.localPosition.x, playerHead.transform.localPosition.y / 2, playerHead.transform.localPosition.z); //Adjusts the position of the player's collider to fit their height



    }

    private void FixedUpdate()
    {
        if (!IsClimbing() && IsGrounded()) //If the player isn't gripping and they are on the ground
        {
            PlayerWalk(); //Handle walking
            JumpControls(); //Handle jumping

        }


    }

    private bool IsGrounded() //raycasts to chek if the player is on the ground, cant exactly walk unless you are standing on something
    {
        //Not yet implemented
        return true;
    }

    private bool IsClimbing() //Checks if the player is currently climbing
    {
        return (leftHand.GetComponent<S_PullPlayer>().isGripped || rightHand.GetComponent<S_PullPlayer>().isGripped);
    }

    private void PlayerWalk()
    {
        Vector3 targetVelocity = Vector3.zero;
        if (currentWalk == WalkType.HEAD || currentWalk == WalkType.CONTROLLER) //If they are using Joystick / Touchpad movement
        {
            if (!wasWalkingLastUpdate)
            {
                if (SteamVR_Actions.default_Movement.GetAxis(SteamVR_Input_Sources.LeftHand).sqrMagnitude != 0F)
                    currentMovingController = SteamVR_Input_Sources.LeftHand;
                else if (SteamVR_Actions.default_Movement.GetAxis(SteamVR_Input_Sources.RightHand).sqrMagnitude != 0F)
                    currentMovingController = SteamVR_Input_Sources.RightHand;
                wasWalkingLastUpdate = true;
            }
            movementAxis = SteamVR_Actions.default_Movement.GetAxis(currentMovingController);


            if (movementAxis.sqrMagnitude == 0)
            {
                wasWalkingLastUpdate = false;
            }


            targetVelocity = new Vector3(movementAxis.x, 0, movementAxis.y);  // Calculate how fast we should be moving
            switch (currentWalk)
            {
                case WalkType.HEAD:
                    targetVelocity = playerHead.transform.TransformDirection(targetVelocity);
                    break;
                case WalkType.CONTROLLER:
                    if (currentMovingController == SteamVR_Input_Sources.LeftHand)
                        targetVelocity = leftHand.transform.TransformDirection(targetVelocity);
                    else
                        targetVelocity = rightHand.transform.TransformDirection(targetVelocity);
                    break;
            }
            targetVelocity *= movementSpeed;
            targetVelocity.y = 0;
          
        }
        else if (currentWalk == WalkType.ARMSWING) //If we are using arm swinging
        {
           targetVelocity= ArmSwingMove();
        }
        playerBody.AddForce(targetVelocity, ForceMode.VelocityChange);
        Vector3 velocity = playerBody.velocity;
        Vector3 clampedVelocity;
        clampedVelocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        clampedVelocity.z = Mathf.Clamp(velocity.z, -maxSpeed, maxSpeed);
        clampedVelocity.y = velocity.y;
        playerBody.velocity = clampedVelocity;
    }


    #region Jumping

    private void JumpControls()
    {
        if (jumpAction.GetLastStateDown(SteamVR_Input_Sources.LeftHand)) //Starts tracking where the left hand is
        {
            if (!leftIsAiming && !IsClimbing())
            {
                leftIsAiming = true;
                leftStartJumpPos = transform.InverseTransformPoint(leftHand.transform.position);
            }
        }
        if (jumpAction.GetLastStateUp(SteamVR_Input_Sources.LeftHand)) //Calculkates position of left hand release and checks for jump
        {

            if (leftIsAiming)
            {
                leftReleaseJumpPos = transform.InverseTransformPoint(leftHand.transform.position);
                if (!rightButtonReleased)
                {
                    countDownEndTime = Time.time + releaseWindowTime;
                }
                leftButtonReleased = true;
            }
            CheckForReset();
            CheckIfJump();
        }

        if (jumpAction.GetLastStateDown(SteamVR_Input_Sources.RightHand))
        {
            if (!rightIsAiming && !IsClimbing())
            {
                rightIsAiming = true;
                rightStartJumpPos = transform.InverseTransformPoint(rightHand.transform.position);
            }
        }
        if (jumpAction.GetLastStateUp(SteamVR_Input_Sources.RightHand))
        {

            if (rightIsAiming)
            {
                rightReleaseJumpPos = transform.InverseTransformPoint(rightHand.transform.position);
                if (!leftButtonReleased)
                {
                    countDownEndTime = Time.time + releaseWindowTime;
                }
                rightButtonReleased = true;
            }
            CheckForReset();
            CheckIfJump();
        }
    }

    private void CheckForReset()
    {
        if ((leftButtonReleased || rightButtonReleased) && Time.time > countDownEndTime)
        {
            ResetVariables();
        }
    }

    private void CheckIfJump() //Cehcks if the player has sucsessfully jumped
    {
        if (leftButtonReleased && rightButtonReleased)
        {
            Vector3 leftDir = leftStartJumpPos - leftReleaseJumpPos;
            Vector3 rightDir = rightStartJumpPos - rightReleaseJumpPos;
            Vector3 localJumpDir = leftDir + rightDir;
            Vector3 worldJumpDir = transform.TransformVector(localJumpDir);
            Vector3 jumpVector = worldJumpDir * velocityMultiplier;

            if (jumpVector.magnitude > velocityMax)
            {
                jumpVector = jumpVector.normalized * velocityMax;
            }

            playerBody.AddForce(jumpVector,ForceMode.VelocityChange);
            ResetVariables();

        }
    }

    private void ResetVariables()
    {
        leftIsAiming = false;
        rightIsAiming = false;
        leftButtonReleased = false;
        rightButtonReleased = false;
    }

    #endregion
}
