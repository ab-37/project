using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Grid_Number_Script : MonoBehaviour
{
    
    /*
    private int[] gridNumber = { 1, 3, 5, 7, 9 };
    private char[] gridSign = { '+', '-', '*', '/' };
    */
    private string[] gridContent = {"1", "+", "3", "-", "5", "*", "7", "/", "9"};

    //private GameObject gridFather;
    public string getGridContent(int square) {
        return gridContent[square];
    }
    private void AssignValuesToGrid()
    {
        foreach (Transform child in gameObject.transform)
        {
            string childName = child.name;
            int childNameInt = int.Parse(childName);
            TextMeshProUGUI textMesh = child.GetComponent<TextMeshProUGUI>();

            if (textMesh != null)
            {
                /*
                if (childNameInt % 2 == 0) {
                    //number
                    textMesh.text = "" + gridNumber[childNameInt / 2];
                }
                else {
                    //sign
                    textMesh.text = "" + gridSign[childNameInt / 2];
                }
                */
                textMesh.text = gridContent[childNameInt];
            }
        }
    }

    private void Start()
    {
        //gridFather = this.gameObject;
        AssignValuesToGrid();
    }

    private void Update()
    {

    }
}
