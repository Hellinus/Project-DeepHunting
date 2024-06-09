using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using MoreMountains.Tools;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Serialization;
using LevelManager = MoreMountains.CorgiEngine.LevelManager;

public enum PressEventType
{
    Drag,
    None,
    ClickTriggerAction
}

public class ReticleBase : MonoBehaviour
{
    [Header("ObjectInfo")]
    
    public ReticleInteractObjectInfoSO ObjectInfo;

    [Header("Control")]
    
    public bool ShowName = true;
    public bool ShowComment = true;
    public PressEventType ReticlePressEventType = PressEventType.None;
    
    [MMEnumCondition("ReticlePressEventType", (int)PressEventType.Drag)]
    public float PressMoveLerpValue = 0.5f;

    [MMEnumCondition("ReticlePressEventType", (int)PressEventType.Drag)]
    public Collider2D ConfineCollider;
    [MMEnumCondition("ReticlePressEventType", (int)PressEventType.Drag)]
    public bool HasTarget;
    [MMCondition("HasTarget", true)]
    public ReticlePuzzleTarget Target;
    [MMCondition("HasTarget", true)]
    public float AttachDistance = 0.3f;
    
    [MMEnumCondition("ReticlePressEventType", (int)PressEventType.ClickTriggerAction)]
    public ActionTrigger LinkedActionTrigger;
    [MMEnumCondition("ReticlePressEventType", (int)PressEventType.ClickTriggerAction)]
    public DialogueSystemTrigger Dialogue;

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

                if (!ConfineCollider)
                {
                    Debug.LogError("Confine Collider not found on " + this.gameObject);
                    return;
                }

                Vector3 v = Vector3.Lerp(this.gameObject.transform.position, reticlePosition + _offsetPosition, PressMoveLerpValue);
                v.x = Mathf.Clamp(v.x, ConfineCollider.bounds.min.x, ConfineCollider.bounds.max.x);
                v.y = Mathf.Clamp(v.y, ConfineCollider.bounds.min.y, ConfineCollider.bounds.max.y);
                this.gameObject.transform.position = v;

                if (HasTarget && Vector3.Distance(Target.transform.position, this.gameObject.transform.position) <= AttachDistance)
                {
                    this.gameObject.transform.position = Target.transform.position;
                    Target.ObjectIntoPosition(this);
                }

                break;
            case PressEventType.None:
                break;
            case PressEventType.ClickTriggerAction:
                _isPressed = true;
                if (!LinkedActionTrigger)
                {
                    Debug.LogError("LinkedActionTrigger not found on " + this.gameObject);
                    return;
                }
                Debug.Log("huh");
                Dialogue?.OnUse();
                LinkedActionTrigger.ReticleTrigger();
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

    public void ChangeReticlePressEventTypeType(PressEventType type)
    {
        ReticlePressEventType = type;
    }
}
