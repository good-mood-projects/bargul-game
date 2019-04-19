using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gyroscope : MonoBehaviour, BasicObject
{
    public MeteorSpawner spawner;
    public ParticleSystem ps;
    public ParticleSystem smokePs;
    public float psStopThreshold = 1.0f; // Time the particle system takes to stop completely
    public bool active;

    public float duration = 3.0f;
    float remainingDuration;
    //RectTransform bufferTransform; // Canvas timer for buffering
    Vector3 startBufferScale;

    List<Meteor> signaledMeteors;

    bool destroyed = false;

    void Start()
    {
        signaledMeteors = new List<Meteor>();
        //bufferTransform = GetComponentInChildren<RectTransform>();
        //startBufferScale = bufferTransform.localScale;
        //bufferTransform.gameObject.SetActive(false);
        if (ps == null) Debug.LogError("ParticleSystem not set in the gyroscope inspector");
        if (smokePs == null) Debug.LogError("ParticleSystem not set in the gyroscope inspector");
    }

    public bool isActive()
    {
        return active;
    }

    public void disableUse()
    {
        remainingDuration = 0.0f;
    }

    public void setDestroyed(bool value)
    {
        Color currentColor = GetComponent<SpriteRenderer>().color;
        if (value)
        {
            currentColor.a = 0.8f;
            GetComponent<SpriteRenderer>().color = currentColor;
        }
        else
        {
            smokePs.Play();
            currentColor.a = 1.0f;
            GetComponent<SpriteRenderer>().color = currentColor;
        }
        destroyed = value;
    }

    public bool isDestroyed()
    {
        return destroyed;
    }

    /// <summary>
    /// Shows the positions of the active meteors
    /// </summary>

    public void Use()
    {
        if (!active && !destroyed)
        {
            active = true;
            remainingDuration = duration;

            // Activates the buffer shrink & particle system
            /*
            bufferTransform.gameObject.SetActive(true);
            bufferTransform.localScale = startBufferScale;
            StartCoroutine(Utils.BufferShrink(bufferTransform, startBufferScale, duration));
            */
            ps.Play();
        }
    }

    void Update()
    {
        // Controls the pause / resume behaviour of the ps
        if (Utils.freezed)
        {
            if (!ps.isPaused) ps.Pause();
        }
        else
        {
            if (!ps.isPlaying) ps.Play();
        }

        if (remainingDuration > 0.0f && !destroyed)
        {
            if(remainingDuration <= psStopThreshold) ps.Stop(); // Starts stoping the particle system
            remainingDuration -= Time.deltaTime;
        }
        else
        {
            ps.Stop(); // Stops the particle system in the case the time.deltaTime skips the threshold duration
            active = false;
        }
    }
}
