using UnityEngine;
using System.Collections;

public class Background_Scroller : MonoBehaviour
{
    public float scrollSpeed;
    public float tileXPos;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileXPos);
        transform.position = startPosition + Vector3.right * newPosition;
    }
}
