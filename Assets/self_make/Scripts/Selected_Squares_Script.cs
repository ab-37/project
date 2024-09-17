using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Selected_Squares_Script : MonoBehaviour
{
    private GameObject[] tileObject = new GameObject[9];
    private bool[] isSelected = new bool[9];

    public bool isSquareSelected(int square) {
        return isSelected[square];
    }
    private void updateSquare(int square) {
        tileObject[square].SetActive(isSelected[square]);
    }
    public void deselectSquare(int square) {
        isSelected[square] = false;
        updateSquare(square);
    }
    public void deselectAllSquares() {
        for (int i = 0 ; i < 9 ; ++i) {
            deselectSquare(i);
        }
    }
    public void selectSquare(int square) {
        isSelected[square] = true;
        updateSquare(square);
    }
    private void Start()
    {
        for (int i = 0 ; i < 9 ; ++i) {
            //get child objects
            tileObject[i] = gameObject.transform.GetChild(i).gameObject;
        }
        deselectAllSquares();
    }

    private void Update()
    {
        
    }
}
