using UnityEngine;
using System.Collections;

public class ResolutionAndVolume : MonoBehaviour {

	public static ResolutionAndVolume instance;

	public int res = 1; // Not being used actually, but set looking forward to a PC port.
	public float masterVolume = 1.0f;
    public float backgroundVolume = 1.0f;
    public float fxVolume = 1.0f;

    // Use this for avoid get destroyed on scene load
    void Awake () 
	{
		if (instance)
		{
			DestroyImmediate(this.gameObject);
		}
		else
		{
			DontDestroyOnLoad(this.gameObject);
			instance = this;
		}
	}

    /// <summary>
    /// Manages the resolution and volume during the whole execution.
    /// </summary>
	public void ChangePreferences(int newRes, float newMaster, float newBackground, float newFx)
	{
		res = newRes;
		masterVolume = newMaster;
        backgroundVolume = newBackground;
        fxVolume = newFx;
    }
}
