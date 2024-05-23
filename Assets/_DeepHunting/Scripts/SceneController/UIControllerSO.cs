using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public struct UIControllerInfo
{
    public string Id;
    public float Value;
}

[CreateAssetMenu(fileName = "UIControllerSO", menuName = "ScriptableObjects/UIControllerSO", order = 2)]
public class UIControllerSO : ScriptableObject
{
    public float DefaultValue = 1f;
    public List<UIControllerInfo> Parameters;
}
