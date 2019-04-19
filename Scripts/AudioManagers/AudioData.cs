using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to save and load the volumes
/// </summary>
[System.Serializable]
    class AudioData 
    {
        float master;
        float background;
        float fx;
        float ui;

        public AudioData(float _master, float _background, float _fx, float _ui)
        {
            master = _master;
            background = _background;
            fx = _fx;
            ui = _ui;
        }

        // Multiplies the return value on 100 so its on the input range needed to work properly for the setters
        public float GetMaster()
        {
            return master * 100;
        }

        public float GetBackground()
        {
            return background * 100;
        }

        public float GetFx()
        {
            return fx * 100;
        }

        public float GetUi()
        {
            return ui * 100;
        }
    }
