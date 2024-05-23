using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using MoreMountains.Tools;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using LevelManager = MoreMountains.CorgiEngine.LevelManager;

public enum PressEventType
{
    Drag,
    None
}

public class ReticleBase : MonoBehaviour
{
    [Header("ObjectInfo")]
    
    public ReticleInteractObjectInfoSO ObjectInfo;

    [Header("Control")]
    
    public bool ShowName = true;
    public bool ShowComment = true;
    public PressEventType ReticlePressEventType = PressEventType.None;
    public float PressMoveLerpValue = 0.5f;

    protected Vector3 _offsetPosition = Vector3.zero;
    protected bool _isPressed = false;
    

    public virtual void OnReticleEnter()
    {
        Debug.Log("Reticle Enter: " + this.gameObject.name);
    }

    public virtual void OnReticleButtonDown()
    {
        Debug.Log("Reticle Down: " + this.gameObject.name);
    }

    public virtual void OnReticleButtonPress(Vector3 reticlePosition)
    {
        switch (ReticlePressEventType)
        {
            case PressEventType.Drag:
                if (!_isPressed)
                {
                    _offsetPosition = this.gameObject.transform.position - reticlePosition;
                    _isPressed = true;
                }
                this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, reticlePosition + _offsetPosition, PressMoveLerpValue);
                break;
            case PressEventType.None:
                break;
        }
    }
    
    public virtual void OnReticleButtonUp()
    {
        Debug.Log("Reticle Up: " + this.gameObject.name);
        _offsetPosition = Vector3.zero;
        _isPressed = false;
    }

    public virtual void OnReticleButtonHighlight()
    {
        _offsetPosition = Vector3.zero;
        _isPressed = false;
        // Debug.Log("Highlight: " + this.gameObject.name);
    }
    
    public virtual void OnReticleLeave()
    {
        Debug.Log("Reticle Leave: " + this.gameObject.name);
    }
}
