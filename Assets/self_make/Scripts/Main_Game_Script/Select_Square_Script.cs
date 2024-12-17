using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Select_Square_Script : MonoBehaviour
{
    bool selectState; //if Z is pressed
    private KeyCode upKey;
    private KeyCode downKey;
    private KeyCode leftKey;
    private KeyCode rightKey;
    private KeyCode selectKey;
    private KeyCode resetKey;
    private KeyCode newQuestionKey;
    private KeyCode spaceKey;
    private int currentPos;
    //private int lastPos;
    private Vector3 unitVectorX;
    private Vector3 unitVectorY;
    private Vector3 realPos;
    public Vector3 gridOrigin;
    public int squareLength; //length of the side of one square
    private List<int> selectedPositions = new List<int>();

    private bool isGameActive;
    private bool isIntroDone; //true if intro has played
    private bool isIntroRunning; //true if intro coroutine is running
    private bool isTimeUp; //true if the time is up
    private bool isOutroRunning; //true if the outro coroutine is running
    private float initialTimerSecs; //timer length

    private Grid_Numbers_Script gridNumbersScript;
    private Selected_Squares_Script selectedSquaresScript;
    private Goal_Script goalScript;
    private Function_Word_Script functionWordScript;
    private Remaining_Script remainingScript;
    private Randomizer_Script randomizerScript;
    private Score_Script scoreScript;
    private Timer_Script timerScript;
    private Intro_Script introScript;
    private Outro_Script outroScript;

    //key input handlers
    private int directionHandler()
    {
        int dir = 0;
        if (Input.GetKeyDown(upKey))
        {
            dir = 3;
        }
        else if (Input.GetKeyDown(downKey))
        {
            dir = -3;
        }
        else if (Input.GetKeyDown(leftKey))
        {
            dir = -1;
        }
        else if (Input.GetKeyDown(rightKey))
        {
            dir = 1;
        }
        return dir;
    }

    //calculate result (moved to static functions)
    /*
    public int calculateResult(string expression) {
        List<string> expressionList = expression.Split(' ').ToList();
        //pos to number and sign
        List<int> numbers = new List<int>();
        List<string> signs = new List<string>();
        int i;
        foreach (string element in expressionList) {
            //Debug.Log(element);
            if (int.TryParse(element, out i)) {
                //number
                //i is used as a temp variable
                numbers.Add(i);
            }
            else {
                //sign
                signs.Add(element);
            }
        }
        
        if (numbers.Count() == signs.Count()) {
            //last sign not followed by anything, delete last sign
            signs.RemoveAt(signs.Count() - 1);
        }
        

        //process mult and div
        i = 0; //i is used as an iterator
        while (i < signs.Count()) {
            if (signs[i] == "*") {
                numbers[i] *= numbers[i + 1];
                numbers.RemoveAt(i + 1);
                signs.RemoveAt(i);
            }
            else if (signs[i] == "/") {
                if (numbers[i + 1] == 0) {
                    //divide by 0, defaults value to 0
                    Debug.Log("Divide by zero!");
                    numbers[i] = 0;
                }
                else {
                    numbers[i] /= numbers[i + 1];
                }
                numbers.RemoveAt(i + 1);
                signs.RemoveAt(i);
            }
            else {
                ++i;
            }
        }
        //process add and sub
        while (signs.Any()) {
            if (signs[0] == "+") {
                numbers[0] += numbers[1];
            }
            else {
                numbers[0] -= numbers[1];
            }
            numbers.RemoveAt(1);
            signs.RemoveAt(0);
        }

        return numbers[0];
    }
    */
    
    //output expression
    private string expressionToString() {
        string outputString = "";
        foreach (int pos in selectedPositions) {
            outputString += gridNumbersScript.getGridContent(pos);
            outputString += " ";
        }
        //debug code
        //Debug.Log(outputString + "[End]");
        return outputString;
    }

    private string concatExpressionToLastCell(string expression, int newPos) {
        return expression + gridNumbersScript.getGridContent(newPos);
    }

    //selecting or deselecting a square
    private void trySelect(int newPos) {
        if (selectedSquaresScript.isSquareSelected(newPos)) {
            //backtrack
            selectedSquaresScript.deselectSquare(currentPos);
            selectedPositions.RemoveAt(selectedPositions.Count() - 1);
        }
        
        else
        {
            //add previous cell
            selectedPositions.Add(currentPos);
            selectedSquaresScript.selectSquare(newPos);
        }
        
        /*
        //add previous cell
        selectedPositions.Add(currentPos);
        selectedSquaresScript.selectSquare(newPos);
        */
        //update function word
        string expression = concatExpressionToLastCell(expressionToString(), newPos);
        int result = Static_Functions.calculateResult(expression);
        functionWordScript.updateText(expression, result);
    }

    //moved to Randomizer_Script
    /*
    //reset grid by pressing reset
    public void resetGrid() {
        //gridNumbersScript.resetGrid();
        //goalScript.setGoalNumber(23); //placeholder code, should be a generated number
        //remainingScript.setOriginalSteps(3); //placeholder code, should be a generated number
    }

    //new grid by pressing new question
    public void newGrid() {
        
        
        //string pathString = randomizerScript.generateRandomPath(2, 5);
        //Debug.Log("first path"+pathString); //placeholder code
        //string pathString2 = randomizerScript.generateRandomPath(2, 5, pathString);
        //Debug.Log(pathString + ", " + pathString2); //placeholder code
        
        //int lastPoint = randomizerScript.getLastPointFromPath(pathString);
        //Debug.Log("second path:"+randomizerScript.generateRandomPathFrom(2,5,lastPoint)); //placeholder code
        
        resetGrid();
    }
    */

    //press or release select
    private void selectPress()
    {
        //Debug.Log("Select"); //debug code
        if (currentPos % 2 == 1) {
            //selected a sign
            Debug.Log("Select failed, selected a sign");
            return;
        }
        if (!remainingScript.hasSteps()) {
            //no more steps remaining
            Debug.Log("Select failed, no more steps");
            return;
        }
        selectState = true;
        selectedSquaresScript.selectSquare(currentPos);
        string expression = gridNumbersScript.getGridContent(currentPos);
        int result = int.Parse(expression);
        functionWordScript.updateText(expression, result);

        //string expression = expressionToString();
    }
    private void selectRelease()
    {
        //Debug.Log("Deselect"); //debug code
        if (currentPos % 2 == 1 && selectedPositions.Count() == 0) {
            //selected a sign
            return;
        }
        if (currentPos % 2 != 1) {
            //is not on a sign
            selectedPositions.Add(currentPos); //add last cell
        }
        int realUpdatePos = selectedPositions.LastOrDefault();

        //decrement step only if 3+ squares selected
        if (selectedPositions.Count() > 2) {
            remainingScript.decrementStep();
        }
        
        string expression = expressionToString();
        int result = Static_Functions.calculateResult(expression);
        //debug code
        Debug.Log("Expression: " + expression + "= " + result);

        functionWordScript.updateText(expression, result);

        if (goalScript.isGoal(result)) {
            //correct
            Debug.Log("Correct!");
            //add score
            scoreScript.addScore(30 + remainingScript.getCurrentSteps() * 10);
            randomizerScript.newProblem();
        }
        else {
            gridNumbersScript.setGridContent(realUpdatePos, result.ToString());
            //moves - 1
        }

        selectState = false;
        selectedSquaresScript.deselectAllSquares();
        selectedPositions.Clear();
    }

    //moving square
    private void updatePos(int newPos)
    {
        //lastPos = currentPos;
        currentPos = newPos;
        realPos = gridOrigin + (currentPos % 3 * unitVectorX) + (currentPos / 3 * unitVectorY);
        transform.position = realPos;
        //Debug.Log(currentPos);
    }

    private bool isInvalidMovement(int newPos, int dir) {
        if (newPos > 8 || newPos < 0) {
            //up or down OOB
            return true;
        }
        if (newPos % 3 == 0 && dir == 1) {
            //right OOB
            return true;
        }
        if (newPos % 3 == 2 && dir == -1) {
            //left OOB
            return true;
        }
        if (selectedSquaresScript.isSquareSelected(newPos) && selectedPositions.LastOrDefault() != newPos) {
            //already selected, selectedPositions[0] won't be 0
            return true;
        }
        return false;
    }
    private void moveSelectSquare(int dir)
    {
        int newPos = currentPos + dir;

        if (isInvalidMovement(newPos, dir)) {
            return;
        } 
        if (selectState)
        {
            trySelect(newPos);

        }
        updatePos(newPos);
    }

    //START THE GAME (COROUTINE)
    private IEnumerator startGameCoroutine(float timerLength) {
        //set timer so player can check the time
        timerScript.setTime(timerLength);

        //clear the score
        scoreScript.clearScore();

        //show press space to start text
        introScript.showPressToStartText();
        //wait until space is pressed
        yield return new WaitUntil(() => Input.GetKeyDown(spaceKey));
        introScript.hidePressToStartText();
        
        //start countdown
        introScript.showCountdownText();
        for (int i = 0 ; i < 4 ; ++i) {
            introScript.updateTextByIndex(i);
            yield return new WaitForSeconds(1);
        }
        introScript.clearText();

        //set everything
        goalScript.showText();
        gridNumbersScript.showNumbers();
        remainingScript.showText();
        scoreScript.showText();
        functionWordScript.updateText("", 0);

        //new problem
        updatePos(currentPos);
        //debug
        //Debug.Log("before new problem");
        randomizerScript.newProblem();
        //start the timer
        timerScript.startTimer();

        //set game to active
        isGameActive = true;
        isIntroDone = true;
        isIntroRunning = false;
    }
    
    //END THE GAME
    private IEnumerator EndGame() {
        Debug.Log("Game Ended");

        //game is now inactive
        isGameActive = false;

        //return score to static variables
        Static_Variables.lastGameScore = scoreScript.getScore();

        //hide the numbers and show time up text
        functionWordScript.clearText();
        gridNumbersScript.hideNumbers();
        outroScript.showTimesUpText();
        
        //after a second, show press space to return text
        yield return new WaitForSeconds(1);
        outroScript.showPressToReturnText();

        //wait for space press
        yield return new WaitUntil(() => Input.GetKeyDown(spaceKey));
        isOutroRunning = false;

        //switch scenes
        Debug.Log("lastGameScore: " + Static_Variables.lastGameScore.ToString());
        SceneManager.LoadScene("Dialogue Main");
    }
    
    private void Awake() {
        selectState = false;
        upKey = KeyCode.UpArrow;
        downKey = KeyCode.DownArrow;
        leftKey = KeyCode.LeftArrow;
        rightKey = KeyCode.RightArrow;
        selectKey = KeyCode.Z;
        resetKey = KeyCode.R;
        newQuestionKey = KeyCode.Space;
        spaceKey = KeyCode.Space;
        unitVectorX = squareLength * Vector3.right;
        unitVectorY = squareLength * Vector3.up;
        currentPos = 4;
        //lastPos = -1;
        //squareLength = 240;

        gridNumbersScript = gameObject.transform.parent.Find("Background/Grid Numbers").GetComponent<Grid_Numbers_Script>();
        selectedSquaresScript = gameObject.transform.parent.Find("Selected Squares").GetComponent<Selected_Squares_Script>();
        goalScript = gameObject.transform.parent.Find("Background/Goal").GetComponent<Goal_Script>();
        functionWordScript = gameObject.transform.parent.Find("Background/Function Word").GetComponent<Function_Word_Script> ();
        remainingScript = gameObject.transform.parent.Find("Background/Remaining").GetComponent<Remaining_Script>();
        randomizerScript = gameObject.transform.parent.Find("Randomizer").GetComponent<Randomizer_Script>();
        scoreScript = gameObject.transform.parent.Find("Background/Score").GetComponent<Score_Script>();
        timerScript = gameObject.transform.parent.Find("Background/Timer").GetComponent<Timer_Script>();
        introScript = gameObject.transform.parent.Find("Transitions/Intro").GetComponent<Intro_Script>();
        outroScript = gameObject.transform.parent.Find("Transitions/Outro").GetComponent<Outro_Script>();

    }

    private void Start()
    {
        isGameActive = false;
        isIntroDone = false;
        isIntroRunning = false;
        isTimeUp = false;
        isOutroRunning = false;
        initialTimerSecs = Static_Variables.levelQuestionParameters[Static_Variables.level].getTime();
        //startGame(60f);
        /*
        updatePos(currentPos);
        randomizerScript.newProblem();
        */
    }
    private void Update()
    {
        //playing intro
        if (!isIntroDone) {
            if (!isIntroRunning) {
                isIntroRunning = true;
                StartCoroutine(startGameCoroutine(initialTimerSecs));
            }
            return;
        }
        if (isTimeUp) {
            if (!isOutroRunning) {
                isOutroRunning = true;
                StartCoroutine(EndGame()); //scene should switch here
            }
            return;
        }
        //in game
        if (isGameActive) {
            if (timerScript.isTimeOver()) {
                isTimeUp = true;
            }
            else {
                int newDirection = directionHandler();
                if (Input.GetKeyDown(resetKey)) {
                    randomizerScript.resetProblem();
                }
                if (Input.GetKeyDown(newQuestionKey)) {
                    randomizerScript.newProblem();
                }

                if (Input.GetKeyDown(selectKey)) {
                    selectPress();
                }
                if (Input.GetKeyUp(selectKey) && selectState) {
                    selectRelease();
                }

                if (newDirection != 0)
                {
                    //Debug.Log(newDirection);
                    moveSelectSquare(newDirection);
                }
            }
        }
    }
}
