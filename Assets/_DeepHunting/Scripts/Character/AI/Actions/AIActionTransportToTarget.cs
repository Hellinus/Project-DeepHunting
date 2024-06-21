using System.Collections;
using System.Collections.Generic;
using System.Net;
using DG.Tweening;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine;

[AddComponentMenu("Corgi Engine/Character/AI/Actions/AIActionTransportToTarget")]
public class AIActionTransportToTarget : AIAction
{
    public bool TargetSetsToPlayer = true;
    public bool ConsistentMovement = true;
    [MMCondition("ConsistentMovement", true)]
    public float TransportTime = 0.2f;
    
    public override void PerformAction()
    {
        Transport();
    }

    protected virtual void Transport()
    {
        if (TargetSetsToPlayer)
        {
            if (LevelManager.HasInstance)
            {
                _brain.Target = LevelManager.Instance.Players[0].transform;
            }

            if (LevelManager.Instance.Players[0].IsOnGround())
            {
                if (ConsistentMovement)
                {
                    this.transform.DOMove(_brain.Target.position, TransportTime);
                }
                else
                {
                    this.transform.position = _brain.Target.position;
                }
            }
        }
        else
        {
            if (ConsistentMovement)
            {
                this.transform.DOMove(_brain.Target.position, TransportTime);
            }
            else
            {
                this.transform.position = _brain.Target.position;
            }
        }
    }
}
