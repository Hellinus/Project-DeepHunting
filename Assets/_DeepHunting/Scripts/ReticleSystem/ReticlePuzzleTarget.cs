using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class ReticlePuzzleTarget : MonoBehaviour
{
    public ReticleBase LinkedObject;
    public MMFeedbacks ObjectIntoPositionFeedbacks;
    public MMFeedbacks LinkedObjectIntoPositionFeedbacks;
    public MMFeedbacks WrongObjectIntoPositionFeedbacks;
    
    public void ObjectIntoPosition(ReticleBase reticleBase)
    {
        ObjectIntoPositionFeedbacks?.PlayFeedbacks();
        if (reticleBase == LinkedObject)
        {
            LinkedObjectIntoPosition();
        }
        else
        {
            WrongObjectIntoPositionFeedbacks?.PlayFeedbacks();
        }
    }

    public void LinkedObjectIntoPosition()
    {
        LinkedObjectIntoPositionFeedbacks?.PlayFeedbacks();
    }
}
