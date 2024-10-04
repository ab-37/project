using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;

public class Select_Square_Script : MonoBehaviour
{
    bool selectState; //if Z is pressed
    private KeyCode upKey;
    private KeyCode downKey;
    private KeyCode leftKey;
    private KeyCode rightKey;
    private KeyCode selectKey;
    private KeyCode resetKey;
    private int currentPos;
    //private int lastPos;
    private Vector3 unitVectorX;
    private Vector3 unitVectorY;
    private Vector3 realPos;
    public Vector3 gridOrigin;
    public int squareLength; //length of the side of one square
    private List<int> selectedPositions = new List<int>();

    private Grid_Numbers_Script gridNumbersScript;
    private Selected_Squares_Script selectedSquaresScript;
    private Goal_Script goalScript;
    private Function_Word_Script functionWordScript;
    private Remaining_Script remainingScript;

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

    //calculate result
    private int calculateResult(string expression) {
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
                //legacy code
                /*
                if (int.TryParse(gridNumbersScript.getGridContent(pos), out i)) {
                    numbers.Add(i);
                }
                else {
                    Debug.Log("Error: Parsing int failed");
                }
                */
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
        int result = calculateResult(expression);
        functionWordScript.UpdateWordText(expression, result);
    }

    //reset grid by pressing reset
    private void resetGrid() {
        gridNumbersScript.resetGrid();
        goalScript.setGoalNumber(23); //placeholder code, should be a generated number
        remainingScript.setOriginalSteps(3); //placeholder code, should be a generated number
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
        functionWordScript.UpdateWordText(expression, result);

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
        int result = calculateResult(expression);
        //debug code
        Debug.Log("Expression: " + expression + "= " + result);

        functionWordScript.UpdateWordText(expression, result);

        if (goalScript.isGoal(result)) {
            Debug.Log("Correct!");
            resetGrid(); //placeholder code, should be new grid
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
        realPos = gridOrigin + ((currentPos % 3) * unitVectorX) + ((currentPos / 3) * unitVectorY);
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
    


    private void Start()
    {
        selectState = false;
        upKey = KeyCode.UpArrow;
        downKey = KeyCode.DownArrow;
        leftKey = KeyCode.LeftArrow;
        rightKey = KeyCode.RightArrow;
        selectKey = KeyCode.Z;
        resetKey = KeyCode.R;
        unitVectorX = squareLength * Vector3.right;
        unitVectorY = squareLength * Vector3.up;
        currentPos = 4;
        //lastPos = -1;
        //squareLength = 240;
        updatePos(currentPos);

        gridNumbersScript = gameObject.transform.parent.Find("Background/Grid Numbers").gameObject.GetComponent<Grid_Numbers_Script>();
        selectedSquaresScript = gameObject.transform.parent.Find("Selected Squares").gameObject.GetComponent<Selected_Squares_Script>();
        goalScript = gameObject.transform.parent.Find("Background/Goal").gameObject.GetComponent<Goal_Script>();
        functionWordScript = gameObject.transform.parent.Find("Background/Function Word").gameObject.GetComponent<Function_Word_Script> ();
        remainingScript = gameObject.transform.parent.Find("Background/Remaining").gameObject.GetComponent<Remaining_Script>();
    }
    private void Update()
    {
        int newDirection = directionHandler();

        if (Input.GetKeyDown(resetKey)) {
            resetGrid();
        }

        if (Input.GetKeyDown(selectKey)) {
            selectPress();
        }
        if (Input.GetKeyUp(selectKey) && selectState == true) {
            selectRelease();
        }

        if (newDirection != 0)
        {
            //Debug.Log(newDirection);
            moveSelectSquare(newDirection);
        }
    }
}
