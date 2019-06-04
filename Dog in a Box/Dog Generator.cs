using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DogGenerator : MonoBehaviour {

    public List<DogPart> generatedDogParts = new List<DogPart>();


    public int normalDogThreshold = 1; //How many standard dogs (1 head, 1 Tail and 4 Legs) should the player make before it gets weird
    private int dogsCompleted = 0;


    public void DogCompleted()
    {
        dogsCompleted++;
    }


   public void GenerateNewDog()
    {
        generatedDogParts.Clear(); //Delete the current dog

        int headAmount = 0, legAmount = 0, tailAmount = 0;


        if (dogsCompleted < normalDogThreshold)
        {
            headAmount = 1;
            legAmount = 4;
            tailAmount = 1;
        }
        else
        {
            int partAmount = Random.Range(5, 15);
            headAmount = Random.Range(0, Mathf.CeilToInt(partAmount / 3));
            legAmount = Random.Range(0, Mathf.CeilToInt((partAmount - headAmount / 2)));
            tailAmount = partAmount - (headAmount + legAmount);
        }

        int breedAmount = Enum.GetNames(typeof(DogPart.Breed)).Length;

        for (int i = 0; i < headAmount; i++)
            CreateDogPart(DogPart.Parts.HEAD, breedAmount);

        for (int i = 0; i < legAmount; i++)
            CreateDogPart(DogPart.Parts.LEG, breedAmount);


        for (int i = 0; i < tailAmount; i++)
            CreateDogPart(DogPart.Parts.TAIL, breedAmount);

        DogCompleted();

    }


    private void CreateDogPart(DogPart.Parts partType, int breeds)
    {
        int chosenBreed = Random.Range(0, breeds);
        DogPart genDogPart = new DogPart(partType, (DogPart.Breed)chosenBreed);
        generatedDogParts.Add(genDogPart);
    }
}
