using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound_Script : MonoBehaviour
{
    private AudioSource audioSource;

    public void playSound() {
        audioSource.PlayDelayed(0);
    }

    private void Awake() {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void Start() {
        
    }

    private void Update() {

    }
}
