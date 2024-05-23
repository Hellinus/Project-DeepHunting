using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class HelpMomGetItQS : QuestStep
{
    public int i = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            i++;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Destroy(this.gameObject);
        }

        if (i > 5)
        {
            FinishQuestStep();
            ES3AutoSaveMgr.Current.Save();
            Destroy(this.gameObject);
        }
    }
}
