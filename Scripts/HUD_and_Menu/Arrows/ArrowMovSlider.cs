using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMovSlider : MonoBehaviour
{


    public float speed = 1.5f;
    public float fade = 1.0f;
    public float offset = 1.0f;
    bool moving = false;
    Vector3 initialPos;
    Vector3 finalPos;
    float fracJourney = 0.0f;
    enum Movstatus {FadeIn, Moving, FadeOut};
    int status = 0;


    // Use this for initialization
    void Start()
    {
        //initialPos = transform.position;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            if (status == (int)Movstatus.FadeIn)
            {
                Color c = gameObject.GetComponent<SpriteRenderer>().color;
                c.a = c.a + Time.deltaTime * fade;
                if (c.a >= 1.0f) {
                    c.a = 1.0f;
                    status++;
                }
                gameObject.GetComponent<SpriteRenderer>().color = c;

            }
            else if (status == (int)Movstatus.Moving)
            {
                fracJourney += Time.deltaTime * speed;
                if (fracJourney > 1.0f) fracJourney = 1.0f;
                transform.position = Vector3.Lerp(initialPos,finalPos,fracJourney);
                if (transform.position == finalPos)
                {
                status++;
                }
            }
            else if (status == (int)Movstatus.FadeOut)
            {
                Color c = gameObject.GetComponent<SpriteRenderer>().color;
                c.a = c.a - Time.deltaTime * fade;
                if (c.a <= 0.0f)
                {
                    c.a = 0.0f;
                    status = 0;
                    transform.position = initialPos;
                    fracJourney = 0.0f;
                }
                gameObject.GetComponent<SpriteRenderer>().color = c;
            }
        }
    }

    void setPosition(Vector2 initPos, Vector2 endPos)
    {
        initialPos = new Vector3(initPos.x, initPos.y, 0.0f);
        finalPos = new Vector3(endPos.x, endPos.y, 0.0f);
        transform.position = initialPos;
    }

    public void updateInitialPos(Vector2 pos)
    {
        initialPos = pos;
    }

    public void initiateAnimation(Vector2 iniPost, Vector2 endPos)
    {
        setPosition(iniPost,endPos);
        Color c = gameObject.GetComponent<SpriteRenderer>().color;
        c.a = 0.0f;
        gameObject.GetComponent<SpriteRenderer>().color = c;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        moving = true;
        status = (int)Movstatus.FadeIn;
    }

    public void finishAnimation()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        moving = false;
    }
}
