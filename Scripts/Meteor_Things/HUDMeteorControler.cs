using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDMeteorControler : MonoBehaviour {

    public Text timer;

    private Meteor meteor;

    void Start()
    {
        meteor = GetComponentInParent<Meteor>();
    }

    void Update()
    {
        timer.text = Mathf.FloorToInt(meteor.TimeLeft()).ToString();
        if(meteor.TimeLeft() <= 0.0f)
        {
            timer.enabled = false; // Hides timer
        }
    }

    public void ShowTime()
    {
        timer.enabled = true;
    }
}
