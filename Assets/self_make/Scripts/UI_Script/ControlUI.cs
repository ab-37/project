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

    //the full dialogue data
    private JsonData fullDialogueData;
    
    //the current loaded node's dialogue data
    private JsonData currentNodeDialogue;

    private GameObject mainMenu;
    public void hideMainUI()
    {
        mainMenu.SetActive(false);
    }
    public void showMainUI()
    {
        mainMenu.SetActive(true);
    }

    public void chooseStage(int value)
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
    public void changeScene(string sceneName,string blockRunning)
    {
        SceneManager.LoadScene(sceneName);
        Static_Variables.blockRunning = blockRunning;
    }
    public void gamePlay()
    {
        SceneManager.LoadScene("Gameplay Main");
    }


    private void playBlockStage(string blockName)
    {
        flowchart.ExecuteBlock(blockName);
    }
    
    private JsonData selectStage(string chosen_id)
    {
        for(int i=0;i< StageData["stage"].Count;i++)
        {
            if(chosen_id == StageData["stage"][i]["id"].ToString())
                return StageData["stage"][i];
        }
        return null;
    }

    private void changeDialogueTest()
    {
        flowchart.SetStringVariable("VarDialogue", "123") ;
    }
    private void ExecuteDialogurText(string character,string Dialogue)
    {
        flowchart.SetStringVariable("VarDialogue", Dialogue);
        switch (character)
        {
            case "Kate":
                flowchart.ExecuteBlock("KateSaying");
                break;
            case "Eve":
                flowchart.ExecuteBlock("EveSaying");
                break;
            case "MysteriousMan":
                flowchart.ExecuteBlock("MysteriousManSaying");
                break;
            case "Back":
                flowchart.ExecuteBlock("BackSaying");
                break;
            case "Commonder":
                flowchart.ExecuteBlock("CommonderSaying");
                break;
            case "Victor":
                flowchart.ExecuteBlock("VictorSaying");
                break;
        }
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

    //load act node
    private bool loadNode(int act, int node) {

        //fetch version
        JsonData versionJson = null;
        bool successFlag = false;
        foreach (JsonData actJson in fullDialogueData["acts"]) {
            if ((int)actJson["version"] == act) {
                versionJson = actJson;
                successFlag = true;
                break;
            }
        }
        if (!successFlag) {
            Debug.Log("Failed to fetch version");
            return false;
        }

        //temp code
        Debug.Log("Scene Description: " + versionJson["scene_description"]);

        //fetch node
        foreach (JsonData contentJson in versionJson["content"]) {
            if ((int)contentJson["node"] == node) {
                currentNodeDialogue = contentJson["dialogues"];
                return true;
            }
        }
        Debug.Log("Failed to fetch node in version " + act.ToString());
        return false;
    }

    //load single dialogue line
    bool loadDialogueLine(int lineNum) {
        if (lineNum >= currentNodeDialogue.Count) {
            return false;
        }
        
        string characterStr = (string)currentNodeDialogue[lineNum]["character"];
        string lineStr = (string)currentNodeDialogue[lineNum]["line"];

        //debug code below, put real code here
        Debug.Log(characterStr + ": " + lineStr);
        
        return true;
    }

    void Start()
    {
        LoadData = File.ReadAllText(Application.dataPath + "/self_make/stage/stage.json");
        StageData = JsonMapper.ToObject(LoadData);

        blockSelect = Static_Variables.blockSelect;
        mainMenu = GameObject.Find("MainUI");

        //load dialogue, temp file path
        fullDialogueData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/self_make/Scripts/NPC_script/acts1.json"));

        if (loadNode(1, 1)) {
            Debug.Log("Dialogue loaded successfully");
        }

        //debug checking line by line
        for (int i = 0 ; loadDialogueLine(i) ; ++i) {}

        //Debug.Log(currentNodeDialogue[0]["character"]); //debug



        //Debug.Log(StageData["stage"][1]["next_stage"]);
        //Debug.Log(selectStage("11")["next_stage"]);

        changeDialogueTest();

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
