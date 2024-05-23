using System.Collections;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using TMPro;
using UnityEngine;

public class FPSUIDebugger : MonoBehaviour
{
    public float m_UpdateShowDeltaTime = 0.5f; //更新帧率的时间间隔; 
    protected TextMeshProUGUI _UI;

    private float m_LastUpdateShowTime = 0f; //上一次更新帧率的时间;  
    private int m_FrameUpdate = 0; //帧数;  
    private float m_FPS = 0; //帧率

    void Start()
    {
        if (GetComponent<TextMeshProUGUI>() == null)
        {
            Debug.LogWarning("FPSUIDebugger requires a TextMeshProUGUI component.");
            return;
        }

        _UI = GetComponent<TextMeshProUGUI>();
        m_LastUpdateShowTime = Time.realtimeSinceStartup;
    }

    private void Update()
    {
        m_FrameUpdate++;
        if (Time.realtimeSinceStartup - m_LastUpdateShowTime >= m_UpdateShowDeltaTime)
        {
            //FPS = 某段时间内的总帧数 / 某段时间
            m_FPS = m_FrameUpdate / (Time.realtimeSinceStartup - m_LastUpdateShowTime);
            m_FrameUpdate = 0;
            m_LastUpdateShowTime = Time.realtimeSinceStartup;
            _UI.text = (1 / Time.deltaTime).ToString();
        }
    }
}
