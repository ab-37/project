using System.Collections;
using System.Collections.Generic;
using TMPro;  
using UnityEngine;
using UnityEngine.UI;

public class Grid_Number_Script : MonoBehaviour
{
    private int[] gridNumber = { 1, 3, 5, 7, 9 };
    private char[] gridSign = { '+', '-', '*', '/' };

    private GameObject gridFather;

    private void AssignValuesToGrid()
    {
        foreach (Transform child in gridFather.transform)
        {
            string childName = child.name;
            int childNameInt = int.Parse(childName);
            TextMeshProUGUI textMesh = child.GetComponent<TextMeshProUGUI>();

            if (textMesh != null)
            {
                if (childNameInt % 2 == 0) {
                    //number
                    textMesh.text = "" + gridNumber[childNameInt / 2];
                }
                else {
                    //sign
                    textMesh.text = "" + gridSign[childNameInt / 2];
                }
            }
        }
    }

    private void Start()
    {
        gridFather = this.gameObject;
        AssignValuesToGrid();
    }

    private void Update()
    {

    }
}
