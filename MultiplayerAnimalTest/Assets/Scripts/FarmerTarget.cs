using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmerTarget : MonoBehaviour
{
    public float _delayTime = 2f;
    public EFarmerJob _animationTriggerString = EFarmerJob.JustStandThere;

    public float GetDelayTime()
    {
        return _delayTime;
    }

    public string GetAnimationString()
    {
        return _animationTriggerString.ToString();
    }
}
