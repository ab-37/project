using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound_Handler_Script : MonoBehaviour
{

    private Dictionary<string, Sound_Script> sounds;

    public void playSFX(string soundName) {
        Sound_Script soundScript = sounds[soundName];
        if (soundScript != null) {
            soundScript.playSound();
        }
    }

    private void Awake() {

    }

    private void Start() {
        GameObject childObject;
        sounds = new Dictionary<string, Sound_Script>();
        foreach (Transform child in gameObject.transform) {
            childObject = child.gameObject;
            sounds.Add(childObject.name, childObject.GetComponent<Sound_Script>());
        }
    }

    private void Update() {

    }
}
