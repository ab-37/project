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

using System.Collections;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using UnityEngine.Rendering;
using UnityEngine.Networking;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine.Analytics;
//using static System.Net.Mime.MediaTypeNames;
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
    public GameObject CheckPanel;
    //public GameObject CheckOKPanel;
    public GameObject LoadPanel;
    public GameObject Loadimage;
    public Text Loadtxt;

    bool isAIReady = false;




    void Start()
    {
        //GameObject obj = GameObject.Find("LoadAI");
        //if (obj != null)
        //{
        //    test = obj.GetComponent<LoadAI>();
        //}

        test = FindObjectOfType<LoadAI>();
        if (test == null)
        {
            Debug.LogError("Component not found!");
        }
        Loadimage.transform.localScale = Vector3.one;

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
        Time.timeScale = 2;

    }
    private async void StartAI()
    {
        if (test != null)
        {

            ShowLoad();
            StartCoroutine(DisplayLoading());
            //test.StartAI();
            Static_Variables.dialogueMode = 2;
            await Task.Run(() =>
            {
                test.StartAI();
            });
            isAIReady = true;
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
        ChoosePanel.gameObject.SetActive(false);
        CheckPanel.gameObject.SetActive(false);
        LoadPanel.gameObject.SetActive(true);

    }
    private void NormalDia()
    {
        Static_Variables.dialogueMode = 0;
        test.NormalJson();
    }
    private void LastDia()
    {
        Static_Variables.dialogueMode = 2;
        test.LastJson();
    }
    private IEnumerator DisplayLoading()
    {
        float totalDuration = 7200f;
        float currentTime = 0f;

        //AsyncOperation async = SceneManager.LoadSceneAsync(ScreenNum);
        //async.allowSceneActivation = false;

        while (currentTime < totalDuration)
        {
            currentTime += Time.deltaTime;
            float progress = Mathf.Clamp01(currentTime / totalDuration);
            if (isAIReady)
            {
                Loadtxt.color = Color.green;

            }
            SetLoading(progress * 100f);
            yield return null;
        }

        SetLoading(100f);
        yield return new WaitForSeconds(0.5f);
        //if (isAIReady)
        //{
        //    SetLoading(100f);
        //    yield return new WaitForSeconds(0.5f);
        //    Static_Variables.dialogueMode = 2;
        //    //async.allowSceneActivation = true;
        //}

    }
    private void SetLoading(float percent)
    {
        if (!isAIReady && percent >= 100f)
        {
            Loadtxt.text = "99%";
        }
        else
        {
            Loadimage.transform.localScale = new Vector3((percent * 0.01f), 1, 1);
            Loadtxt.text = Mathf.RoundToInt(percent).ToString() + "%";
        }
    }
}
