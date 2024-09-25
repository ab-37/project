using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Function_Word_Script : MonoBehaviour
{

    private Selected_Squares_Script SelectedSquaresScript;

    private float AutoSetSize(TextMeshProUGUI textMesh, string word)
    {
        return textMesh.fontSize - word.Length * textMesh.fontSize/100;
    }

    public void UpdateWordText(string expression)
    {
        
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

                    //textMesh.text = ""+result;
                    break;
            }
        }
    }
    void Start()
    {
      
    }

    void Update()
    {
        
    }
}
