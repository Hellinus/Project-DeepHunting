using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using PixelCrushers.DialogueSystem.Wrappers;
using UnityEngine;
using UnityEngine.Rendering;

public enum TriggerType
{
    TriggeredEnter,
    TriggeredExit,
    TriggerStay,
    TriggerByReticleClick,
    TriggerByEvent
}
public class ActionTrigger : MonoBehaviour
{
    [Header("Trigger Requirements")]
    public TriggerType TriggerType;
    [MMEnumCondition("TriggerType", (int)TriggerType.TriggeredExit, (int)TriggerType.TriggeredEnter)]
    public bool TagSetsToPlayer = false;
    [MMEnumCondition("TriggerType", (int)TriggerType.TriggeredExit, (int)TriggerType.TriggeredEnter)]
    public List<string> AcceptedTags;
    [MMEnumCondition("TriggerType", (int)TriggerType.TriggerStay)]
    public float TargetTime = 1f;
    [MMEnumCondition("TriggerType", (int)TriggerType.TriggerStay)]
    public bool TriggerNotOnce = false;
    [MMCondition("TriggerNotOnce", true)]
    public int TriggerTime = 3;
    [MMEnumCondition("TriggerType", (int)TriggerType.TriggerStay, (int)TriggerType.TriggerByEvent)]
    public DialogueSystemTrigger Dialogue;
    
    protected float _curStayTime = 0f;
    protected bool _isTriggered = false;
    protected int _curTriggerTime = 0;
    
        private void OnTriggerEnter2D(Collider2D other)
    {
        if(TriggerType != TriggerType.TriggeredEnter) return;
        
        foreach (var tag in AcceptedTags)
        {
            if (other.CompareTag(tag))
            {
                ActionEvent.Trigger(this.gameObject.name);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(TriggerType != TriggerType.TriggerStay) return;

        if (other.CompareTag("Player"))
        {
            if (_isTriggered) return;
            
            _curStayTime += Time.deltaTime;
            if (_curStayTime >= TargetTime)
            {
                Dialogue?.OnUse();
                ActionEvent.Trigger(this.gameObject.name);
                _isTriggered = true;
                _curStayTime = 0f;
                
                if (TriggerNotOnce && _curTriggerTime == -1)
                {
                    _isTriggered = false;
                }
                else if (TriggerNotOnce && _curTriggerTime < TriggerTime)
                {
                    _isTriggered = false;
                    _curTriggerTime += 1;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(TriggerType != TriggerType.TriggeredExit) return;
        
        foreach (var tag in AcceptedTags)
        {
            if (other.CompareTag(tag))
            {
                ActionEvent.Trigger(this.gameObject.name);
            }
        }
    }

    public void ReticleTrigger()
    {
        if(TriggerType != TriggerType.TriggerByReticleClick) return;

        ActionEvent.Trigger(this.gameObject.name);

    }

    public void EventTrigger()
    {
        if(TriggerType != TriggerType.TriggerByEvent) return;
        
        Dialogue?.OnUse();
        ActionEvent.Trigger(this.gameObject.name);
    }
    

    private void OnValidate()
    {
        if (TagSetsToPlayer)
        {
            if (AcceptedTags.Count > 0)
            {
                if (AcceptedTags[0] != "Player")
                {
                    AcceptedTags.Insert(0, "Player");
                }
            }
            else
            {
                AcceptedTags.Add("Player");
            }
        }
    }
}
