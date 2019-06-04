using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleWires : PuzzleBase
{

    /*
    RULES
    
    If there are no yellow wires and there is a blue light, cut the sixth wire.


    If there is exactly one yellow wire, and more than one white wire, cut the third
    wire.


    If there is exactly one red wire, cut the second wire.


    Otherwise, cut the fifth wire.

     */

    public List<Wire> wires = new List<Wire>(); //The wire components in the puzzle
    public int correctWire;
    public List<Renderer> lights = new List<Renderer>();
    public Material red, blue, green, off;
    public Material yellowWire, whiteWire, redWire;//The actual wires relevent to cutting
    public List<Material> dummyWires = new List<Material>(); //Blue, Green, Black, Purple

    private int strikes = 0;

    [ContextMenu("Generate")]
    public void Gen()
    {
        GeneratePuzzle(0);
    }
    public override void GeneratePuzzle(int difficulty)
    {
        base.GeneratePuzzle(difficulty);

        correctWire = Random.Range(0, 4); //Selects the wire that will need to be cut

        foreach (Renderer r in lights) //Turn all the lights off
            r.material = off;

       foreach(Wire w in wires)
            w.SetColour(dummyWires[Random.Range(0, dummyWires.Count)]);
 
        switch (correctWire) //Sets up the puzzle to be solvable
        {


            case 0:
                SetRandomLights();
                lights[Random.Range(0, 3)].material = blue; //Ensures we get the blue light
                wires[Random.Range(0, 6)].SetColour(whiteWire); //Adds valid red herring wires
                wires[Random.Range(0, 6)].SetColour(whiteWire);

                break;
            case 1:
                List<int> required = new List<int>(); //Set the required wires by the puzzle
                int gen = Random.Range(0, 6);
                wires[gen].SetColour(yellowWire);
                required.Add(gen);
                while (required.Contains(gen))
                    gen = Random.Range(0, 6);
                wires[gen].SetColour(whiteWire);
                required.Add(gen);
                while (required.Contains(gen))
                    gen = Random.Range(0, 6);
                wires[gen].SetColour(whiteWire);
                required.Add(gen);
                gen = Random.Range(0, 6);
                if (!required.Contains(gen))  //Set some other valid wires
                    wires[gen].SetColour(whiteWire);
                gen = Random.Range(0, 6);
                if (!required.Contains(gen))
                    wires[gen].SetColour(whiteWire);
                gen = Random.Range(0, 6);
                if (!required.Contains(gen))
                    wires[gen].SetColour(whiteWire);

                SetRandomLights();
                break;
            case 2:
                wires[Random.Range(0, 6)].SetColour(redWire);
                SetRandomLights();
                break;
            default: //Final combination, just has to not be the other rules
                wires[Random.Range(0, 6)].SetColour(whiteWire);
                SetRandomLights();
                break;

        }
    }


    private void SetRandomLights()
    {
        lights[Random.Range(0, 3)].material = green; //Set some random lights
        lights[Random.Range(0, 3)].material = red; 
    }

    public void WireCut(int wireID)
    {
        if (wireID == correctWire)
            SolvePuzzle();
        else
            strikes++;
        if (strikes >= 2)
            Debug.Log("Alarm Activated");
    }

}
