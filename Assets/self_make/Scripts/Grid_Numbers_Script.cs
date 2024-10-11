using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Grid_Numbers_Script : MonoBehaviour
{
    
    /*
    private int[] gridNumber = { 1, 3, 5, 7, 9 };
    private char[] gridSign = { '+', '-', '*', '/' };
    */
    private string[] originalContent = {"4", "/", "5", "-", "3", "*", "1", "+", "2"};
    private string[] gridContent = new string[9];

    //private GameObject gridFather;
    public string getGridContent(int square) {
        return gridContent[square];
    }
    private void updateGrid()
    {
        foreach (Transform child in gameObject.transform)
        {
            //string childName = child.name;
            int childNameInt = int.Parse(child.name);
            TextMeshProUGUI textMesh = child.GetComponent<TextMeshProUGUI>();

            if (textMesh != null)
            {
                textMesh.text = gridContent[childNameInt];
            }
        }
    }

    public void resetGrid() {
        for (int i = 0 ; i < 9 ; ++i) {
            gridContent[i] = originalContent[i];
        }
        updateGrid();
    }

    public void setGridContent(int pos, string value) {
        gridContent[pos] = value;
        updateGrid();
    }
    
    private void Awake() {
        
    }
    private void Start()
    {
        resetGrid();
    }

    private void Update()
    {

    }
}
