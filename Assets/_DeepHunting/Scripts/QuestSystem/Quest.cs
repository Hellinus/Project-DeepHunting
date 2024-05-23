using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Quest
{
    // static info
    public QuestInfoSO Info;
    
    // state info
    public QuestState State;
    private int _currentQuestStepIndex;

    public Quest(QuestInfoSO questInfo)
    {
        this.Info = questInfo;
        this.State = QuestState.REQUIREMENTS_NOT_MET;
        this._currentQuestStepIndex = 0;
    }

    public void MoveToNextStep()
    {
        _currentQuestStepIndex++;
    }

    public bool CurrentStepExists()
    {
        return (_currentQuestStepIndex < Info.questStepPrefabs.Length);
    }

    public void InstantiateCurrentQuestStep(Transform parentTransform)
    {
        GameObject questStepPrefab = GetCurrentQuestStepPrefab();
        if (questStepPrefab != null)
        {
            QuestStep questStep = Object.Instantiate<GameObject>(questStepPrefab, parentTransform).GetComponent<QuestStep>();
            questStep.InitializeQuestStep(Info.id);
        }
    }

    private GameObject GetCurrentQuestStepPrefab()
    {
        GameObject questStepPrefab = null;
        if (CurrentStepExists())
        {
            questStepPrefab = Info.questStepPrefabs[_currentQuestStepIndex];
        }
        else
        {
            Debug.LogWarning("Tried to get quest step prefab, but stepIndex was out of range,"
                + "there's no current step: QuestId=" + Info.id + ", stepIndex=" + _currentQuestStepIndex);
        }

        return questStepPrefab;
    }
    
    
    
}
