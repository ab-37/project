using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IntBound
{
    public int lowBound;
    public int highBound;
    
    //constructor
    public IntBound(int low = 1, int high = 99) {
        lowBound = low;
        highBound = high;
    }
    
    //convert tuple to intBound
    public static implicit operator IntBound((int, int) tup) 
    => new IntBound(tup.Item1, tup.Item2);

    //add bounds together
    public static IntBound operator+(IntBound a, IntBound b) 
    => new IntBound(a.lowBound + b.lowBound, a.highBound + b.highBound);

    //subtract bounds
    public static IntBound operator-(IntBound a, IntBound b)
    => new IntBound(a.lowBound - b.lowBound, a.highBound - b.highBound);

    //copy a bound
    /*
    public void copy(intBound b) {
        Debug.Log("copy is called");
        lowBound = b.lowBound;
        highBound = b.highBound;
    }
    */

    //check if an int is within bound
    public bool isInBound(int n) 
    => n >= lowBound && n <= highBound;

    //output a random int within bound
    public int random()
    => UnityEngine.Random.Range(lowBound, highBound + 1);
}

public class Problem
{
    //private List<string> paths = new List<string>(); //move to QuestionParameters?
    private string[] grid = new string[9];
    private int goal = 0;
    private int steps = 0;

    public Problem() {

    }

    //set variables
    public void setGrid(ref string[] newGrid) {
        if (newGrid.Length != 9) {
            Debug.Log("ERROR: Grid does not have 9 elements");
            return;
        }
        for (int i = 0 ; i < 9 ; ++i) {
            grid[i] = newGrid[i];
        }
    }

    public void setGrid(int ind, string content) {
        if (ind > 8 || ind < 0) {
            Debug.Log("ERROR: Grid index out of bounds");
            return;
        }
        grid[ind] = content;
    }

    public void setGoal(int newGoal) {
        goal = newGoal;
    }

    public void setSteps(int newSteps) {
        steps = newSteps;
    }
    
    //get variables
    //public string[] getGrid() => grid;
    public string getGridContent(int ind) => grid[ind];

    public int getGoal() => goal;

    public int getSteps() => steps;

    /*
    public void copy(Problem p) {
        for (int i = 0 ; i < 9 ; ++i) {
            grid[i] = p.grid[i];
        }
        goal = p.goal;
        steps = p.steps;
    }
    */
}

public class QuestionParameters
{
    //static variables for functions
    private static string[][] possiblePathsArray = {
        new string[] {"012", "014", "452"}, //3 squares
        new string[] {"01254", "01258", "01436", "01476", "01478", "01458", "01452", "45210"}, //5 squares
        new string[] {"0125436", "0125476", "0125478", "0125876", "0125874", "0143678", "0147852", "0145876", "4521036"}, //7 squares
        new string[] {"012543678", "012587634", "012587436", "014367852", "452103678"} //9 squares
    };

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

