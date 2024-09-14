using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Select_Square : MonoBehaviour
{
    bool selectState; //if Z is pressed
    private KeyCode upKey;
    private KeyCode downKey;
    private KeyCode leftKey;
    private KeyCode rightKey;
    private KeyCode selectKey;
    private int currentPos;
    //private int lastPos;
    private Vector3 unitVectorX;
    private Vector3 unitVectorY;
    private Vector3 realPos;
    public Vector3 gridOrigin;
    public int squareLength; //length of the side of one square
    private List<int> selectedPositions = new List<int>();

    private Grid_Number_Script gridNumber;
    private Selected_Squares selectedSquares;

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
    private int calculateResult() {
        //pos to number and sign
        List<int> numbers = new List<int>();
        List<string> signs = new List<string>();
        int i;
        foreach (int pos in selectedPositions) {
            if (pos % 2 == 0) {
                //number
                //i is used as a temp variable
                if (int.TryParse(gridNumber.getGridContent(pos), out i)) {
                    numbers.Add(i);
                }
                else {
                    Debug.Log("Error: Parsing int failed");
                }
            }
            else {
                //sign
                signs.Add(gridNumber.getGridContent(pos));
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
                numbers[i] /= numbers[i + 1];
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
            outputString += gridNumber.getGridContent(pos);
            outputString += " ";
        }
        return outputString;
    }

    //selecting or deselecting a square
    private void trySelect(int newPos) {
        if (selectedSquares.isSquareSelected(newPos)) {
            //backtrack
            selectedSquares.deselectSquare(currentPos);
            selectedPositions.RemoveAt(selectedPositions.Count() - 1);
            return;
        }
        //add previous cell
        selectedPositions.Add(currentPos);
        selectedSquares.selectSquare(newPos);
    }

    //press or release select
    private void selectPress()
    {
        //Debug.Log("Select"); //debug code
        if (currentPos % 2 == 1) {
            //selected a sign
            Debug.Log("Select Failed");
            return;
        }
        selectState = true;
        selectedSquares.selectSquare(currentPos);
    }
    private void selectRelease()
    {
        //Debug.Log("Deselect"); //debug code
        if (currentPos % 2 == 1 && selectedPositions.Count() == 0) {
            //selected a sign
            return;
        }

        selectedPositions.Add(currentPos); //add last cell
        //int result = calculateResult(); //may be used in the future
        
        //debug code
        Debug.Log("Expression: " + expressionToString() + " = " + calculateResult());

        selectState = false;
        selectedSquares.deselectAllSquares();
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
        if (selectedSquares.isSquareSelected(newPos) && selectedPositions.LastOrDefault() != newPos) {
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
        unitVectorX = squareLength * Vector3.right;
        unitVectorY = squareLength * Vector3.up;
        currentPos = 4;
        //lastPos = -1;
        //squareLength = 240;
        updatePos(currentPos);

        gridNumber = gameObject.transform.parent.Find("Background/Grid Numbers").gameObject.GetComponent<Grid_Number_Script>();
        selectedSquares = gameObject.transform.parent.Find("Selected Squares").gameObject.GetComponent<Selected_Squares>();

    }
    private void Update()
    {
        int newDirection = directionHandler();

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
