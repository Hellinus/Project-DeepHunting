using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackPath("Events/Quest Events")]
public class MMF_QuestEvent : MMF_Feedback
{
    /// a static bool used to disable all feedbacks of this type at once
    public static bool FeedbackTypeAuthorized = true;
    /// sets the inspector color for this feedback
#if UNITY_EDITOR
    public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.EventsColor; } }
#endif
    
    [MMFInspectorGroup("Quest Events", true, 17)]
    /// the type of event to trigger
    [Tooltip("the type of event to trigger")]
    public QuestEventType eventType = QuestEventType.OnStartQuest;
    public string questId;
    
    protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
    {
        if (Active)
        {
            QuestEvent.Trigger(eventType, questId);
        }
    }
}
