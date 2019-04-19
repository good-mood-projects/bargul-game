using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spiner : MonoBehaviour
{
    public float rotationSpeed = 15.0f;

	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(0.0f, 0.0f, rotationSpeed * Time.deltaTime);
	}
}
