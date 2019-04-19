using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Utils
{
    public static bool freezed = false;
    public static IEnumerator MoveAToB(Transform objectTransform, Vector2 end, float timeToFall)
    {
        // Travel from A to B
        float counter = 0f;
        Vector2 start = objectTransform.localPosition; 
        while (counter/timeToFall <= 1.0f)
        {
            if (!freezed)
            {
                objectTransform.localPosition = Vector2.Lerp(start, end, counter / timeToFall);
                counter += Time.deltaTime;
            }
            yield return null;
        }

        objectTransform.localPosition = end;
    }

    /// <summary>
    /// Shrinks the buffer icon off the object
    /// </summary>
    /// <param name="bufferToShrink"> Object to shrink </param>
    /// <param name="timeToShrink"> Duration time of the object use </param>
    /// <returns></returns>
    public static IEnumerator BufferShrink(RectTransform bufferToShrink, Vector3 startBufferScale, float timeToShrink)
    {
        // Shrinks the buffer until dissapears
        float counter = 0f;
        while (counter / timeToShrink <= 1.0f)
        {
            if (!freezed)
            {
                bufferToShrink.localScale = Vector3.Lerp(startBufferScale, Vector3.zero, counter / timeToShrink);
                counter += Time.deltaTime;
            }
            yield return null;
        }

        bufferToShrink.gameObject.SetActive(false);
    }

    public static IEnumerator Shrink(Transform transformToShrink, Vector3 startScale, float timeToShrink)
    {
        // Shrinks the buffer until dissapears
        float counter = 0f;
        while (counter / timeToShrink <= 1.0f)
        {
            transformToShrink.localScale = Vector3.Lerp(startScale, Vector3.zero, counter / timeToShrink);
            counter += Time.deltaTime;
            yield return null;
        }

        transformToShrink.gameObject.SetActive(false);
    }

    /// <summary>
    /// Fades out a given transform in a given time
    /// </summary>
    /// <param name="bufferToShrink"> Object to fade </param>
    /// <param name="timeToShrink"> Duration time of fade out </param>
    /// <returns></returns>
    public static IEnumerator FadeOut(Image UIImageToFade,float timeToFade)
    {
        Color start = UIImageToFade.color;
        Color end = start;
        end.a = 255;

        float counter = 0f;
        while (counter / timeToFade <= 1.0f)
        {
            UIImageToFade.color = Color.Lerp(start, end, counter / timeToFade);
            counter += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// Fades out a given transform in a given time
    /// </summary>
    /// <param name="bufferToShrink"> Object to fade </param>
    /// <param name="timeToShrink"> Duration time of fade out </param>
    /// <returns></returns>
    public static IEnumerator FadeIn(Image UIImageToFade, float timeToFade)
    {
        Color start = UIImageToFade.color;
        Color end = start;
        end.a = 0;

        float counter = 0f;
        while (counter / timeToFade <= 1.0f)
        {
            UIImageToFade.color = Color.Lerp(start, end, counter / timeToFade);
            counter += Time.deltaTime;
            yield return null;
        }
    }
}
