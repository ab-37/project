using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;

public class BG_Script : MonoBehaviour
{
    private List<Transform> bgs = new List<Transform>();
    private string currentBg;

    //get bg's transform
    private GameObject getBgObject(string name) 
    => bgs.Find(bg => bg.gameObject.name == name).gameObject;

    //set bg (hide old bg, show new bg)
    public void setBg(string newBg) {
        if (currentBg == newBg) {
            return;
        }
        getBgObject(newBg).SetActive(true);
        getBgObject(currentBg).SetActive(false);
        currentBg = newBg;
    }

    private void Awake() {
        foreach (Transform bg in gameObject.transform.parent.Find("bg")) {
            bgs.Add(bg);
            bg.gameObject.SetActive(false);
        }
        currentBg = "1";
        getBgObject("1").SetActive(true);
    }

    private void Start() {
        
    }

    private void Update() {
        
    }
}
