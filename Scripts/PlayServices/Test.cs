using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{

    public Text UId;

    // Use this for initialization
    void Start()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        int tries = 0;
        bool success = false;
        LogIn();

    }

    public bool LogIn()
    {
        bool outPut = false;
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                Debug.Log("login success");
                Debug.Log(Social.localUser.id);
                Debug.Log(Social.localUser.userName);
                UId.text = Social.localUser.id;
                Social.ReportScore(12345, "CgkIuvj6vc0IEAIQAQ", (bool result) => {
                    if (result)
                    {
                        UId.text = "SUCCESS UPLOADING";
                    }
                    else
                    {
                        UId.text = "ERROR UPLOADING";
                    }
                    // handle success or failure
                });
                Social.ShowLeaderboardUI();
                outPut = true;
            }
            else
            {
                Debug.Log("login fail");
            }

        });
        return outPut;
    }

    public void LogOut()
    {
        Debug.Log("LogOut");
        ((PlayGamesPlatform)Social.Active).SignOut();
    }
}
