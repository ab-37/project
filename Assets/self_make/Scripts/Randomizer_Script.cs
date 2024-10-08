using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Randomizer_Script : MonoBehaviour
{
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
    private void Start()
    {
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
    }

    private void Update()
    {
        
    }
}
