using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PauseBuilder : MonoBehaviour {

    public Text score, record;

    public void BuildPause()
    {
        record.text = "RECORD\n" + PlayerPrefs.GetInt("Highscore").ToString();
        score.text = "SCORE\n" + GameController.Instance.GetScore().ToString();
        gameObject.SetActive(true);
    }

    public void Resume()
    {
        gameObject.SetActive(false);
    }
}