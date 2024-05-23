using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Rendering;

public enum TriggerType
{
    TriggeredEnter,
    TriggeredExit
}
public class ActionTrigger : MonoBehaviour
{
    [Header("Trigger Requirements")]
    public TriggerType TriggerType;
    public bool TagSetsToPlayer = false;
    public List<string> AcceptedTags;

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
