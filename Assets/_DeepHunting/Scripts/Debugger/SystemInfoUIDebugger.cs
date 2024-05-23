using System.Collections;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using TMPro;
using UnityEngine;

public class SystemInfoUIDebugger : MonoBehaviour
{
    protected TextMeshProUGUI _UI;
    protected string _deviceUniqueIdentifier;
    protected string _deviceName;
        
    void Start()
    {
        if(GetComponent<TextMeshProUGUI>()==null)
        {
            Debug.LogWarning ("WaterMarkUIDebugger requires a TextMeshProUGUI component.");
            return;
        }
        _UI = GetComponent<TextMeshProUGUI>();

        _deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
        _deviceName = SystemInfo.deviceName;
    }
        
    void Update()
    {
        _UI.text = System.DateTime.Now.ToString() + "   |   " + _deviceUniqueIdentifier + "   |   " + _deviceName;
    }
}
