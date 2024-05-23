using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using UnityEngine;
using MoreMountains.Tools;
using TMPro;
using Unity.VisualScripting;

public class PlayerReticleObject : MonoBehaviour
{
    public CharacterReticle Reticle;

    protected Canvas _canvas;
    protected TextMeshProUGUI _hitButton;
    protected TextMeshProUGUI _displayName;
    protected TextMeshProUGUI _comment;
    protected TMPPlayer.TMProPlayer _commentTMProPlayer;
    private void Start()
    {
        _canvas = GetComponentInChildren<Canvas>();
        _canvas.renderMode = RenderMode.WorldSpace;

        if (FindObjectOfType<GUIManager>() != null)
        {
            if (FindObjectOfType<GUIManager>().TryGetComponent<Camera>(out var camera))
            {
                _canvas.worldCamera = camera;
            }
        }
        
        TextMeshProUGUI[] l = _canvas.gameObject.GetComponentsInChildren<TextMeshProUGUI>();

        if (l.Length >= 3)
        {
            _hitButton = l[0];
            _displayName = l[1];
            _comment = l[2];
        }

        _commentTMProPlayer = gameObject.GetComponentInChildren<TMPPlayer.TMProPlayer>();

        ClearName();
        ClearComment();
        ClearHitButton();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<ReticleBase>(out var r))
        {
            Reticle.ReticleColliderInteract(Reticle2DColliderInteractType.Enter, r);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent<ReticleBase>(out var r))
        {
            Reticle.ReticleColliderInteract(Reticle2DColliderInteractType.Stay, r);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<ReticleBase>(out var r))
        {
            Reticle.ReticleColliderInteract(Reticle2DColliderInteractType.Exit, r);
        }
    }

    public void ShowName(string s)
    {
        _displayName.text = s;
    }

    public void ClearName()
    {
        _displayName.text = "";
    }
    
    public void ShowComment(string s)
    {
        _commentTMProPlayer.SetText(s);
    }

    public void ClearComment()
    {
        _comment.text = "";
    }

    public void ShowHitButton()
    {
        _hitButton.gameObject.SetActive(true);
    }

    public void ClearHitButton()
    {
        _hitButton.gameObject.SetActive(false);
    }
}
