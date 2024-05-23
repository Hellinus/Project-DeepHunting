using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class QuestManager : MonoBehaviour, MMEventListener<QuestEvent>
{
    private Dictionary<string, Quest> _questMap;

    private void Awake()
    {
        _questMap = CreateQuestMap();
    }

    private void OnEnable()
    {
        this.MMEventStartListening<QuestEvent>();
    }
    private void OnDisable()
    {
        this.MMEventStopListening<QuestEvent>();
    }
    
    public virtual void OnMMEvent(QuestEvent questEvent)
    {
        switch (questEvent.questEventType)
        {
            case QuestEventType.OnStartQuest:
                StartQuest(questEvent.questId);
                break;
            case QuestEventType.OnAdvanceQuest:
                AdvanceQuest(questEvent.questId);
                break;
            case QuestEventType.OnFinishQuest:
                FinishQuest(questEvent.questId);
                break;
        }
    }

    private void StartQuest(string id)
    {
        Debug.Log("StartQuest: " + id);
        Quest quest = GetQuestById(id);
        quest.InstantiateCurrentQuestStep(this.transform);
        quest.State = QuestState.IN_PROGRESS;
        
        //TODO: For Debug
        QuestStateUIDebugger.Instance.Quests.Add(quest);
        QuestStateUIDebugger.Instance.QuestUpdate();
    }

    private void AdvanceQuest(string id)
    {
        Quest quest = GetQuestById(id);
        // move on to the next step
        quest.MoveToNextStep();
        
        // if there are more steps, instantiate the next one
        if (quest.CurrentStepExists())
        {
            quest.InstantiateCurrentQuestStep(this.transform);
        }
        // if there are no more steps, then we've finished all of them for this quest
        else
        {
            FinishQuest(id);
        }
        
    }

    private void FinishQuest(string id)
    {
        Debug.Log("FinishQuest: " + id);
        Quest quest = GetQuestById(id);
        quest.State = QuestState.FINISHED;
        
        //TODO: For Debug
        QuestStateUIDebugger.Instance.QuestUpdate();
    }

    private Dictionary<string, Quest> CreateQuestMap()
    {
        // Assets/Resources/Quests
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");

        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach (QuestInfoSO questInfo in allQuests)
        {
            if (idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning("Duplicate ID found when creating quest map: " + questInfo.id);
            }
            idToQuestMap.Add(questInfo.id, new Quest(questInfo));
        }

        return idToQuestMap;
    }

    private Quest GetQuestById(string id)
    {
        Quest quest = _questMap[id];
        if (quest == null)
        {
            Debug.LogError("ID not found in the Quest Map: "+ id);
        }

        return quest;
    }
}
