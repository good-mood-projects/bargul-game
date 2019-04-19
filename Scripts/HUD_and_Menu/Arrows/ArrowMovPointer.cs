using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMovPointer : MonoBehaviour {


    public float speed = 1.5f;
    public float amplitude = 1.0f;
    public float offset = 0.01f;
    bool moving = false;
    bool horizontal = false;
    Vector3 initialPos;

	// Use this for initialization
	void Start () {
        //initialPos = transform.position;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (moving && !horizontal)
        {
            transform.position = new Vector3(initialPos.x, initialPos.y + offset + amplitude * Mathf.Sin(Time.time * 2 * Mathf.PI * speed), transform.position.z);
        }
        else if (moving && horizontal)
            transform.position = new Vector3(initialPos.x + offset + amplitude * Mathf.Sin(Time.time * 2 * Mathf.PI * speed), initialPos.y, transform.position.z);
    }

    public void setHorizontal()
    {
        gameObject.transform.Rotate(0.0f, 0.0f, -90.0f);
        horizontal = true;
    }


    void setPosition(Vector2 pos)
    {
        initialPos = new Vector3(pos.x, pos.y+offset, 0.0f);
        transform.position = initialPos;
    }

    public void initiateAnimation(Vector2 position)
    {
        Color c = gameObject.GetComponent<SpriteRenderer>().color;
        c.a = 1.0f;
        gameObject.GetComponent<SpriteRenderer>().color = c;
        setPosition(position);
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        moving = true;
    }

    public void finishAnimation()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        horizontal = false;
        moving = false;
    }
}
