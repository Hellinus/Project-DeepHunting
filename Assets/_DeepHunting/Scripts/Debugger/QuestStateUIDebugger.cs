using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using TMPro;
using UnityEditor;
using UnityEngine;

public class QuestStateUIDebugger : MMSingleton<QuestStateUIDebugger>
{
    public List<Quest> Quests = new List<Quest>();

    private TextMeshProUGUI _UI;

    private void Start()
    {
        _UI = GetComponent<TextMeshProUGUI>();
        Quests.Clear();
    }

    public void QuestUpdate()
    {
        _UI.text = "";
        foreach (var VARIABLE in Quests)
        {
            _UI.text = _UI.text + "\n" + VARIABLE.Info.id + ": " + VARIABLE.State;
        }
        
        // foreach (var VARIABLE in Quests)
        // {
        //     switch (VARIABLE.State)
        //     {
        //         case QuestState.REQUIREMENTS_NOT_MET:
        //             Debug.LogAssertion("Error");
        //             break;
        //         case QuestState.IN_PROGRESS:
        //             _UI.text = _UI.text + "" + VARIABLE.Info.id + ": " + VARIABLE.State;
        //             break;
        //         case QuestState.FINISHED:
        //             Quests.Remove(VARIABLE);
        //             break;
        //     }
        // }
    }
}
