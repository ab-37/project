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
    private Vector2Int currentPos;
    private Vector3 realPos;
    public Vector3 gridOrigin;
    public int squareLength; //length of the side of one square

    //key input handlers
    private Vector2Int directionHandler()
    {
        Vector2Int dir = new Vector2Int(0, 0);
        if (Input.GetKeyDown(upKey))
        {
            dir.y = 1;
        }
        else if (Input.GetKeyDown(downKey))
        {
            dir.y = -1;
        }
        else if (Input.GetKeyDown(leftKey))
        {
            dir.x = -1;
        }
        else if (Input.GetKeyDown(rightKey))
        {
            dir.x = 1;
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
    private void updatePos(Vector2Int newPos)
    {
        currentPos = newPos;
        realPos = ((Vector3)(Vector2)(currentPos * squareLength)) + gridOrigin;
        transform.position = realPos;
        Debug.Log(currentPos);
    }
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
    private void moveSelectSquare(Vector2Int dir)
    {

        Vector2Int newPos = currentPos + dir;
        //Debug.Log(newPos);
        newPos = fixPosOOB(newPos);
        //Debug.Log(newPos);
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
        currentPos = Vector2Int.zero;
        //squareLength = 240;
        updatePos(Vector2Int.zero);

    }
    private void Update()
    {
        Vector2Int newDirection = directionHandler();

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
        if (newDirection != Vector2Int.zero)
        {
            //Debug.Log(newDirection);
            moveSelectSquare(newDirection);
        }
    }
}
