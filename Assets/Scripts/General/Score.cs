using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Score : MonoBehaviour
{
    Text text;
    int[] curScore = new int[2], firstBest = new int[2], secondBest = new int[2], thirdBest = new int[2];

    private void Awake()
    {
        text = GetComponent<Text>();
        curScore[0] = PlayerPrefs.GetInt("wave");
        curScore[1] = PlayerPrefs.GetInt("timeSurvived");
        text.text = ("You made it to wave " + curScore[0]) + ("\nYou survived " + curScore[1] + " seconds");

        if(PlayerPrefs.GetString("first") != null)
            firstBest = MethodPlus.StringParse<int>(PlayerPrefs.GetString("first"), 'r').ToArray();
        if (PlayerPrefs.GetString("second") != null)
            secondBest = MethodPlus.StringParse<int>(PlayerPrefs.GetString("second"), 'r').ToArray();
        if (PlayerPrefs.GetString("third") != null)
            thirdBest = MethodPlus.StringParse<int>(PlayerPrefs.GetString("third"), 'r').ToArray();

        if (firstBest[0] < curScore[0] || ((firstBest[0] == curScore[0]) && firstBest[1] <= curScore[1]))
        {
            thirdBest = secondBest;
            secondBest = firstBest;
            firstBest = curScore;
        }
        else if(secondBest[0] < curScore[0] || ((secondBest[0] == curScore[0]) && secondBest[1] <= curScore[1]))
        {
            thirdBest = secondBest;
            secondBest = curScore;
        }
        else if (thirdBest[0] < curScore[0] || ((thirdBest[0] == curScore[0]) && thirdBest[1] <= curScore[1]))
        {
            thirdBest = curScore;
        }
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetString("first", MethodPlus.ToString(firstBest, "r"));
        PlayerPrefs.SetString("second", MethodPlus.ToString(secondBest, "r"));
        PlayerPrefs.SetString("third", MethodPlus.ToString(thirdBest, "r"));
    }
}
