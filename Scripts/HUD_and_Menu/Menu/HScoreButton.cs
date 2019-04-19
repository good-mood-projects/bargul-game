using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class HScoreButton : MonoBehaviour {


    void Awake()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }
    public void OnClickLeaderboards()
    {
        GraphicRaycaster r = FindObjectOfType<GraphicRaycaster>();
        r.enabled = false;
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                Debug.Log("login success");
                Social.ShowLeaderboardUI();
            }
            else
            {
                Debug.Log("login fail");
            }

        });
        r.enabled = true;
    }
}