    /*
    private int goalLowBound, goalHighBound; //range of target number, HAS TO BE A SUBRANGE OF STEP ANSWER BOUND
    private int stepAnswerLowBound, stepAnswerHighBound; //range of the answer of every step
    private int[] numbersLowBound = new int[5], numbersHighBound = new int[5]; //range of number in each square
    private List<string> signs = new List<string>(); //all available signs
    private List<string> availablePaths = new List<string>(); //all available paths
    private int stepsLowBound, stepsHighBound; //range of minimum steps needed to complete the question
    private int extraSteps; //number of extra steps given on top of steps needed
    */
    private int objective; //time limit or points needed for the level
    private int levelMode; //level mode, 1 = countdown, 2 = point requirement
    private IntBound goalBound; //range of target number, HAS TO BE A SUBRANGE OF STEP ANSWER BOUND
    private IntBound stepAnswerBound; //range of the answer of every step
    private IntBound[] numbersBound = new IntBound[5]; //range of number in each square
    private List<string> availablePaths = new List<string>(); //all available paths
    private IntBound stepsBound; //range of minimum steps needed to complete the question
    private int extraSteps; //number of extra steps given on top of steps needed
    private List<string> signs = new List<string>(); //all available signs
    private int specialMode; //special cases
    /*
    !!!!!!!!!!!!!!!!!!!!!
    START OF CONSTRUCTORS
    !!!!!!!!!!!!!!!!!!!!!
    */
    //case 1: path is a bound
    public QuestionParameters(
        int objective,
        int levelMode,
        IntBound goalBound,
        IntBound stepAnswerBound,
        (IntBound, IntBound, IntBound, IntBound, IntBound) numbersBound,
        IntBound pathBound,
        IntBound stepsBound,
        int extraSteps,
        bool add = true,
        bool sub = true,
        bool mul = true,
        bool div = false,
        int specialMode = 0
        ) {
        //copy bounds
        /*
        this.goalBound.copy(goalBound);
        this.stepAnswerBound.copy(stepAnswerBound);
        this.stepsBound.copy(stepsBound);
        this.extraSteps = extraSteps;

        this.numbersBound[0].copy(numbersBound.Item1);
        this.numbersBound[1].copy(numbersBound.Item2);
        this.numbersBound[2].copy(numbersBound.Item3);
        this.numbersBound[3].copy(numbersBound.Item4);
        this.numbersBound[4].copy(numbersBound.Item5);
        */
        this.objective = objective;
        this.levelMode = levelMode;
        this.goalBound = goalBound;
        this.stepAnswerBound = stepAnswerBound;
        this.stepsBound = stepsBound;
        this.extraSteps = extraSteps;

        this.numbersBound[0] = numbersBound.Item1;
        this.numbersBound[1] = numbersBound.Item2;
        this.numbersBound[2] = numbersBound.Item3;
        this.numbersBound[3] = numbersBound.Item4;
        this.numbersBound[4] = numbersBound.Item5;

        //signs
        //check if there are signs
        if (!(add || sub || mul || div)) {
            Debug.Log("no signs, adding + only");
            add = true;
        }
        if (add) signs.Add("+");
        if (sub) signs.Add("-");
        if (mul) signs.Add("*");
        if (div) signs.Add("/");
        
        //paths, fetch from possiblePathsArray
        pathBound -= (2, 2);
        for (int i = pathBound.lowBound ; i <= pathBound.highBound ; ++i) {
            foreach (string path in possiblePathsArray[i]) {
                availablePaths.Add(path);
            }
        }

        //check if availablePaths is empty
        if (!availablePaths.Any()) {
            Debug.Log("no paths, adding \"012\"");
            availablePaths.Add("012");
        }
        
        //special mode
        this.specialMode = specialMode;
    }

    //case 2: path is a special array of paths
    public QuestionParameters(
        int objective,
        int levelMode,
        IntBound goalBound,
        IntBound stepAnswerBound,
        (IntBound, IntBound, IntBound, IntBound, IntBound) numbersBound,
        ref string[] paths,
        IntBound stepsBound,
        int extraSteps,
        bool add = true,
        bool sub = true,
        bool mul = true,
        bool div = false,
        int specialMode = 0
        ) {

        this.objective = objective;
        this.levelMode = levelMode;
        //copy bounds
        this.goalBound = goalBound;
        this.stepAnswerBound = stepAnswerBound;
        this.stepsBound = stepsBound;
        this.extraSteps = extraSteps;

        this.numbersBound[0] = numbersBound.Item1;
        this.numbersBound[1] = numbersBound.Item2;
        this.numbersBound[2] = numbersBound.Item3;
        this.numbersBound[3] = numbersBound.Item4;
        this.numbersBound[4] = numbersBound.Item5;

        //signs
        //check if there are signs
        if (!(add || sub || mul || div)) {
            Debug.Log("no signs, adding + only");
            add = true;
        }
        if (add) signs.Add("+");
        if (sub) signs.Add("-");
        if (mul) signs.Add("*");
        if (div) signs.Add("/");
        
        //paths
        foreach (string path in paths) {
            availablePaths.Add(path);
        }

        //check if availablePaths is empty
        if (!availablePaths.Any()) {
            Debug.Log("no paths, adding \"012\"");
            availablePaths.Add("012");
        }

        //special mode
        this.specialMode = specialMode;
    }

    //case 3: empty constructor
    public QuestionParameters() {

    }
    /*
    !!!!!!!!!!!!!!!!!!!
    END OF CONSTRUCTORS
    !!!!!!!!!!!!!!!!!!!
    */

    public int getLevelMode() 
    => levelMode;

    public int getObjective() 
    => objective;

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
    private bool isValidProblem(ref Problem problem, ref List<string> paths) {
        string[] tempGrid = new string[9];
        int lastAnswer = new int();
        //copy grid to a temporary grid
        for (int i = 0 ; i < 9 ; ++i) {
            tempGrid[i] = problem.getGridContent(i);
        }

        //process each path
        foreach (string path in paths) {
            lastAnswer = processPath(path, ref tempGrid);
            if (!stepAnswerBound.isInBound(lastAnswer)) {
                //answer out of bounds
                return false;
            }
        }
        //check if the goal is in bound
        if (!goalBound.isInBound(lastAnswer)) {
            //out of bounds
            return false;
        }

        //check if the goal is one of the numbers in the grid
        for (int i = 0 ; i <= 8 ; i += 2) {
            if (lastAnswer == int.Parse(problem.getGridContent(i))) {
                return false;
            }
        }

        problem.setGoal(lastAnswer);
        return true;
    }

