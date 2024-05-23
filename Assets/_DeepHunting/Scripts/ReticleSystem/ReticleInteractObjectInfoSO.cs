using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ReticleInteractObjectInfoSO", menuName = "ScriptableObjects/ReticleInteractObjectInfoSO", order = 2)]
public class ReticleInteractObjectInfoSO : ScriptableObject
{
    [field: SerializeField] public string Id { get; private set; }
    public int LayerIndex = 0;
    
    [Header("General")]
    public List<string> DisplayName;
    public List<string> FarRangeComment = null;
    public List<string> CloseRangeComment = null;
    
    private void OnValidate()
    {
#if UNITY_EDITOR
        Id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
