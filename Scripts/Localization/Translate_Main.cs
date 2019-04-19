using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Translate_Main : MonoBehaviour
{
    public Text musicText;
    public Text UIText;
    public Text FXText;
    public Text finalScoreText;
    public Text saveScoreText;
    public Text currentScore;
    public Text initialTutText;

    public GameController controller;

    // Use this for initialization
    void Awake()
    {
        if (Application.systemLanguage != SystemLanguage.Spanish)
        {
            SetupEnglish();
        }
    }

    void SetupEnglish()
    {
        initialTutText.text = "Move Bargul\nDo not get hit\nby meteors!";
        ITutorialSection[] sections = controller.GetComponents<ITutorialSection>();
        foreach (ITutorialSection section in sections)
        {
            section.setLanguage("EN");
        }
        finalScoreText.text = "Final Score:";
        saveScoreText.text = "Save Score";
        currentScore.text = "Current score:";
        musicText.text = "Music:";
        UIText.text = "Interface:";
        FXText.text = "Sound FX:";
    }
}
