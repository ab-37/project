using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;
using LLMUnity;
using System.Threading.Tasks;
using System.Linq;
using System;
using Unity.VisualScripting;
using LLMUnitySamples;
using System.Text.RegularExpressions;
using UnityEditor.U2D.Animation;

public class Ncharact : MonoBehaviour
{

    private LoadAI test;

    [Header("Buttons")]
    public Button start;
    public Button Normal;
    public Button AI;
    public Button Last;
    public Button CheckOK;

    [Header("Panels")]
    public GameObject ChoosePanel;
    //public GameObject NormalPanel;
    //public GameObject AIPanel;
    //public GameObject LastPanel;
    public GameObject CheckPanel;
    //public GameObject CheckOKPanel;
    public GameObject LoadPanel;





    void Start()
    {
        //GameObject obj = GameObject.Find("LoadAI");
        //if (obj != null)
        //{
        //    test = obj.GetComponent<LoadAI>();
        //}
        //if (test == null)
        //{
        //    Debug.LogError("LoadAI component not found!");
        //}
        test = FindObjectOfType<LoadAI>();
        if (test == null)
        {
            Debug.LogError("Component not found!");
        }
        AddListener();
    }

    protected virtual void AddListener()
    {
        start.onClick.AddListener(ShowChoose);
        Normal.onClick.AddListener(NormalDia);
        AI.onClick.AddListener(ShowCheck);
        Last.onClick.AddListener(LastDia);
        CheckOK.onClick.AddListener(StartAI);
    }

    private void Awake()
    {
        Time.timeScale = 1f;   
        
    }
    private void StartAI()
    {
        if (test != null)
        {
            test.StartAI();
        }
        else
        {
            Debug.LogError("Test2_1 instance is null!");
        }
    }
    private void ShowChoose()
    {
        ChoosePanel.gameObject.SetActive(true);
    }

    private void ShowCheck()
    {
        CheckPanel.gameObject.SetActive(true);
    }
    private void ShowLoad()
    {
        LoadPanel.gameObject.SetActive(true);
    }
    private void NormalDia()
    {
        test.NormalJson();
        Static_Variables.dialogueMode = 0;
    }
    private void LastDia()
    {
        test.LastJson();
        Static_Variables.dialogueMode = 2;
    }
    //private void StartGame()
    //{
    //    Time.timeScale = 1f;
    //    start.gameObject.SetActive(false);
    //}
}
