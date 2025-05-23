using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Effect", menuName = "Effects/Effect")]
public class Effect : ScriptableObject
{
    public virtual void UseEffect()
    {
        //Use effect
    }
}
