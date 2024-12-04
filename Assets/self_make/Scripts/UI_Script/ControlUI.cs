using LLMUnitySamples;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Fungus;
using UnityEngine.SceneManagement;

public class ControlUI : MonoBehaviour
{
    public Flowchart flowchart;
    public string blockSelect;

    private GameObject mainMenu;
    public void hideMainUI()
    {
        mainMenu.SetActive(false);
    }
    public void showMainUI()
    {
        mainMenu.SetActive(true);
    }

    public void choseSatge(int value)
    {
        switch(value)
        {
            case 1:
                flowchart.ExecuteBlock("Stage1-1");
                break;
            case 2:
                flowchart.ExecuteBlock("Stage1-2");
                break;
            case 3:
                flowchart.ExecuteBlock("Stage1-3");
                break;
        }
    }

    //change scence and tell what is stage running
    public void changeScence(string scenceName,string blockRunning)
    {
        SceneManager.LoadScene(scenceName);
        Static_Variables.blockRuning = blockRunning;
    }
    public void gamePlay()
    {
        SceneManager.LoadScene("Gameplay Main");
    }


    private void playBlockStage(string blockName)
    {
        flowchart.ExecuteBlock(blockName);
    }
    
    
    void Start()
    {
        blockSelect = Static_Variables.blockSelect;
        mainMenu = GameObject.Find("MainUI");

        if (blockSelect != null)
            playBlockStage(blockSelect);
        else
            Debug.Log("no block select");

    }

    void Update()
    {
        
    }
}