    //generate signs
    private bool generateSigns(ref bool[] squaresUsed, ref Problem problem, ref List<string> paths, int signSquare) {
        if (signSquare > 3) {
            //all signs are generated, check target
            //to do: need to check if used signs are a subset of all signs
            return isValidProblem(ref problem, ref paths);

            //int g = generateGoal();
            //return g >= goalLowBound && g <= goalHighBound;
        }
        if (specialMode == 0 && (!squaresUsed[signSquare * 2 + 1])) {
            //this sign is not used, generate a random sign
            problem.setGrid(signSquare * 2 + 1, signs[UnityEngine.Random.Range(0, signs.Count)]);
            return generateSigns(ref squaresUsed, ref problem, ref paths, signSquare + 1);
        }

        //create a list with all signs that can be used in this square
        List<int> signsIndexList = new List<int>();
        for (int i = 0 ; i < signs.Count ; ++i) {
            signsIndexList.Add(i);
        }
        //shuffle the list
        Static_Functions.shuffleList(ref signsIndexList);

        //special mode 1: all same signs
        if (specialMode == 1) {
            foreach (int i in signsIndexList) {
                for (int j = 1 ; j <= 7 ; j += 2) {
                    problem.setGrid(j, signs[i]);
                }
                if (isValidProblem(ref problem, ref paths)) {
                    return true;
                }
            }
            return false;
        }

        //check if the sign can be used
        foreach (int i in signsIndexList) {
            problem.setGrid(signSquare * 2 + 1, signs[i]);
            if (generateSigns(ref squaresUsed, ref problem, ref paths, signSquare + 1)) {
                return true;
            }
        }

        //if every sign failed
        return false;
    }

    //generate a number square
    private bool generateNumber(ref bool[] squaresUsed, ref Problem problem, ref List<string> paths, int numSquare) {
        if (numSquare > 4) {
            //all numbers are generated, generate signs
            return generateSigns(ref squaresUsed, ref problem, ref paths, 0);
        }
        if (!squaresUsed[numSquare * 2]) {
            //this square is not used, generate a random number
            problem.setGrid(numSquare * 2, numbersBound[numSquare].random().ToString());
            return generateNumber(ref squaresUsed, ref problem, ref paths, numSquare + 1);
        }

        //create a list with all numbers that can be used in this square
        List<int> numbersList = new List<int>();
        for (int i = numbersBound[numSquare].lowBound ; i <= numbersBound[numSquare].highBound ; ++i) {
            numbersList.Add(i);
        }
        //shuffle the list
        Static_Functions.shuffleList(ref numbersList);

        //check if the number can be used
        foreach (int n in numbersList) {
            problem.setGrid(numSquare * 2, n.ToString());
            if (generateNumber(ref squaresUsed, ref problem, ref paths, numSquare + 1)) {
                return true;
            }
        }

        //if every number failed
        return false;
    }

