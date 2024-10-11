using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Randomizer_Script : MonoBehaviour
{
    //all possible paths
    private static string[][] pathArray = {
        new string[] {"012", "014", "452"}, //3 squares
        new string[] {"01254", "01258", "01436", "01476", "01478", "01458", "01452", "45210"}, //5 squares
        new string[] {"0125436", "0125476", "0125478", "0125876", "0125874", "0143678", "0147852", "0145876", "4521036"}, //7 squares
        new string[] {"012543678", "012587634", "012587436", "014367852", "452103678"} //9 squares
    };

    //all possible rotations and flips
    private static string[] pathFilter = {
        "012345678", //no rotation
        "630741852", //90 clockwise
        "258147036", //90 counterclockwise
        "876543210", //180
        "210543876", //horizontal flip
        "678345012", //vertical flip
        "852741630", //90 clockwise, horizontal flip
        "036147258", //90 counterclockwise, horizontal flip
    };

    //generate 1 random path
    public string generateRandomPath(int stepsLowBoundInclusive, int stepsHighBoundInclusive) {
        int pathLengthMinus2 = UnityEngine.Random.Range(stepsLowBoundInclusive, stepsHighBoundInclusive + 1) - 2; //length of path - 3
        int pathIndex = UnityEngine.Random.Range(0, pathArray[pathLengthMinus2].Length); //index of path
        int filterIndex = UnityEngine.Random.Range(0, pathFilter.Length); //index of filter
        string path = "";
        foreach (char c in pathArray[pathLengthMinus2][pathIndex]) {
            //Debug.Log(pathFilter[filterIndex][int.Parse(c.ToString())].ToString());
            path += pathFilter[filterIndex][int.Parse(c.ToString())].ToString();
        }
        return path;
    }
    /*
    private int targetLowBound, targetHighBound; //range of target number
    private int[] numbersLowBound = new int[5], numbersHighBound = new int[5]; //range of number in each square
    private List<string> signs; //all available signs (pick 4 to use)
    private int equationStepsLowBound, equationStepsHighBound; //number of numbers needed to create an equation (not including signs)
    private int stepsLowBound, stepsHighBound; //range of minimum steps needed to complete the question
    private int extraSteps; //number of extra steps given on top of steps needed

    private string[] grid = {"4", "+", "5", "-", "3", "*", "1", "+", "2"};
    private List<string> solution;
    private int totalSteps;
    */
    
    /*
    private bool findPath(ref string path, int pos, int remEquationSteps) {
        if (remEquationSteps == 0) {
            return true;
        }
        path += pos.ToString();

        List<int> directions = new List<int> {3, 1, -1, -3};
        while (directions.Any()) {
            int ind = UnityEngine.Random.Range(0, directions.Count());
            int dir = directions[ind];
            directions.RemoveAt(ind);
            //...
        }
        //fails, backtrack
        path.Remove(path.Length - 1);
        return false;
    }

    //generate a random equation path
    private string generateEquationPath(int steps) {
        string path = "";
        if (!findPath(ref path, UnityEngine.Random.Range(0, 5) * 2, steps)) {
            Debug.Log("Unable to generate a maze!");
            return "failed";
        }
        return path;
    }
    //generate a random number of minimum steps
    private int generateNumberOfSteps() {
        return UnityEngine.Random.Range(stepsLowBound, stepsHighBound + 1);
    }
    public void newRandomQuestion() {
        bool isValidQuestion = false;
        int neededSteps;
        while (!isValidQuestion) {
            //generating solution
            solution.Clear();
            neededSteps = generateNumberOfSteps();
            for (int i = 0 ; i < neededSteps ; ++i) {
                solution.Append(generateEquationPath(neededSteps));
            }
            //solution check

        }
        totalSteps = neededSteps + extraSteps;
    }
    */
    private void Awake() {
        /*
        targetLowBound = 10;
        targetHighBound = 99;
        for (int i = 0 ; i < 5 ; ++i) {
            numbersHighBound[i] = 9;
            numbersLowBound[i] = 1;
        }
        for (int i = 0 ; i < 4 ; ++i) {
            signs.Append("+");
            signs.Append("-");
            signs.Append("*");
        }
        equationStepsLowBound = 2;
        equationStepsHighBound = 4;
        stepsLowBound = 1;
        stepsHighBound = 2;
        extraSteps = 1;
        */
        //createRandomPath(2, 5);
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
