using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Static_Variables
{
    public static int lastGameScore;
    public static float lastGameTime;
    public static string blockSelect;//return what stage need to play 
    public static string blockRuning;//save what is it stage

    //level number
    public static int level = 7;
    //level data

    public static string[][] specialPaths = {
        //from the middle, without all squares
        new string[] {"452", "45210", "4521036"},
        //only length 2
        new string[] {"012", "014", "452"}
    };

    //needs 12 levels
    public static QuestionParameters[] levelQuestionParameters = {
        //id 0
        new QuestionParameters(
            150, 2, (10, 45), (10, 45), 
            ((1, 9), (1, 9), (1, 9), (1, 9), (1, 9)),
            (2, 5), (1, 1), 0,
            true, false, false, false),
        //id 1
        new QuestionParameters(
            150, 2, (20, 56), (1, 56), 
            ((1, 4), (2, 5), (1, 8), (3, 6), (4, 7)),
            (3, 5), (2, 2), 0,
            true, false, false, false),
        //id 2
        new QuestionParameters(
            150, 2, (10, 50), (1, 50),
            ((1, 7), (1, 7), (1, 7), (1, 7), (1, 7)),
            (2, 4), (1, 2), 1),
        //id 3
        new QuestionParameters(
            120, 1, (10, 50), (1, 50), 
            ((1, 7), (1, 7), (1, 7), (1, 7), (1, 7)),
            (2, 4), (1, 1), 1),
        //id 4
        new QuestionParameters(
            120, 1, (10, 50), (1, 50), 
            ((1, 7), (1, 7), (1, 7), (1, 7), (1, 7)),
            (2, 4), (1, 2), 1),
        //id 5
        new QuestionParameters(
            90, 1, (75, 400), (0, 500), 
            ((15, 75), (15, 75), (300, 500), (15, 75), (15, 75)),
            ref specialPaths[0], (1, 1), 0,
            false, true, false, false),
        //id 6
        new QuestionParameters(
            200, 2, (10, 50), (1, 50), 
            ((1, 7), (1, 7), (1, 7), (1, 7), (1, 7)),
            (2, 4), (1, 1), 1),
        //id 7
        new QuestionParameters(
            30, 1, (1, 99), (1, 99),
            ((1, 9), (1, 9), (1, 9), (1, 9), (1, 9)),
            ref specialPaths[1], (1, 1), 0,
            true, true, true, false, 1
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