    //generate a new grid
    private bool generateNewGrid(ref bool[] squaresUsed, ref List<string> paths, ref Problem problem) {
        return generateNumber(ref squaresUsed, ref problem, ref paths, 0);
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
    private string getPath(int pathIndex, int filterIndex) {
        string path = "";
        foreach (char c in availablePaths[pathIndex]) {
            //Debug.Log(pathFilter[filterIndex][int.Parse(c.ToString())].ToString()); //debug code
            path += pathFilter[filterIndex][int.Parse(c.ToString())].ToString();
        }
        return path;
    }

    /*
    //generate first path
    private string generateRandomPath() {
        //int pathLengthMinus2 = UnityEngine.Random.Range(pathLengthLowBound, pathLengthHighBound + 1) - 2; //length of path - 2
        int pathIndex = UnityEngine.Random.Range(0, availablePaths.Count); 
        int filterIndex = UnityEngine.Random.Range(0, pathFilter.Length);
        return getPath(pathIndex, filterIndex);
    }

    //generate second path, overloading last function
    private string generateRandomPath(string lastPath) {
        //int pathLengthMinus2 = UnityEngine.Random.Range(pathLengthLowBound, pathLengthHighBound + 1) - 2; //length of path - 2
        int pathIndex; //index of path
        int filterIndex; //index of filter
        string path = "";
        
        Static_Functions.shuffleList(ref availablePaths);
        Static_Functions.shuffleArray(ref pathFilter);

        bool pathFound = false;
        //since the arrays are shuffled, we can iterate one by one
        for (pathIndex = 0 ; pathIndex < availablePaths.Count ; ++pathIndex) {
            for (filterIndex = 0 ; filterIndex < pathFilter.Length ; ++filterIndex) {

                path = getPath(pathIndex, filterIndex);
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
            Debug.Log("Error: Cannot generate a path. Last path: " + lastPath);
            return "";
        }
        return path;
    }

    //generate all paths needed (old)
    private void generatePaths(ref List<string> paths, ref int stepsNeeded) {
        paths.Clear();
        if (stepsNeeded == 0) {
            Debug.Log("Error, Steps needed is 0, generating one anyway");
        }
        paths.Add(generateRandomPath()); //first path
        for (int i = 1 ; i < stepsNeeded ; ++i) { //second paths onwards
            paths.Add(generateRandomPath(paths[i - 1]));
        }
    }
    */

    //generate paths (new algorithm)
    private bool generatePaths(ref List<string> paths, int stepsNeeded, ref Problem problem, bool isFirstPath) {
        if (stepsNeeded == 0) {
            //path generation is done
            bool[] squareUsed = {false, false, false, false, false, false, false, false, false};
            foreach (string path in paths) {
                foreach (char c in path) {
                    squareUsed[int.Parse(c.ToString())] = true;
                }
            }
            return generateNewGrid(ref squareUsed, ref paths, ref problem);
        }

        //create a new list of path indexes shuffled
        List<int> pathIndexList = new List<int>();
        for (int i = 0 ; i < availablePaths.Count() ; ++i) {
            pathIndexList.Add(i);
        }
        Static_Functions.shuffleList(ref pathIndexList);
        
        //used to avoid calling getPath multiple times
        string nextPath;

        //go through every path and filter
        foreach (int pathIndex in pathIndexList) {
            //create a new list of filter indexes shuffled
            int[] filterIndexList = {0, 1, 2, 3, 4, 5, 6, 7};
            Static_Functions.shuffleArray(ref filterIndexList);

            foreach (int filterIndex in filterIndexList) {
                nextPath = getPath(pathIndex, filterIndex);
                if (!isFirstPath) {
                    if (!isValidPath(paths.LastOrDefault(), nextPath)) {
                        //not a valid path, try next path
                        continue;
                    }
                }
                //add the path
                paths.Add(nextPath);

                //debug
                //Debug.Log(stepsNeeded.ToString() + "paths left, Trying path" + paths.LastOrDefault());

                if (generatePaths(ref paths, stepsNeeded - 1, ref problem, false)) {
                    return true;
                }

                //path failed, remove last path
                paths.RemoveAt(paths.Count() - 1);
            }
        }

        //if everything failed
        return false;
    }

    //generate a new problem
    public Problem generateNewProblem() {
        //generate a new problem variable
        Problem problem = new Problem();

        //generate steps needed
        int stepsNeeded = stepsBound.random();
        problem.setSteps(stepsNeeded + extraSteps);

        //generate paths
        List<string> paths = new List<string>();
        /*
        generatePaths(ref paths, ref stepsNeeded);
        
        //check which squares are used
        bool[] squareUsed = {false, false, false, false, false, false, false, false, false};
        foreach (string path in paths) {
            foreach (char c in path) {
                squareUsed[int.Parse(c.ToString())] = true;
            }
        }
        if (!generateNewGrid(ref squareUsed, ref paths, ref problem)) {
            Debug.Log("Error: Generate new problem failed");
        }
        */
        if (!generatePaths(ref paths, stepsNeeded, ref problem, true)) {
            Debug.Log("Error: Generate new problem failed");
        }
        
        //debug log
        newProblemDebugLog(ref problem, ref paths);

        return problem;
    }

    private void newProblemDebugLog(ref Problem problem, ref List<string> paths) {
        string debugLogString = "New problem:\nGoal Number is " + problem.getGoal().ToString() + "\nGrid:\n";
        for (int i = 2 ; i >= 0 ; --i) {
            for (int j = 0 ; j < 3 ; ++j) {
                debugLogString += problem.getGridContent(i * 3 + j) + " ";
            }
            debugLogString += "\n";
        }
        debugLogString += "\n";
        for (int i = 0 ; i < paths.Count ; ++i) {
            debugLogString += "Path " + i.ToString() + ": " + paths[i]+ "\n";
        }
        Debug.Log(debugLogString);
    }

}