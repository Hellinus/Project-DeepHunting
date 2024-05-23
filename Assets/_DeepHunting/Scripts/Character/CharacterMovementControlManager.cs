using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine;

public class CharacterMovementControlManager : MonoBehaviour
{
    [SerializeField][MMReadOnly] protected bool _setIdle = false;
    [SerializeField][MMReadOnly] protected float _setHorizontalMovement = 0f;
    [SerializeField] [MMReadOnly] protected int _setCharacterFaceDirection = 0;
    
    public virtual void SetIdle()
    {
        StopControl();
        LevelManager.Instance.Players[0].SetIdle();
        _setIdle = true;
    }
    
    public virtual void SetHorizontalMovement(float value)
    {
        StopControl();
        LevelManager.Instance.Players[0].SetHorizontalMovement(value);
        _setHorizontalMovement = value;
    }

    public virtual void FlipCharacter()
    {
        LevelManager.Instance.Players[0].FlipCharacter();
    }

    public virtual void SetCharacterFaceDirection(int value)
    {
        if (value > 0)
        {
            if (!LevelManager.Instance.Players[0].IsFacingRight)
            {
                FlipCharacter();
            }

            _setCharacterFaceDirection = 1;
        }
        else
        {
            if (LevelManager.Instance.Players[0].IsFacingRight)
            {
                FlipCharacter();
            }

            _setCharacterFaceDirection = -1;
        }
    }

    public virtual void AddCharacterHoriztonalPosition(float x)
    {
        Vector3 v = LevelManager.Instance.Players[0].gameObject.transform.position;
        v.x += x;
        LevelManager.Instance.Players[0].gameObject.transform.position = v;
    }
    
    
    
    public virtual void StopControl()
    {
        if (_setIdle)
        {
            LevelManager.Instance.Players[0].UnsetIdle();
            _setIdle = false;
        }

        if (!Mathf.Approximately(_setHorizontalMovement, 0f))
        {
            LevelManager.Instance.Players[0].UnsetHorizontalMovement();
            _setHorizontalMovement = 0f;
        }
    }
    
}
