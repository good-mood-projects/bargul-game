using UnityEngine;
using System.Collections;

public interface BasicObject
{
    bool isActive();
    void Use();
    bool isDestroyed();
    void setDestroyed(bool value);
}

