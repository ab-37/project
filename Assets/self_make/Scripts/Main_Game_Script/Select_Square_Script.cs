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
    private bool isLevelEnded; //true if the level is ended
    private bool isOutroRunning; //true if the outro coroutine is running
    private int levelMode; //level mode, 1 = countdown, 2 = point requirement
    private int objective; //objective

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
            //countup level score check
            if (levelMode == 2) {
                if (scoreScript.getScore() >= objective) {
                    isLevelEnded = true;
                    timerScript.stopTimer();
                    goto resetEverything;
                }
            }
            randomizerScript.newProblem();
        }
        else {
            gridNumbersScript.setGridContent(realUpdatePos, result.ToString());
            //moves - 1
        }

        resetEverything:
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
    private IEnumerator startGameCoroutine() {
        //set the timer 
        timerScript.setTimerMode(levelMode == 1);
        timerScript.setTime(0);
        if (levelMode == 1) {
            timerScript.setTime(objective);
        }

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

        //return score or time to static variables (obsolete, now check for countup levels)
        if (levelMode == 1) {
            Static_Variables.lastGameScore = scoreScript.getScore();
            Debug.Log("lastGameScore: " + Static_Variables.lastGameScore.ToString());
        }
        else {
            Static_Variables.lastGameTime = timerScript.getTime();
            Debug.Log("lastGameTime: " + Static_Variables.lastGameTime.ToString());
        }

        //hide the numbers and show time up text
        functionWordScript.clearText();
        gridNumbersScript.hideNumbers();
        outroScript.showLevelEndedText();
        
        //after a second, show press space to return text
        yield return new WaitForSeconds(1);
        outroScript.showPressToReturnText();

        //wait for space press
        yield return new WaitUntil(() => Input.GetKeyDown(spaceKey));
        isOutroRunning = false;

        //switch scenes
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
        isLevelEnded = false;
        isOutroRunning = false;

        levelMode = Static_Variables.levelQuestionParameters[Static_Variables.level].getLevelMode();
        objective = Static_Variables.levelQuestionParameters[Static_Variables.level].getObjective();
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
                StartCoroutine(startGameCoroutine());
            }
            return;
        }
        if (isLevelEnded) {
            if (!isOutroRunning) {
                isOutroRunning = true;
                StartCoroutine(EndGame()); //scene should switch here
            }
            return;
        }
        //in game
        if (isGameActive) {
            //game end check
            if (levelMode == 1) {
                if (timerScript.isTimeOver()) {
                    isLevelEnded = true;
                    return;
                }
            }
            int newDirection = directionHandler();
            if (Input.GetKeyDown(resetKey)) {
                randomizerScript.resetProblem();
            }
            if (Input.GetKeyDown(newQuestionKey)) {
                randomizerScript.newProblem(true);
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
