using System.Collections;
using System.Collections.Generic;
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
    private Vector3 unitVectorX;
    private Vector3 unitVectorY;
    private Vector3 realPos;
    public Vector3 gridOrigin;
    public int squareLength; //length of the side of one square

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
    /*
    //obsolete, doesn't work outside Update() function
    private int selectKeyHandler()
    {
        if (Input.GetKeyDown(selectKey))
        {
            return 1; //down
        }
        if (Input.GetKeyUp(selectKey))
        {
            return 0; //up
        }
        return -1;
    }
    */

    //selecting and deselecting
    private void select()
    {
        Debug.Log("Selected"); //placeholder code
    }
    private void deselect()
    {
        Debug.Log("Deselected"); //placeholder code
    }

    //moving square
    private void updatePos(int newPos)
    {
        currentPos = newPos;
        realPos = gridOrigin + ((currentPos % 3) * unitVectorX) + ((currentPos / 3) * unitVectorY);
        transform.position = realPos;
        Debug.Log(currentPos);
    }
    /*
    private Vector2Int fixPosOOB(Vector2Int pos) //out of bounds
    {
        Vector2Int newPos = pos;
        if (newPos.x > 1)
        {
            newPos.x = 1;
        }
        else if (newPos.x < -1)
        {
            newPos.x = -1;
        }
        if (newPos.y > 1)
        {
            newPos.y = 1;
        }
        else if (newPos.y < -1)
        {
            newPos.y = -1;
        }
        //Debug.Log(newPos);
        return newPos;
    }
    */

    private bool isInvalidMovement(int newPos, int dir) {
        if (newPos > 8 || newPos < 0) {
            return true;
        }
        if (newPos % 3 == 0 && dir == 1) {
            return true;
        }
        if (newPos % 3 == 2 && dir == -1) {
            return true;
        }
        return false;
    }
    private void moveSelectSquare(int dir)
    {
        int newPos = currentPos + dir;
        //Debug.Log(newPos);
        //newPos = fixPosOOB(newPos);
        //Debug.Log(newPos);
        if (isInvalidMovement(newPos, dir)) {
            return;
        } 
        if (selectState)
        {
            Debug.Log("do select stuff");
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
        //squareLength = 240;
        updatePos(currentPos);

    }
    private void Update()
    {
        int newDirection = directionHandler();

        //handle select key
        bool newSelectState = Input.GetKey(selectKey);
        if (newSelectState && !selectState)
        {
            select();
        }
        else if (!newSelectState && selectState)
        {
            deselect();
        }
        selectState = newSelectState;
        if (newDirection != 0)
        {
            //Debug.Log(newDirection);
            moveSelectSquare(newDirection);
        }
    }
}
