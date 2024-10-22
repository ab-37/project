using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEditor;
using UnityEngine;

public class Randomizer_Script : MonoBehaviour
{   
    //all possible paths
    private static string[][] possiblePaths = {
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

    //variables needed to create new problems
    //ALL BOUNDS ARE INCLUSIVE, +1 TO THE HIGH BOUND WHEN USING UnityEngine.Random.Range()
    private static int goalLowBound, goalHighBound; //range of target number
    private static int[] numbersLowBound = new int[5], numbersHighBound = new int[5]; //range of number in each square
    private static List<string> signs = new List<string>(); //all available signs (pick 4 to use)
    private static int pathLengthLowBound, pathLengthHighBound; //number of numbers needed to create an equation (not including signs)
    private static int stepsLowBound, stepsHighBound; //range of minimum steps needed to complete the question
    private static int extraSteps; //number of extra steps given on top of steps needed

    //current saved problem
    private List<string> paths = new List<string>();
    private string[] grid = new string[9];
    private int goal;
    private int steps;

    //private string[] grid = {"4", "+", "5", "-", "3", "*", "1", "+", "2"};
    //private List<string> solution;
    //private int totalSteps;
    
    Select_Square_Script selectSquareScript;
    Grid_Numbers_Script gridNumbersScript;
    Goal_Script goalScript;
    Remaining_Script remainingScript;

    //shuffle array
    private void shuffleArray<T>(ref T[] arr) {
        int rng;
        T temp;
        int n = arr.Length;
        //fisher-yates shuffle algorithm
        while (n > 1) {
            rng = UnityEngine.Random.Range(0, n--);
            temp = arr[n];
            arr[n] = arr[rng];
            arr[rng] = temp;
        }
    }

    private void shuffleSigns() {
        int rng;
        string temp;
        int n = signs.Count;
        //fisher-yates shuffle algorithm
        while (n > 1) {
            rng = UnityEngine.Random.Range(0, n--);
            temp = signs[n];
            signs[n] = signs[rng];
            signs[rng] = temp;
        }
    }

    //generate steps
    private int generateSteps() {
        return UnityEngine.Random.Range(stepsLowBound, stepsHighBound + 1);
    }

    //generate grid
    private void generateNewGrid() {
        shuffleSigns();
        //generate numbers
        for (int i = 0 ; i < 5 ; ++i) {
            grid[i * 2] = UnityEngine.Random.Range(numbersLowBound[i], numbersHighBound[i] + 1).ToString();
        }

        //throw an error if signs list has less than 4 signs
        if (signs.Count < 4) {
            Debug.Log("Not enough signs in list, defalts to + for all 4 signs");
            for (int i = 0 ; i < 4 ; ++i) {
                grid[i * 2 + 1] = "+";
            }
            return;
        }
        //generate signs
        for (int i = 0 ; i < 4 ; ++i) {
            grid[i * 2 + 1] = signs[i];
        }
    }

    //check if the new path is valid
    private bool isValidPath(string lastPath, string newPath) {
        if (!newPath.Contains(lastPath[lastPath.Length - 1])) {
            //doesn't contain the last square from the last path
            return false;
        }
        if (!newPath.EndsWith(newPath[0].ToString())) {
            //new path doesn't start from the end of the last path
            return true;
        }
        if ((lastPath + newPath).Distinct().Count() == lastPath.Length + newPath.Length - 1) {
            //last path and new path only share one square, i.e. end of last path and start of new path
            //that means it can be done with just one path instead of two
            return false;
        }
        return true;
    }

    //get path from variables
    private string getPath(int pathLengthMinus2, int pathIndex, int filterIndex) {
        string path = "";
        foreach (char c in possiblePaths[pathLengthMinus2][pathIndex]) {
            //Debug.Log(pathFilter[filterIndex][int.Parse(c.ToString())].ToString()); //debug code
            path += pathFilter[filterIndex][int.Parse(c.ToString())].ToString();
        }
        return path;
    }

    //generate first path
    private string generateRandomPath() {
        int pathLengthMinus2 = UnityEngine.Random.Range(pathLengthLowBound, pathLengthHighBound + 1) - 2; //length of path - 2
        int pathIndex = UnityEngine.Random.Range(0, possiblePaths[pathLengthMinus2].Length); 
        int filterIndex = UnityEngine.Random.Range(0, pathFilter.Length);
        return getPath(pathLengthMinus2, pathIndex, filterIndex);
    }

    //generate second path, overloading last function
    private string generateRandomPath(string lastPath) {
        int pathLengthMinus2 = UnityEngine.Random.Range(pathLengthLowBound, pathLengthHighBound + 1) - 2; //length of path - 2
        int pathIndex; //index of path
        int filterIndex; //index of filter
        string path = "";
        
        shuffleArray(ref possiblePaths[pathLengthMinus2]);
        shuffleArray(ref pathFilter);

        bool pathFound = false;
        //since the arrays are shuffled, we can iterate one by one
        for (pathIndex = 0 ; pathIndex < possiblePaths[pathLengthMinus2].Length ; ++pathIndex) {
            for (filterIndex = 0 ; filterIndex < pathFilter.Length ; ++filterIndex) {

                path = getPath(pathLengthMinus2, pathIndex, filterIndex);
                if (isValidPath(lastPath, path)) {
                    pathFound = true;
                    break;
                }
            }
            if (pathFound) {
                break;
            }
        }
        if (!pathFound) {
            Debug.Log("Error: Cannot generate a path. Last path: " + lastPath + ", new path length: " + (pathLengthMinus2 + 2).ToString());
            return "";
        }
        return path;
    }

    //generate an expression from path to feed to calculateResult in Select_Square_Script
    private string pathToExpression(string path, ref string[] tempGrid) {
        string expression = "";
        foreach (char c in path) {
            int n = c - '0';
            expression += tempGrid[n];
            expression += " ";
        }

        return expression;
    }

    private void generateGoal() {
        string[] tempGrid = new string[9];
        for (int i = 0 ; i < 9 ; ++i) {
            tempGrid[i] = grid[i];
        }
        for (int i = 0 ; i < paths.Count ; ++i) {
            //calculate the goal
            string expression = pathToExpression(paths[i], ref tempGrid);
            Debug.Log(expression);
            goal = selectSquareScript.calculateResult(expression);
            //"complete a step"
            tempGrid[int.Parse(paths[i][paths[i].Length - 1].ToString())] = goal.ToString();
        }
    }

    private void generateNewProblem() {
        //generate steps needed
        int stepsNeeded = generateSteps();
        steps = stepsNeeded + extraSteps;

        //generate grid
        generateNewGrid();

        //generate paths
        paths.Clear();
        paths.Add(generateRandomPath()); //first path
        for (int i = 1 ; i < stepsNeeded ; ++i) { //second paths onwards
            paths.Add(generateRandomPath(paths[i - 1]));
        }

        //generate goal
        generateGoal();

        //to do: goal bound check

        //debug code
        string debugLogString = "New problem:\nGoal Number is " + goal.ToString() + "\nGrid:\n";
        for (int i = 2 ; i >= 0 ; --i) {
            for (int j = 0 ; j < 3 ; ++j) {
                debugLogString += grid[i * 3 + j] + " ";
            }
            debugLogString += "\n";
        }
        debugLogString += "\n";
        for (int i = 0 ; i < paths.Count ; ++i) {
            debugLogString += "Path " + i.ToString() + ": " + paths[i]+ "\n";
        }
        Debug.Log(debugLogString);
    }

    //called when reset key is pressed
    public void resetProblem() {
        gridNumbersScript.resetGrid();
        remainingScript.resetSteps();
    }

    //called when new problem key is pressed
    public void newProblem() {
        generateNewProblem();
        gridNumbersScript.setOriginalContent(grid);
        goalScript.setGoalNumber(goal);
        remainingScript.setOriginalSteps(steps);
    }

    private void Awake() {
        
        goalLowBound = 10;
        goalHighBound = 99;
        for (int i = 0 ; i < 5 ; ++i) {
            numbersHighBound[i] = 9;
            numbersLowBound[i] = 1;
        }
        for (int i = 0 ; i < 4 ; ++i) {
            signs.Add("+");
            signs.Add("-");
            signs.Add("*");
        }
        pathLengthLowBound = 2;
        pathLengthHighBound = 4;
        stepsLowBound = 1;
        stepsHighBound = 2;
        extraSteps = 1;
        
        //createRandomPath(2, 5);
        selectSquareScript = gameObject.transform.parent.Find("Select Square").gameObject.GetComponent<Select_Square_Script>();
        gridNumbersScript = gameObject.transform.parent.Find("Background/Grid Numbers").gameObject.GetComponent<Grid_Numbers_Script>();
        goalScript = gameObject.transform.parent.Find("Background/Goal").gameObject.GetComponent<Goal_Script>();
        remainingScript = gameObject.transform.parent.Find("Background/Remaining").gameObject.GetComponent<Remaining_Script>();

    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
