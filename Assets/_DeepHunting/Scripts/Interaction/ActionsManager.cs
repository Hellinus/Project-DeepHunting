using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class TriggerInfo
{
    [Header("Info")]
    public string Name;
    public ActionTrigger ActionTrigger;
    public bool SetActivateAtStart = true;
    public bool DisabledAfterTriggerd = true;

    [Header("Feedbacks")]
    public List<MMFeedbacks> FeedbacksList;

    [Header("Consequent Actions")]
    public List<SetActivateColliderStruct> SetActivateCollidersList;
    public List<SetActivateConsequentActionStruct> SetActivateConsequentActionsList;
    public List<SetActivateGameObjectStruct> SetActivateGameObjectsList;
}

[System.Serializable]
public class SetActivateColliderStruct
{
    public GameObject Collider; 
    public bool SetActivate = true;
}

[System.Serializable]
public class SetActivateConsequentActionStruct
{
    public ActionTrigger ActionTrigger; 
    public bool SetActivate = false;
}

[System.Serializable]
public class SetActivateGameObjectStruct
{
    public GameObject GameObject; 
    public bool SetActivate = false;
}

public class ActionsManager : MMSingleton<ActionsManager>, MMEventListener<ActionEvent>
{
    public bool Lock = false;
    
    [SerializeField] protected List<TriggerInfo> ActionsList;

    [SerializeField] protected List<TriggerInfo> StaticActionsList;
    
    private void Start()
    {
        foreach (var triggerInfo in ActionsList)
        {
            if (triggerInfo.SetActivateAtStart)
            {
                triggerInfo.ActionTrigger.gameObject.SetActive(true);
            }
            else
            {
                triggerInfo.ActionTrigger.gameObject.SetActive(false);
            }
        }
        
        foreach (var triggerInfo in StaticActionsList)
        {
            if (triggerInfo.SetActivateAtStart)
            {
                triggerInfo.ActionTrigger.gameObject.SetActive(true);
            }
            else
            {
                triggerInfo.ActionTrigger.gameObject.SetActive(false);
            }
        }
    }

    public virtual void OnMMEvent(ActionEvent eventType)
    {
        string name = eventType.ActionName;

        foreach (var triggerInfo in ActionsList)
        {
            if (triggerInfo.Name.Equals(name))
            {
                ActionTriggered(triggerInfo);
                if (triggerInfo.DisabledAfterTriggerd)
                {
                    triggerInfo.ActionTrigger.gameObject.SetActive(false);
                }
            }
        }
        
        foreach (var triggerInfo in StaticActionsList)
        {
            if (triggerInfo.Name.Equals(name))
            {
                ActionTriggered(triggerInfo);
                if (triggerInfo.DisabledAfterTriggerd)
                {
                    triggerInfo.ActionTrigger.gameObject.SetActive(false);
                }
            }
        }
    }

    protected void ActionTriggered(TriggerInfo triggerInfo)
    {

        foreach (var feedbacks in triggerInfo.FeedbacksList)
        {
            feedbacks.PlayFeedbacks();
        }

        foreach (var setActivateCollidersStruct in triggerInfo.SetActivateCollidersList)
        {
            if (setActivateCollidersStruct.SetActivate)
            {
                setActivateCollidersStruct.Collider.SetActive(true);
            }
            else
            {
                setActivateCollidersStruct.Collider.SetActive(false);
            }
        }

        foreach (var setActivateConsequentActionsList in triggerInfo.SetActivateConsequentActionsList)
        {
            if (setActivateConsequentActionsList.SetActivate)
            {
                setActivateConsequentActionsList.ActionTrigger.gameObject.SetActive(true);
            }
            else
            {
                setActivateConsequentActionsList.ActionTrigger.gameObject.SetActive(false);
            }
        }

        foreach (var setActivateGameObjectStruct in triggerInfo.SetActivateGameObjectsList)
        {
            if (setActivateGameObjectStruct.SetActivate)
            {
                setActivateGameObjectStruct.GameObject.SetActive(true);
            }
            else
            {
                setActivateGameObjectStruct.GameObject.SetActive(false);
            }
        }
    }

    private void OnValidate()
    {
        foreach (var triggerInitialization in ActionsList)
        {
            triggerInitialization.Name = triggerInitialization.ActionTrigger.gameObject.name;
        }
        
        foreach (var triggerInitialization in StaticActionsList)
        {
            triggerInitialization.Name = triggerInitialization.ActionTrigger.gameObject.name;
        }

        if (Lock)
        {
            
        }
    }
    
    private void OnEnable()
    {
        this.MMEventStartListening<ActionEvent>();
    }
    private void OnDisable()
    {
        this.MMEventStopListening<ActionEvent>();
    }
}
