using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//A basic interactable Object Script -- Placing this on anything will allow it to be highlighted when it can be picked up, and to be picked up.
[RequireComponent(typeof(Rigidbody))]
public class InteractableObject : MonoBehaviour
{


    public Material highlightMaterial;
    protected Material pastMaterial;

    protected new Renderer renderer;

    protected MeshRenderer mesh;

    public bool isHeld = false; //used to check by other scripts

    protected Material[] defaultMaterials;
    protected Material[] highlightMaterials;


    public float highlightForgetThreshold = 0.01F;
    private float timeSinceHighlight;

    protected virtual void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        pastMaterial = mesh.material;
        defaultMaterials =  (Material[]) mesh.materials.Clone();
        highlightMaterials = (Material[])mesh.materials.Clone();
        Array.Resize(ref highlightMaterials, highlightMaterials.Length + 1);
        highlightMaterials[highlightMaterials.Length - 1] = highlightMaterial;

    }

    protected virtual void Update()
    {
        if (timeSinceHighlight > highlightForgetThreshold)
            OnHoverExit();
        timeSinceHighlight += Time.deltaTime;
    }

    public virtual void OnHover()
    {
        mesh.materials = highlightMaterials;
        timeSinceHighlight = 0;

    }

    public virtual void OnHoverExit()
    {
        mesh.materials = defaultMaterials;

    }

  


    public virtual void OnPickUp()
    {
        isHeld = true;
    }

    public virtual void OnDropped()
    {
        isHeld = false;
    }
}
