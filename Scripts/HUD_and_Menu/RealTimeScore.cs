using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealTimeScore : MonoBehaviour {

    Text scoreValue;
    public LevelManager manager;
    float period = 0.1f;
    float currentTime = 0.0f;

    void Awake()
    {
        scoreValue = GetComponent<Text>();
    }
	// Update is called once per frame
	void Update () {
        currentTime += Time.deltaTime;
        if (currentTime > period)
        {
            scoreValue.text = Mathf.FloorToInt(manager.getScore()).ToString();
            currentTime = 0.0f;
        }
	}
}
