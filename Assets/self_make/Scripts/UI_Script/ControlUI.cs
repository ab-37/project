using LLMUnitySamples;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Fungus;
using UnityEngine.SceneManagement;
using LitJson;
using System.IO;

public class ControlUI : MonoBehaviour
{
    public Flowchart flowchart;
    public string blockSelect;

    private string LoadData;
    private JsonData StageData;

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
    
    private JsonData selcetStage(string chosen_id)
    {
        for(int i=0;i< StageData["stage"].Count;i++)
        {
            if(chosen_id == StageData["stage"][i]["id"].ToString())
                return StageData["stage"][i];
        }
        return null;
    }

    private void changeDialuage()
    {
        flowchart.SetStringVariable("VarDialogue", "123") ;
    }
    /*private void findBlockText(string act_number)
    {
        for (int i = 0; i < StageData["stage"].Count; i++)
        {
            if (chosen_id == StageData["stage"][i]["id"].ToString())
                return StageData["stage"][i];
        }
        return null;

    }*/

    void Start()
    {
        LoadData = File.ReadAllText(Application.dataPath + "/self_make/stage/stage.json");
        StageData = JsonMapper.ToObject(LoadData);

        blockSelect = Static_Variables.blockSelect;
        mainMenu = GameObject.Find("MainUI");


        //Debug.Log(StageData["stage"][1]["next_stage"]);
        Debug.Log(selcetStage("11")["next_stage"]);

        changeDialuage();

        /*
        if (blockSelect != "0")
            playBlockStage(blockSelect);
        else
            Debug.Log("no block select");
        */

    }

    void Update()
    {
        
    }
}
