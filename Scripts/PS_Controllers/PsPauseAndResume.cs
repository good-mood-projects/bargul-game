using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class wich manages pause/resume & destruction of free particle systems.
/// </summary>
public class PsPauseAndResume : MonoBehaviour
{
    ParticleSystem ps;
    bool active = false;

    public void ActivatePS()
    {
        active = true;
        ps.Play();
    }

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (ps.isStopped)
        {
            active = false;
        }

        if (active)
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
        }
	}
}
