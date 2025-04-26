using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Skip_Handler_Script : MonoBehaviour
{
    //keys
    private static KeyCode[] skipKeys = new KeyCode[] {
        KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5
    };

    //where to jump
    private static (int, int)[] actJump = new (int, int)[] {
        (0, 1), (0, 1), (1, 5), (2, 4), (3, 4), (4, 4)
    };

    //time
    private float time;

    //current active key
    private int activeKey;

    //key press counter
    private int keyCounter;

    private void Awake() {
        
    }

    private void Start() {
        activeKey = -1;
        keyCounter = 0;
    }

    private void Update() {
        time -= Time.deltaTime;
        for (int i = 0 ; i < 6 ; ++i) {
            if (Input.GetKeyDown(skipKeys[i])) {
                Debug.Log(i.ToString() + " is pressed");
                if (time < 0 || activeKey != i) {
                    //new key
                    activeKey = i;
                    keyCounter = 1;
                    time = 1f;
                    return;
                }
                //previous key
                ++keyCounter;
                time = 1f;
                Debug.Log("Press #" + keyCounter.ToString());
                if (keyCounter >= 3) {
                    //jump
                    keyCounter = 0; //failsafe
                    Static_Variables.currentAct = actJump[i].Item1;
                    Static_Variables.currentPart = actJump[i].Item2;
                    if (i == 0) {
                        SceneManager.LoadScene("NPC");
                        return;
                    }
                    SceneManager.LoadScene("Dialogue Main");
                    return;
                }
            }
        }
    }
}
