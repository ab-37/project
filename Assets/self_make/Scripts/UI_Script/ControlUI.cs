using LLMUnitySamples;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Fungus;

public class ControlUI : MonoBehaviour
{

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
            case 0:
                //Fungus.Flowchart.BroadcastFungusMessage("�Ĥ@���o�ͨƥ�");
                break;
            case 1:
                //Fungus.Flowchart.BroadcastFungusMessage("�Ĥ@���o�ͨƥ�(2)");
                break;
            case 2:
               // Fungus.Flowchart.BroadcastFungusMessage("�Ĥ@���o�ͨƥ�(3)");
                break;
        }
    }

    void Start()
    {
        mainMenu = GameObject.Find("MainUI");
    }

    void Update()
    {
        
    }
}
