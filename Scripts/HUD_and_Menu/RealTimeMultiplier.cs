using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealTimeMultiplier : MonoBehaviour
{

    Text multiplierValue;
    public LevelManager manager;
    float period = 0.1f;
    float currentTime = 0.0f;

    void Awake()
    {
        multiplierValue = GetComponent<Text>();
    }
    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > period)
        {
            if (Application.systemLanguage == SystemLanguage.Spanish)
                multiplierValue.text = "Multiplicador: "+Mathf.FloorToInt(manager.getNumObjects()).ToString()+"X";
            else
                multiplierValue.text = "Bonus: " + Mathf.FloorToInt(manager.getNumObjects()).ToString() + "X";
            currentTime = 0.0f;
        }
    }
}

