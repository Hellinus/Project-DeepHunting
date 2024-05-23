using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;
public enum QuestEventType
{
    OnStartQuest,
    OnAdvanceQuest,
    OnFinishQuest
}

public struct QuestEvent
{
    public string questId;
    public QuestEventType questEventType;
    public QuestEvent(QuestEventType newEventType, string newId)
    {
        questEventType = newEventType;
        questId = newId;
    }
    static QuestEvent e;
    public static void Trigger(QuestEventType newEventType, string newId)
    {
        e.questEventType = newEventType;
        e.questId = newId;
        MMEventManager.TriggerEvent(e);
    }
}
