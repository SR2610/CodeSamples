using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogPart : InteractableObject
{

    public DogPart partType = new DogPart();
    public GameObject partPrefab;

    public float attachRadius = 0.2F;

    public bool removedFromBox = false;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void OnHover()
    {
        base.OnHover();
    }

    public override void OnHoverExit()
    {
        base.OnHoverExit();
    }


    public override void OnPickUp()
    {
        base.OnPickUp();
        if (gameObject.layer != 15 || gameObject.layer != 16)
            gameObject.layer = 15;
    }

    public override void OnDropped()
    {
        base.OnDropped();
        AttemptToAttach();
    }

    private void AttemptToAttach() //Tries to attach a part when it is let go of
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, attachRadius * transform.localScale.x); //Checks colliders in the attach radius (default 0.2f) of the part when it is let go of, no more janky relying on Physics collisions
        foreach (Collider col in cols)
        {
            if (col.gameObject.GetComponent<S_DogBody>() && col.GetComponent<S_DogBody>().isHeld && removedFromBox) //Checks that its found a body that is being held
            {
                //Attach the dog part
                CreateJoint(col.GetComponent<Rigidbody>());
                gameObject.layer = 16;
                GetComponent<Rigidbody>().detectCollisions = false; //Disables the attached parts collisions with everything, this is a bit annoying but it can probably be figured out at some point.
                GetComponent<Rigidbody>().mass = 0;
                transform.parent = col.transform.parent;
                col.GetComponent<S_DogBody>().OnPartAttached(partType, gameObject);
                GameObject.Find("GameManager").GetComponent<AudioManager>().PlaySFX("Attach", transform);
            }
        }
    }



    //Creates a joint between the part that is being attached and the body, this doesn't really do much at the moment and is more experimental
    private void CreateJoint(Rigidbody toConnectTo)
    {
        gameObject.AddComponent<FixedJoint>().connectedBody = toConnectTo;
    }
}
