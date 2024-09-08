using System.Collections;
using System.Collections.Generic;
using TMPro;  
using UnityEngine;
using UnityEngine.UI;

public class Grid_Number_Script : MonoBehaviour
{
    private int[] gridNumber = { 1, 3, 5, 7, 9 };
    private char[] gridSign = { '+', '-', '*', '/' };

    public GameObject gridFather;

    void AssignValuesToGrid()
    {
        foreach (Transform child in gridFather.transform)
        {
            string childName = child.name;
            TextMeshProUGUI textMesh = child.GetComponent<TextMeshProUGUI>();

            if (textMesh != null)
            {
                switch (childName)
                {
                    case "1":
                        textMesh.text = "" + gridNumber[0];
                        break;
                    case "2":
                        textMesh.text = "" + gridSign[0];
                        break;
                    case "3":
                        textMesh.text = "" + gridNumber[1];
                        break;
                    case "4":
                        textMesh.text = "" + gridSign[1];
                        break;
                    case "5":
                        textMesh.text = "" + gridNumber[2];
                        break;
                    case "6":
                        textMesh.text = "" + gridSign[2];
                        break;
                    case "7":
                        textMesh.text = "" + gridNumber[3];
                        break;
                    case "8":
                        textMesh.text = "" + gridSign[3];
                        break;
                    case "9":
                        textMesh.text = "" + gridNumber[4];
                        break;
                }
            }
        }
    }

    void Start()
    {
        gridFather = this.gameObject;
        AssignValuesToGrid();
    }

    void Update()
    {

    }
}
