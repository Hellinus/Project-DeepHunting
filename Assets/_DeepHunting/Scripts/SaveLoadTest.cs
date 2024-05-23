using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SaveLoadTest : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            ES3AutoSaveMgr.Current.Save();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            ES3AutoSaveMgr.Current.Load();
        }
    }
}
