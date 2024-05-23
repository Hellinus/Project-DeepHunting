using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public struct ActionEvent
{
    public string ActionName;
    
    public ActionEvent(string newName)
    {
        ActionName = newName;
    }
    
    static ActionEvent e;
    
    public static void Trigger(string newName)
    {
        e.ActionName = newName;
        MMEventManager.TriggerEvent(e);
    }
}