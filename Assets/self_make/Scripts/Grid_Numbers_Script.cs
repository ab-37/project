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
    //static private char[] choseSign = { '+', '-', '*', '/' };
    private string[] originalContent = {"4", "/", "5", "-", "3", "*", "1", "+", "2"};
    private string[] gridContent = new string[9];

    /*
    private int steps;
    private int step;
    private int target;//wait for produce
    private int[] numbers=new int[5];
    private char[] signs= new char[4];
    */

    //private GameObject gridFather;

    //hide and show numbers
    public void hideNumbers() {
        foreach (Transform child in gameObject.transform) {
            child.gameObject.SetActive(false);
        }
    }
    public void showNumbers() {
        foreach (Transform child in gameObject.transform) {
            child.gameObject.SetActive(true);
        }
        //updateGrid();
    }

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
    /*
    //restet signs,step,steps,numbers
    private void resetNewNumber()
    {
        steps = Random.Range(1,2);
        step = Random.Range(1,3)*3;

        foreach(int i in numbers)
            numbers[i]=Random.Range(1,9);

        foreach (int i in signs)
            signs[i] = choseSign[Random.Range(0, 3)];
    }

    //undateNewNumber to originalContent
    private void updateNewNumber()
    {
        for (int i=0;i<9;i++)
        {
            if (i % 2 == 0)
                originalContent[i] = numbers[i - i / 2].ToString();
            else
                originalContent[i] = signs[i - i / 2 - 1].ToString();
        }
    }
    */

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

    public void setOriginalContent(string[] grid) {
        for (int i = 0 ; i < 9 ; ++i) {
            originalContent[i] = grid[i];
        }
        resetGrid();
    }
    
    private void Awake() {
        
    }
    private void Start()
    {
        //resetGrid();
        hideNumbers();
    }

    private void Update()
    {

    }
}
