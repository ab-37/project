using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private string[] pathFilter = {
        "012345678", //no rotation
        "630741852", //90 clockwise
        "258147036", //90 counterclockwise
        "876543210", //180
        "210543876", //horizontal flip
        "678345012", //vertical flip
        "852741630", //90 clockwise, horizontal flip
        "036147258", //90 counterclockwise, horizontal flip
    };

    /*
    !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    THE OLD VARIABLES, KEEP FOR FALLBACK
    !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    */
    /*
    //variables needed to create new problems
    //ALL BOUNDS ARE INCLUSIVE, +1 TO THE HIGH BOUND WHEN USING UnityEngine.Random.Range()
    private int goalLowBound, goalHighBound; //range of target number, HAS TO BE A SUBRANGE OF STEP ANSWER BOUND
    private int stepAnswerLowBound, stepAnswerHighBound; //range of the answer of every step
    private int[] numbersLowBound = new int[5], numbersHighBound = new int[5]; //range of number in each square
    private List<string> signsLegacy = new List<string>(); //(legacy) all available signs including duplicates (pick 4 to use)
    private List<string> signs = new List<string>(); //all available signs
    private int pathLengthLowBound, pathLengthHighBound; //number of numbers needed to create an equation (not including signs)
    private int stepsLowBound, stepsHighBound; //range of minimum steps needed to complete the question
    private int extraSteps; //number of extra steps given on top of steps needed

    //current saved problem
    private List<string> paths = new List<string>();
    private string[] grid = new string[9];
    private int goal;
    private int steps;

    //private string[] grid = {"4", "+", "5", "-", "3", "*", "1", "+", "2"};
    //private List<string> solution;
    //private int totalSteps;
    */
    //private QuestionParameters questionParameters;
    private Problem currentProblem;
    
    //private Select_Square_Script selectSquareScript;
    private Grid_Numbers_Script gridNumbersScript;
    private Goal_Script goalScript;
    private Remaining_Script remainingScript;
    private Score_Script scoreScript;

    /*
    //moved to static functions

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

    private void shuffleList<T>(ref List<T> list) {
        int rng;
        T temp;
        int n = list.Count;
        //fisher-yates shuffle algorithm
        while (n > 1) {
            rng = UnityEngine.Random.Range(0, n--);
            temp = list[n];
            list[n] = list[rng];
            list[rng] = temp;
        }
    }
    */

    //generate steps
    /*
    private int generateSteps() {
        return UnityEngine.Random.Range(stepsLowBound, stepsHighBound + 1);
    }
    */


    /*    
    //generate grid (legacy, now only used in failed instances)
    private void generateNewGridLegacy() {
        Static_Functions.shuffleList(ref signsLegacy);
        //generate numbers
        for (int i = 0 ; i < 5 ; ++i) {
            grid[i * 2] = UnityEngine.Random.Range(numbersLowBound[i], numbersHighBound[i] + 1).ToString();
        }

        //throw an error if signs list has less than 4 signs
        if (signsLegacy.Count < 4) {
            Debug.Log("Not enough signs in list, defalts to + for all 4 signs");
            for (int i = 0 ; i < 4 ; ++i) {
                grid[i * 2 + 1] = "+";
            }
            return;
        }
        //generate signs
        for (int i = 0 ; i < 4 ; ++i) {
            grid[i * 2 + 1] = signsLegacy[i];
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
        
        Static_Functions.shuffleArray(ref possiblePaths[pathLengthMinus2]);
        Static_Functions.shuffleArray(ref pathFilter);

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

    //generate all paths needed
    private void generatePaths(int stepsNeeded) {
        paths.Clear();
        if (stepsNeeded == 0) {
            Debug.Log("Error, Steps needed is 0, generating one anyway");
        }
        paths.Add(generateRandomPath()); //first path
        for (int i = 1 ; i < stepsNeeded ; ++i) { //second paths onwards
            paths.Add(generateRandomPath(paths[i - 1]));
        }
        
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
    
    

    private int generateGoalLegacy() {
        string[] tempGrid = new string[9];
        for (int i = 0 ; i < 9 ; ++i) {
            tempGrid[i] = grid[i];
        }
        for (int i = 0 ; i < paths.Count ; ++i) {
            //calculate the goal
            string expression = pathToExpression(paths[i], ref tempGrid);
            //Debug.Log(expression);
            goal = Static_Functions.calculateResult(expression);
            //"complete a step"
            tempGrid[int.Parse(paths[i][paths[i].Length - 1].ToString())] = goal.ToString();
        }
        return goal;
    }

    //output debug log for grid
    private void newProblemDebugLog() {
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
    */

    /*
    //generate a new problem (legacy)
    private void generateNewProblemLegacy() {
        //generate steps needed
        int stepsNeeded = generateSteps();
        steps = stepsNeeded + extraSteps;

        //generate grid
        generateNewGridLegacy();

        //generate paths
        generatePaths(stepsNeeded);

        //generate goal
        generateGoal();

        //to do: goal bound check

        //debug log
        newProblemDebugLog();
    }
    */

    /*
    !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    THIS IS THE START OF THE NEW ALGORITHM
    !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

    time complexity: O(n^5 * s^4)
    n = possible numbers, s = possible signs
    */

    //generate signs that can be used
    /*
    private void updateSignsLegacy() {
        signs.Clear();
        foreach (string sign in signs) {
            if (!signs.Contains(sign)) {
                signs.Add(sign);
            }
        }
    }
    */
    
    /*
    //complete a path and fill the last square
    private int processPath(string path, ref string[] grid) {
        //calculate the answer
        string expression = pathToExpression(path, ref grid);
        //Debug.Log(expression);
        int answer = Static_Functions.calculateResult(expression);
        //"complete a step"
        grid[int.Parse(path[path.Length - 1].ToString())] = answer.ToString();
        return answer;
    }

    //check if the problem can be used
    private bool isValidProblem() {
        string[] tempGrid = new string[9];
        int lastAnswer = new int();
        //copy grid to a temporary grid
        for (int i = 0 ; i < 9 ; ++i) {
            tempGrid[i] = grid[i];
        }

        //process each path
        foreach (string path in paths) {
            lastAnswer = processPath(path, ref tempGrid);
            if (lastAnswer > stepAnswerHighBound || lastAnswer < stepAnswerLowBound) {
                //answer out of bounds
                return false;
            }
        }
        //check if the goal is in bound
        if (lastAnswer > goalHighBound || lastAnswer < goalLowBound) {
            //out of bounds
            return false;
        }
        goal = lastAnswer;
        return true;
    }

    //generate signs
    private bool generateSigns(ref bool[] squaresUsed, int signSquare) {
        if (signSquare > 3) {
            //all signs are generated, check target
            //to do: need to check if used signs are a subset of all signs
            return isValidProblem();

            //int g = generateGoal();
            //return g >= goalLowBound && g <= goalHighBound;
        }
        if (!squaresUsed[signSquare * 2 + 1]) {
            //this sign is not used, generate a random sign
            grid[signSquare * 2 + 1] = signs[UnityEngine.Random.Range(0, signs.Count)];
            return generateSigns(ref squaresUsed, signSquare + 1);
        }

        //create a list with all signs that can be used in this square
        List<int> signsIndexList = new List<int>();
        for (int i = 0 ; i < signs.Count ; ++i) {
            signsIndexList.Add(i);
        }
        //shuffle the list
        Static_Functions.shuffleList(ref signsIndexList);

        //check if the sign can be used
        foreach (int i in signsIndexList) {
            grid[signSquare * 2 + 1] = signs[i];
            if (generateSigns(ref squaresUsed, signSquare + 1)) {
                return true;
            }
        }

        //if every sign failed
        return false;
    }

    //generate a number square
    private bool generateNumber(ref bool[] squaresUsed, int numSquare) {
        if (numSquare > 4) {
            //all numbers are generated, generate signs
            return generateSigns(ref squaresUsed, 0);
        }
        if (!squaresUsed[numSquare * 2]) {
            //this square is not used, generate a random number
            grid[numSquare * 2] = UnityEngine.Random.Range(numbersLowBound[numSquare], numbersHighBound[numSquare] + 1).ToString();
            return generateNumber(ref squaresUsed, numSquare + 1);
        }

        //create a list with all numbers that can be used in this square
        List<int> numbersList = new List<int>();
        for (int i = numbersLowBound[numSquare] ; i <= numbersHighBound[numSquare] ; ++i) {
            numbersList.Add(i);
        }
        //shuffle the list
        Static_Functions.shuffleList(ref numbersList);

        //check if the number can be used
        foreach (int n in numbersList) {
            grid[numSquare * 2] = n.ToString();
            if (generateNumber(ref squaresUsed, numSquare + 1)) {
                return true;
            }
        }

        //if every number failed
        return false;
    }

    //generate a new grid
    private bool generateNewGrid(ref bool[] squaresUsed) {
        return generateNumber(ref squaresUsed, 0);
    }

    //generate a new problem
    private void generateNewProblem() {
        //generate steps needed
        int stepsNeeded = generateSteps();
        steps = stepsNeeded + extraSteps;

        //generate paths
        generatePaths(stepsNeeded);
        
        //check which squares are used
        bool[] squareUsed = {false, false, false, false, false, false, false, false, false};
        foreach (string path in paths) {
            foreach (char c in path) {
                squareUsed[int.Parse(c.ToString())] = true;
            }
        }

        //generate signs that can be used (obsolete, signsUnique is now the default)
        //updateSignsUnique();

        //if the new algorithm fails use the old algorithm
        if (!generateNewGrid(ref squareUsed)) {
            Debug.Log("Error: Cannot generate grid, falling back to legacy code");
            generateNewGridLegacy();
            generateGoalLegacy();
        }
        
        //debug log
        newProblemDebugLog();

    }
    */

    /*
    !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    THIS IS THE END OF THE NEW ALGORITHM
    !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    */

    //called when reset key is pressed
    public void resetProblem() {
        //subtract 5 from score
        scoreScript.addScore(-5);
        gridNumbersScript.resetGrid();
        remainingScript.resetSteps();
    }

    //placeholder code
    /*
    public QuestionParameters QP = new QuestionParameters(
            (10, 50), (1, 50), 
            ((1, 7), (1, 7), (1, 7), (1, 7), (1, 7)),
            (2, 5), (1, 1), 1, true, true, true, false);
    */

    //called when new problem key is pressed
    public void newProblem() {
        currentProblem = Static_Variables.levelQuestionParameters[Static_Variables.level].generateNewProblem();
        
        //placeholder code
        //currentProblem.copy(QP.generateNewProblem());
        //currentProblem = QP.generateNewProblem();
        string[] tempGrid = new string[9];
        for (int i = 0 ; i < 9 ; ++i) {
            tempGrid[i] = currentProblem.getGridContent(i);
        }

        //subtract 15 from score
        scoreScript.addScore(-15);
        gridNumbersScript.setOriginalContent(tempGrid);
        goalScript.setGoalNumber(currentProblem.getGoal());
        remainingScript.setOriginalSteps(currentProblem.getSteps());
    }

    //set all parameters to default value
    //legacy code, keep for testing purposes
    /*
    private void setDefaultParameters() {
        goalLowBound = 10;
        goalHighBound = 99;
        stepAnswerLowBound = 1;
        stepAnswerHighBound = 99;
        for (int i = 0 ; i < 5 ; ++i) {
            numbersHighBound[i] = 9;
            numbersLowBound[i] = 1;
        }
        
        //obsolete code, keep for fallback functions
        for (int i = 0 ; i < 4 ; ++i) {
            signsLegacy.Add("+");
            signsLegacy.Add("-");
            signsLegacy.Add("*");
        }

        signs.Add("+");
        signs.Add("-");
        signs.Add("*");

        pathLengthLowBound = 2;
        pathLengthHighBound = 4;
        stepsLowBound = 1;
        stepsHighBound = 2;
        extraSteps = 1;
    }
    */

    private void Awake() {
        //setDefaultParameters();
        //createRandomPath(2, 5);
        //selectSquareScript = gameObject.transform.parent.Find("Select Square").gameObject.GetComponent<Select_Square_Script>();
        gridNumbersScript = gameObject.transform.parent.Find("Background/Grid Numbers").gameObject.GetComponent<Grid_Numbers_Script>();
        goalScript = gameObject.transform.parent.Find("Background/Goal").gameObject.GetComponent<Goal_Script>();
        remainingScript = gameObject.transform.parent.Find("Background/Remaining").gameObject.GetComponent<Remaining_Script>();
        scoreScript = gameObject.transform.parent.Find("Background/Score").gameObject.GetComponent<Score_Script>();

    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}