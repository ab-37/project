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

    //shuffle array
    private void shuffleArray<T>(ref T[] arr) {
        //fisher-yates shuffle algorithm
        int rng;
        T temp;
        int n = arr.Length;
        while (n > 1) {
            rng = UnityEngine.Random.Range(0, n--);
            temp = arr[n];
            arr[n] = arr[rng];
            arr[rng] = temp;
        }
    }

    /*
    public string generateRandomPathFrom(int stepsLowBoundInclusive, int stepsHighBoundInclusive, int startPoint)
    {
        if (startPoint != 0 && startPoint != 2 && startPoint != 4 && startPoint != 6 && startPoint != 8)
        {
            Debug.LogError("Invalid starting point. Must be 0, 2, 4, 6, or 8.");
            return "";
        }

        string path = "";
        int currentPoint = startPoint;
        int pathLengthMinus2 = UnityEngine.Random.Range(stepsLowBoundInclusive, stepsHighBoundInclusive + 1) - 2; //length of path - 2
        int pathIndex;

        if (startPoint == 4)
            pathIndex = pathArray[pathLengthMinus2].Length-1;//chose '4' first
        else
            pathIndex = UnityEngine.Random.Range(0, pathArray[pathLengthMinus2].Length-1); //without '4' first


        //chose where is it from first
        switch (currentPoint){
            case 0:

                foreach (char c in pathArray[pathLengthMinus2][pathIndex])
                {
                    path += pathFilter[0+ (int)UnityEngine.Random.Range(0,1)*7][int.Parse(c.ToString())].ToString();
                }
                break;
            case 2:
                foreach (char c in pathArray[pathLengthMinus2][pathIndex])
                {
                    path += pathFilter[2 + (int)UnityEngine.Random.Range(0, 1) * 2][int.Parse(c.ToString())].ToString();
                }
                break;
            case 4:
                    path = pathArray[pathLengthMinus2][pathIndex];
                break;
            case 6:
                foreach (char c in pathArray[pathLengthMinus2][pathIndex])
                {
                    path += pathFilter[1 + (int)UnityEngine.Random.Range(0, 1) * 4][int.Parse(c.ToString())].ToString();
                }
                break;
            case 8:
                foreach (char c in pathArray[pathLengthMinus2][pathIndex])
                {
                    path += pathFilter[3 + (int)UnityEngine.Random.Range(0, 1) * 3][int.Parse(c.ToString())].ToString();
                }
                break;
        }
        return path;
    }
    */

    //find path final location
    /*
    public int getLastPointFromPath(string path)
    {
        return int.Parse(path[path.Length - 1].ToString());
    }
    */

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
        foreach (char c in pathArray[pathLengthMinus2][pathIndex]) {
            //Debug.Log(pathFilter[filterIndex][int.Parse(c.ToString())].ToString()); //debug code
            path += pathFilter[filterIndex][int.Parse(c.ToString())].ToString();
        }
        return path;
    }

    //generate first path
    public string generateRandomPath(int stepsLowBoundInclusive, int stepsHighBoundInclusive) {
        int pathLengthMinus2 = UnityEngine.Random.Range(stepsLowBoundInclusive, stepsHighBoundInclusive + 1) - 2; //length of path - 2
        int pathIndex = UnityEngine.Random.Range(0, pathArray[pathLengthMinus2].Length); 
        int filterIndex = UnityEngine.Random.Range(0, pathFilter.Length);
        return getPath(pathLengthMinus2, pathIndex, filterIndex);
    }

    //generate second path, overloading last function
    public string generateRandomPath(int stepsLowBoundInclusive, int stepsHighBoundInclusive, string lastPath) {
        int pathLengthMinus2 = UnityEngine.Random.Range(stepsLowBoundInclusive, stepsHighBoundInclusive + 1) - 2; //length of path - 2
        int pathIndex; //index of path
        int filterIndex; //index of filter
        string path = "";
        
        shuffleArray(ref pathArray[pathLengthMinus2]);
        shuffleArray(ref pathFilter);

        bool pathFound = false;
        //since the arrays are shuffled, we can iterate one by one
        for (pathIndex = 0 ; pathIndex < pathArray[pathLengthMinus2].Length ; ++pathIndex) {
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
