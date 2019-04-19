using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shape {

    Vector2[] hitShape;
    int spriteImage;

    public Shape(Vector2[] newHitShape, int size)
    {
        hitShape = newHitShape;
        spriteImage = size;
    }

    public void SetHitShape(Vector2[] newHitShape)
    {
        hitShape = newHitShape;
    }

    public Vector2[] GetHitShape()
    {
        return hitShape;
    }

    public void SetSpriteImage(int newSpriteImage)
    {
        spriteImage = newSpriteImage;
    }

    public int GetSpriteImage()
    {
        return spriteImage;
    }
}
