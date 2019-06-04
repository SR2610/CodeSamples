﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class PuzzleSimon : PuzzleBase
{
    public enum SimonColours //Button Colours
    {
        RED = 0,
        BLUE = 1,
        GREEN = 2,
        YELLOW = 3
    }

    public List<SimonButton> buttons = new List<SimonButton>();

    public List<SimonColours> puzzleColours = new List<SimonColours>(); //List of colours generated by the puzzle
    public List<SimonColours> enteredColours = new List<SimonColours>(); //List of colours entered by the player

    public int puzzleLength = 8;  //The number of colours for the puzzle

    private bool displayingColours = false;
    private float timeSinceLastDisplayed;
    public float timeBetweenDisplays = 5F;
    private float timeSincePressed = 0;
    private int nextShown;
    public AudioClip errorSound;

    /*Used for the buttons of the simon game, takes an in (0-3) and converts it to a colour.*/
    public void SimonButtonPressed(int colour)
    {
        if (timeSincePressed < .8F)
            return;
        displayingColours = false;
        SimonColours pressed = (SimonColours)colour; //Convert to the colour from an int
        enteredColours.Add(pressed);
        timeSincePressed = 0;
        if (enteredColours[enteredColours.Count - 1] == puzzleColours[enteredColours.Count - 1]) //Correct button pressed, move on
        {
            if (enteredColours.Count == puzzleColours.Count)//If we are now at the end of the sequence
            {
                if (puzzleColours.Count == puzzleLength) //If we have completed the puzzle, call the solve function
                {
                   // AudioManager.instance.PlaySFX(victorySound, transform, AudioManager.AudioChannel.SFX, false);
                    SolvePuzzle();
                }
                else
                {
                    puzzleColours.Add((SimonColours)Random.Range(0, 4)); //Add the next colour in the sequence
                    enteredColours.Clear();
                    DisplayPuzzleColours(); //Show the colours of the puzzle
                }
            }
            else //Not at the end, puzzle needs to be continued
            {
                //Do nothing
            }

        }
        else //Wrong button pressed, restart the puzzle
        {
            AudioManager.instance.PlaySFX(errorSound, transform, AudioManager.AudioChannel.SFX, false);
            GeneratePuzzle(0); //Regenerate the puzzle
        }



    }

    private void Update()
    {
        if (puzzleActive)
        {

            if (!displayingColours && enteredColours.Count == 0) //If we are not displaying the colours currently and they are not solving it, add onto time since displayed
                timeSinceLastDisplayed += Time.deltaTime;
            if (timeSinceLastDisplayed >= timeBetweenDisplays) //If its been a while, show the sequence
                DisplayPuzzleColours();

            if (timeSincePressed < 1.5F)
                timeSincePressed += Time.deltaTime;
        }
        else
            displayingColours = false;
    }


    private void DisplayPuzzleColours() //Display the colours in the current puzzle
    {
        displayingColours = true;
        timeSinceLastDisplayed = 0;
        nextShown = 0;
        StartCoroutine("FlashButton");
    }


    private IEnumerator FlashButton()
    {
        yield return new WaitForSeconds(0.8F);
        if (displayingColours) //Check if we can still show sequence colours
        {
            if (puzzleColours.Count == nextShown)
            {
                displayingColours = false;
            }
            else
            {
                buttons[(int)puzzleColours[nextShown]].FlashButton(0.5F);//Display next colour in sequence
                StartCoroutine("FlashButton");

            }
        }
        nextShown++;

    }

    private IEnumerator CompletePuzzle() //Flasehes all the lights to signify puzzle completed
    {
        yield return new WaitForSeconds(1F);
        buttons[0].FlashButton(.3F);
        yield return new WaitForSeconds(0.4F);
        buttons[1].FlashButton(.3F);
        yield return new WaitForSeconds(0.4F);
        buttons[2].FlashButton(.3F);
        yield return new WaitForSeconds(0.4F);
        buttons[3].FlashButton(.3F);
        yield return new WaitForSeconds(0.4F);



    }


    public override void GeneratePuzzle(int difficulty) //Generates the start of the puzzle, the reset is generated as solved
    {
        base.GeneratePuzzle(difficulty);

        puzzleColours.Clear();
        enteredColours.Clear();
        puzzleColours.Add((SimonColours)Random.Range(0, 3));

    }

    private void Start() //When the game starts, generate the puzzle
    {
        GeneratePuzzle(0);
    }

    public override void ResetPuzzle() //Generate a new puzzle when failed
    {
        GeneratePuzzle(0);
    }

    public override void SolvePuzzle()
    {
        base.SolvePuzzle();
        for(int i =0; i==3; i++)
        {
            buttons[i].GetComponent<Interactable>().enabled = false;
            buttons[i].GetComponent<HoverButton>().enabled = false;
        }
        StartCoroutine("CompletePuzzle");
    }

}