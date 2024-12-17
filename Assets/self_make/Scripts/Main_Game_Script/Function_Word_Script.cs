using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Function_Word_Script : MonoBehaviour
{

    //private Selected_Squares_Script SelectedSquaresScript;
    private TextMeshProUGUI expressionTextMesh;
    private TextMeshProUGUI resultTextMesh;

    /*
    private float AutoSetSize(TextMeshProUGUI textMesh, string word)
    {
        return textMesh.fontSize - word.Length * textMesh.fontSize/100;
    }
    */

    public void updateText(string expression, int result)
    {
        /*
        foreach (Transform child in gameObject.transform)
        {
            int childNameInt = int.Parse(child.name);
            TextMeshProUGUI textMesh = child.GetComponent<TextMeshProUGUI>();


            switch (childNameInt)
            {
                case 0:
                    textMesh.text = expression;
                    textMesh.fontSize = AutoSetSize(textMesh, expression);
                    break;
                case 1:

                    textMesh.text = result.ToString();
                    break;
            }
        }
        */
        expressionTextMesh.text = expression;
        //expressionTextMesh.fontSize = AutoSetSize(expressionTextMesh, expression);
        resultTextMesh.text = result.ToString();
    }

    public void clearText() {
        expressionTextMesh.text = "";
        resultTextMesh.text = "";
    }

    private void Awake() {
        expressionTextMesh = gameObject.transform.Find("Expression").GetComponent<TextMeshProUGUI>();
        resultTextMesh = gameObject.transform.Find("Result").GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        //UpdateWordText("", 0);
        clearText();
    }

    private void Update()
    {
        
    }
}
