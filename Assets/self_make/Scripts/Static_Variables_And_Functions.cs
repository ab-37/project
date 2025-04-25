using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Static_Variables
{

    //current part
    public static int currentAct = 0;
    public static int currentPart = 1;
    public static int lastGameScore = 0;
    public static float lastGameTime = 0;
    //public static string blockSelect;//return what stage need to play 
    //public static string blockRunning;//save what is it stage
    public static string currentBg = "1"; //bg data

    //act directories
    public static string[] actDirectories = {
        Application.dataPath + "/self_make/Scripts/NPC_script/CNact.json", //original
        "", //unused
        Application.dataPath + "/self_make/Scripts/CNdialogues.json", //AI, last
    };

    //mode of the dialogue
    //0 = original, 2 = last
    public static int dialogueMode;

    //is the dialogue loaded by ControlUI
    public static bool isDialogueLoadedControlUI;
    //is the dialogue loaded by End_script
    public static bool isDialogueLoadedEndScript;

    //level number
    public static int level_id;
    //level data

    public static string[][] specialPaths = {
        //from the middle, without all squares
        new string[] {"452", "45210", "4521036"},
        //only length 2
        //new string[] {"012", "014", "452"},
        //only length 2 and 3
        //new string[] {"012", "014", "452", "01254", "01258", "01436", "01476", "01478", "01458", "01452", "45210"}
    };

    //needs ? levels
    public static QuestionParameters[] levelQuestionParameters = {
        //id 0, unused tutorial
        new QuestionParameters(
            200, 0, (10, 50), (1, 50), 
            ((2, 5), (2, 5), (2, 5), (2, 5), (2, 5)),
            (2, 3), (1, 2), 0
        ),
        //id 1, tutorial (1-1)
        new QuestionParameters(
            150, 0, (10, 50), (1, 50), 
            ((1, 6), (1, 6), (1, 6), (1, 6), (1, 6)),
            (2, 5), (1, 1), 0,
            true, false, false, false
        ),
        //id 2, level 1-2, no target
        new QuestionParameters(
            200, 2, (10, 50), (1, 50),
            ((1, 7), (1, 7), (1, 7), (1, 7), (1, 7)),
            (2, 4), (1, 1), 1,
            true, false, true, false
        ),
        //id 3, unused
        new QuestionParameters(
            120, 1, (10, 50), (1, 50), 
            ((1, 7), (1, 7), (1, 7), (1, 7), (1, 7)),
            (2, 4), (1, 1), 1
        ),
        //id 4, level 2-1, target 120p
        new QuestionParameters(
            120, 1, (10, 50), (1, 50), 
            ((1, 7), (1, 7), (1, 7), (1, 7), (1, 7)),
            (2, 4), (1, 2), 1
        ),
        //id 5, level 4-1 (special), target 100s
        new QuestionParameters(
            300, 2, (75, 400), (0, 500), 
            ((15, 75), (15, 75), (300, 500), (15, 75), (15, 75)),
            ref specialPaths[0], (1, 1), 0,
            false, true, false, false, 1
        ),
        //id 6, unused
        new QuestionParameters(
            200, 2, (10, 50), (1, 50), 
            ((1, 7), (1, 7), (1, 7), (1, 7), (1, 7)),
            (2, 4), (1, 1), 1
        ),
        //id 7, level 3-1 (special), target 300p/360p
        new QuestionParameters(
            40, 1, (1, 99), (1, 99),
            ((1, 9), (1, 9), (1, 9), (1, 9), (1, 9)),
            (2, 2), (1, 1), 0,
            true, true, true, false, 1
        ),
        //id 8, level 7-1 (special), target 300p
        new QuestionParameters(
            120, 1, (1000, 10000), (10, 10000),
            ((10, 25), (10, 25), (10, 25), (10, 25), (10, 25)),
            (3, 3), (1, 1), 0,
            false, false, true, false, 1
        ),
        //id 9, level 2-2, target 120p/180p
        new QuestionParameters(
            120, 1, (10, 99), (1, 99),
            ((1, 9), (1, 9), (1, 9), (1, 9), (1, 9)),
            (2, 5), (1, 2), 1
        ),
        //id 10, level 3-2, target 360p/480p
        new QuestionParameters(
            240, 1, (10, 99), (1, 99),
            ((1, 9), (1, 9), (1, 9), (1, 9), (1, 9)),
            (2, 5), (1, 2), 1
        ),
        //id 11, level 4-2/7-2, target 700p/850p/1000p
        new QuestionParameters(
            600, 1, (13, 144), (-144, 144),
            ((1, 12), (1, 12), (1, 12), (1, 12), (1, 12)),
            (3, 5), (1, 2), 1
        ),
        //id 12, unused
        new QuestionParameters(
            600, 1, (13, 144), (-144, 144),
            ((1, 12), (1, 12), (1, 12), (1, 12), (1, 12)),
            (3, 5), (1, 2), 1
        )
    };
}

public static class Static_Functions
{
    //shuffle array
    public static void shuffleArray<T>(ref T[] arr) {
        int rng;
        T temp;
        int n = arr.Length;
        //fisher-yates shuffle algorithm
        while (n > 1) {
            rng = UnityEngine.Random.Range(0, n--);
            temp = arr[n];
            arr[n] = arr[rng];
            arr[rng] = temp;
        }
    }

    //shuffle list
    public static void shuffleList<T>(ref List<T> list) {
        int rng;
        T temp;
        int n = list.Count;
        //fisher-yates shuffle algorithm
        while (n > 1) {
            rng = UnityEngine.Random.Range(0, n--);
            temp = list[n];
            list[n] = list[rng];
            list[rng] = temp;
        }
    }

    //calculate result from expression
    public static int calculateResult(string expression) {
        List<string> expressionList = expression.Split(' ').ToList();
        //pos to number and sign
        List<int> numbers = new List<int>();
        List<string> signs = new List<string>();
        int i;
        foreach (string element in expressionList) {
            //Debug.Log(element);
            if (int.TryParse(element, out i)) {
                //number
                //i is used as a temp variable
                numbers.Add(i);
                //legacy code
                /*
                if (int.TryParse(gridNumbersScript.getGridContent(pos), out i)) {
                    numbers.Add(i);
                }
                else {
                    Debug.Log("Error: Parsing int failed");
                }
                */
            }
            else {
                //sign
                signs.Add(element);
            }
        }
        
        if (numbers.Count() == signs.Count()) {
            //last sign not followed by anything, delete last sign
            signs.RemoveAt(signs.Count() - 1);
        }
        

        //process mult and div
        i = 0; //i is used as an iterator
        while (i < signs.Count()) {
            if (signs[i] == "*") {
                numbers[i] *= numbers[i + 1];
                numbers.RemoveAt(i + 1);
                signs.RemoveAt(i);
            }
            else if (signs[i] == "/") {
                if (numbers[i + 1] == 0) {
                    //divide by 0, defaults value to 0
                    Debug.Log("Divide by zero!");
                    numbers[i] = 0;
                }
                else {
                    numbers[i] /= numbers[i + 1];
                }
                numbers.RemoveAt(i + 1);
                signs.RemoveAt(i);
            }
            else {
                ++i;
            }
        }
        //process add and sub
        while (signs.Any()) {
            if (signs[0] == "+") {
                numbers[0] += numbers[1];
            }
            else {
                numbers[0] -= numbers[1];
            }
            numbers.RemoveAt(1);
            signs.RemoveAt(0);
        }

        return numbers[0];
    }
}