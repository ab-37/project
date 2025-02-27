using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Randomizer_Script : MonoBehaviour
{   
    
    private Problem currentProblem;
    
    //private Select_Square_Script selectSquareScript;
    private Grid_Numbers_Script gridNumbersScript;
    private Goal_Script goalScript;
    private Remaining_Script remainingScript;
    private Score_Script scoreScript;
    private Sound_Handler_Script soundHandlerScript;

    //called when reset key is pressed
    public void resetProblem() {
        if (remainingScript.isNoStepsUsed()) {
            return;
        }
        //subtract 5 from score
        scoreScript.addScore(-5);
        gridNumbersScript.resetGrid();
        remainingScript.resetSteps();
        //play sfx
        soundHandlerScript.playSFX("Reset Question");
    }

    //called when new problem key is pressed
    public void newProblem(bool isManualReset = false) {
        currentProblem = Static_Variables.levelQuestionParameters[Static_Variables.level_id].generateNewProblem();
        
        //placeholder code
        //currentProblem.copy(QP.generateNewProblem());
        //currentProblem = QP.generateNewProblem();
        string[] tempGrid = new string[9];
        for (int i = 0 ; i < 9 ; ++i) {
            tempGrid[i] = currentProblem.getGridContent(i);
        }

        if (isManualReset) {
            //subtract 15 from score
            scoreScript.addScore(-15);
            //play sfx
            soundHandlerScript.playSFX("New Question");
        }
        gridNumbersScript.setOriginalContent(tempGrid);
        goalScript.setGoalNumber(currentProblem.getGoal());
        remainingScript.setOriginalSteps(currentProblem.getSteps());
    }

    private void Awake() {
        
        //selectSquareScript = gameObject.transform.parent.Find("Select Square").gameObject.GetComponent<Select_Square_Script>();
        gridNumbersScript = gameObject.transform.parent.Find("Background/Grid Numbers").gameObject.GetComponent<Grid_Numbers_Script>();
        goalScript = gameObject.transform.parent.Find("Background/Goal").gameObject.GetComponent<Goal_Script>();
        remainingScript = gameObject.transform.parent.Find("Background/Remaining").gameObject.GetComponent<Remaining_Script>();
        scoreScript = gameObject.transform.parent.Find("Background/Score").gameObject.GetComponent<Score_Script>();
        soundHandlerScript = gameObject.transform.parent.Find("Sound Handler").gameObject.GetComponent<Sound_Handler_Script>();

    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}