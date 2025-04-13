using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;

public class BG_Script : MonoBehaviour
{
    private List<Transform> bgs = new List<Transform>();
    private string currentBg = "";

    //get bg's transform
    private GameObject getBgObject(string name) 
    => bgs.Find(bg => bg.gameObject.name == name).gameObject;

    //set bg (hide old bg, show new bg)
    public void setBg(string newBg) {
        //Debug.Log("Static_Variables.currentBg = " + Static_Variables.currentBg);
        //Debug.Log("currentBg = " + currentBg);
        if (currentBg != "") {
            getBgObject(currentBg).SetActive(false);
        }
        getBgObject(newBg).SetActive(true);
        currentBg = newBg;
        Static_Variables.currentBg = newBg;
    }
    /*
    private void forceSetBg(string newBg) {
        getBgObject(newBg).SetActive(true);
        currentBg = newBg;
        Static_Variables.currentBg = newBg;
    }
    */

    private void Awake() {
        //Debug.Log("Awake");
        foreach (Transform bg in gameObject.transform.parent.Find("bg")) {
            bgs.Add(bg);
            bg.gameObject.SetActive(false);
        }
        /*
        Static_Variables.currentBg = "1";
        currentBg = "1";
        getBgObject("1").SetActive(true);
        */
    }

    private void Start() {
        //Debug.Log("Start");
        setBg(Static_Variables.currentBg);
    }

    private void Update() {
        
    }
}
