using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DogUtils
{
    public static int GetValueFromDogPart(DogPart dog)
    {
        int partValue = (int)dog.part;
        int breedValue = (int)dog.breed;
        int result = 0;
        result = partValue << 2;
        result = result ^ breedValue;
        return result;

    }



    public static int GetDogScore(int[] dogProvided, List<DogPart> dogToMake, float timeToMakeDog)
    {
        int[] desiredDogCounts = new int[12];
        int[] actualDogCounts = new int[12];

        int timeBonus = (timeToMakeDog <= 10) ? 5000 : (timeToMakeDog <= 15) ? 2500 : 0; // if time less than 10 sec, 5k bonus.  if time less than 15 sec, 2.5k bonus.  greater than 15 = no bonus 


        actualDogCounts = dogProvided;
        desiredDogCounts = GenerateCountFromList(dogToMake);


        int desiredParts = 0;
        int correctParts = 0;
        int incorrectParts = 0;
        int tooManyParts = 0;
        int missingParts = 0;
        int partsAttached = 0;

        for (int i = 0; i < desiredDogCounts.Length; i++)
        {
            desiredParts += desiredDogCounts[i];
            partsAttached += actualDogCounts[i];
            if (desiredDogCounts[i] == 0 && actualDogCounts[i] > 0) //If we didn't want any, then they're ALL WRONG
                incorrectParts += actualDogCounts[i];
            if (desiredDogCounts[i] > 0)
            {
                if (actualDogCounts[i] <= desiredDogCounts[i])
                { //If we have less than or the correct number of parts
                    correctParts += actualDogCounts[i];
                    missingParts += desiredDogCounts[i] - actualDogCounts[i]; //If they haven't given enough
                }
                else
                { //If we have more than the desired amount
                    correctParts += desiredDogCounts[i];
                    tooManyParts += actualDogCounts[i] - desiredDogCounts[i];
                }
            }
        }

        if (partsAttached == 0) //If there is not attached parts, they get a 0* review
            return 0;

        int score = 0;
        int scorePerCorrectPart = 1000;
        int missingPenalty = 250;
        int incorrectPenalty = 500;
        int tooManyPenalty = 100;
        score += (correctParts * scorePerCorrectPart);
        score -= (missingParts * missingPenalty);
        score -= (incorrectParts * incorrectPenalty);
        score -= (tooManyParts * tooManyPenalty);

        Debug.Log("Desired: " + desiredParts);
        Debug.Log("Correct: " + correctParts);
        Debug.Log("Incorrect: " + incorrectParts);
        Debug.Log("TooMany: " + tooManyParts);
        Debug.Log("Missing: " + missingParts);


        int actualTotalScore = score + timeBonus;
        int maxPossScore = desiredParts * scorePerCorrectPart; //If they got every part that would be the highest score

        float ScorePercentage = (float)actualTotalScore / (float)maxPossScore;

        Debug.Log("%: " + ScorePercentage);
        Debug.Log("Actual: " + actualTotalScore);
        Debug.Log("Possible: " + maxPossScore);
        int ScoreOutOf5 = 0;
        if (ScorePercentage <= 0.2f)
            ScoreOutOf5 = 1;
        else if (ScorePercentage <= 0.4f)
            ScoreOutOf5 = 2;
        else if (ScorePercentage <= 0.6f)
            ScoreOutOf5 = 3;
        else if (ScorePercentage <= 0.8f)
            ScoreOutOf5 = 4;
        else if (ScorePercentage <= 1)
            ScoreOutOf5 = 5;

        return ScoreOutOf5;
    }

    public static int[] GenerateCountFromList(List<DogPart> dogToCount)
    {
        int[] countedList = new int[12];

        foreach (DogPart part in dogToCount)
            countedList[GetValueFromDogPart(part)]++;


        return countedList;

    }
}
