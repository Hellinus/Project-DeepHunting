using System.Collections;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using TMPro;
using UnityEngine;

public class AnimatorUIDebugger : MonoBehaviour
{
    protected Character _character;
    protected TextMeshProUGUI _UI;
        
    void Start()
    {
        if(GetComponent<TextMeshProUGUI>()==null)
        {
            Debug.LogWarning ("AnimatorDebugger requires a TextMeshProUGUI component.");
            return;
        }
        _UI = GetComponent<TextMeshProUGUI>();
    }
        
    void Update()
    {
        if (_character == null)
        {
            _character = FindObjectOfType<Character>();
        }


        AnimatorClipInfo[] m_CurrentClipInfo = _character.GetComponentInChildren<Animator>().GetCurrentAnimatorClipInfo(0);
        if (m_CurrentClipInfo.Length > 0)
        {
            _UI.text = m_CurrentClipInfo[0].clip.name;
        }
    }
}
