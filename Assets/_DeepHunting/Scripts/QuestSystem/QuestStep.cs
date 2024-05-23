using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{
    private string _questId;
    private bool _isFinished = false;

    // For Save System
    protected void Start()
    {
        if (_isFinished)
        {
            Destroy(this.gameObject);
        }
    }
    
    public void InitializeQuestStep(string questId)
    {
        this._questId = questId;
    }

    protected void FinishQuestStep()
    {
        if (!_isFinished)
        {
            _isFinished = true;
            MMEventManager.TriggerEvent(new QuestEvent(QuestEventType.OnAdvanceQuest, _questId));
            Destroy(this.gameObject);
        }
    }
    
}
