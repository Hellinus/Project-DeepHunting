using System;using System.Collections;
using System.Collections.Generic;
using InputIcons;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using PixelCrushers.DialogueSystem;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using LevelManager = MoreMountains.CorgiEngine.LevelManager;

public enum Reticle2DColliderInteractType
{
    Enter,
    Stay,
    Exit
}

public enum ReticleButtonState
{
    Down,
    Pressed,
    Up,
    Off
}

public class CharacterReticle : CharacterAbility, MMEventListener<CorgiEngineEvent>
{
    [Header("Reticle")]
    
    /// the gameobject to display as the aim's reticle/crosshair. Leave it blank if you don't want a reticle
    [Tooltip("the gameobject to display as the aim's reticle/crosshair. Leave it blank if you don't want a reticle")]
    public GameObject Reticle;
    public bool FaceReticleDirection = true;
    /// if set to false, the reticle won't be added and displayed
    [Tooltip("if set to false, the reticle won't be added and displayed")]
    public bool DisplayReticle = true;
    /// the distance at which the reticle will be from the weapon
    [Tooltip("the distance at which the reticle will be from the weapon")]
    [MMCondition("DisplayReticle")]
    public float ReticleDistance;
    /// the z position of the reticle
    [Tooltip("the z position of the reticle")]
    [MMCondition("DisplayReticle")]
    public float ReticleZPosition;
    public float ReticlePositionLerpSpeed = 0.5f;
    /// whether or not the reticle should be hidden when the character is dead
    [Tooltip("whether or not the reticle should be hidden when the character is dead")] [MMCondition("DisplayReticle")]
    public bool DisableReticleOnDeath = true;

    public float InteractDistance = 2f;
    
    [Range(1, 10)] public int GamepadMoveSpeed = 5;
    [Range(1, 10)] public int MouseMoveSpeed = 5;
    
    
    protected Vector3 _mousePosition;
    protected Camera _mainCamera;
    protected Vector3 _reticlePosition;
    protected GameObject _reticle;
    protected bool _charHztlMvmtFlipInitialSettingSet = false;
    protected bool _charHztlMvmtFlipInitialSetting;
    protected PlayerReticleObject _playerReticleObject;
    protected bool _isDraging = false;
    protected bool _isHiding = false;
    protected bool _shouldHide = false;
    
    [Header("Debug")]
    
    [MMReadOnly]public List<ReticleBase> _layerList;

    [MMReadOnly]public ReticleButtonState _reticleButtonState = ReticleButtonState.Off;
    [MMReadOnly]public ReticleBase _prePressedReticleBase = null;
    [MMReadOnly]public ReticleButtonState _preReticleButtonState = ReticleButtonState.Off;
    [MMReadOnly]public ReticleBase _preCommentReticleBase = null;
    
    protected override void Initialization()
    {
        base.Initialization();
        
        _mainCamera = Camera.main;
        
        if (Reticle == null) { return; }
        if (!DisplayReticle) { return; }

        InitiateReticle();


        
        _layerList = new List<ReticleBase>();
        
        if (_characterHorizontalMovement != null)
        {
            _charHztlMvmtFlipInitialSetting = _characterHorizontalMovement.FlipCharacterToFaceDirection;
        }
    }

    protected override void HandleInput()
    {
        if (_inputManager.ReticleClickButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
        {
            _reticleButtonState = ReticleButtonState.Down;
        }
        else if (_inputManager.ReticleClickButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed)
        {
            _reticleButtonState = ReticleButtonState.Pressed;
        }
        else if (_inputManager.ReticleClickButton.State.CurrentState == MMInput.ButtonStates.ButtonUp)
        {
            _reticleButtonState = ReticleButtonState.Up;
        }
        else
        {
            _reticleButtonState = ReticleButtonState.Off;
        }
    }
    
    public override void ProcessAbility()
    {
        base.ProcessAbility();
        
        GetCurrentAim ();
        MoveReticle();
        HideReticle ();

        if (!_isHiding)
        {
            HandleFacingDirection();
            HandleDrag();
            HandleComment();
            HandleDisplayName();
            HandleClickActionTrigger();
        }
    }

    protected virtual void GetCurrentAim()
    {
        if (InputIconsManagerSO.CurrentInputDeviceIsKeyboard())
        {
            _mousePosition.x += _inputManager.ReticleMovement.x * (Screen.width / 2000f) * MouseMoveSpeed;
            _mousePosition.x = Mathf.Clamp(_mousePosition.x, 0f, Screen.width);
        
            _mousePosition.y += _inputManager.ReticleMovement.y * (Screen.height / 2000f) * MouseMoveSpeed;
            _mousePosition.y = Mathf.Clamp(_mousePosition.y, 0f, Screen.height);
            
            _mousePosition.z = _mainCamera.transform.position.z * -1;
        }
        else
        {
            _mousePosition.x += _inputManager.ReticleMovement.x * (Screen.width / 1000f) * GamepadMoveSpeed;
            _mousePosition.x = Mathf.Clamp(_mousePosition.x, 0f, Screen.width);
        
            _mousePosition.y += _inputManager.ReticleMovement.y * (Screen.height / 1000f) * GamepadMoveSpeed;
            _mousePosition.y = Mathf.Clamp(_mousePosition.y, 0f, Screen.height);
        
            _mousePosition.z = _mainCamera.transform.position.z * -1;
        }
    }
    
    protected virtual void MoveReticle()
    {
        if (_reticle == null) { return; }

        _reticlePosition = _mainCamera.ScreenToWorldPoint (_mousePosition);
        _reticlePosition.z = ReticleZPosition;

        _reticle.transform.position = Vector3.Lerp(_reticle.transform.position, _reticlePosition, ReticlePositionLerpSpeed);

    }
    
    protected virtual void HideReticle()
    {
        if (DisableReticleOnDeath && (_reticle != null))
        {
            if (LevelManager.Instance.Players[0] != null)
            {
                if (LevelManager.Instance.Players[0].ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead)
                {
                    _reticle.gameObject.SetActive(false);
                    _isHiding = true;
                }
            }
        }
    }

    public void SetReticleHideState(bool b)
    {
        // Debug.Log(b);
        _reticle.gameObject.SetActive(b);
        _isHiding = !b;
    }
    
    /// <summary>
    /// If FaceWeaponDirection is true, will force the character to face the weapon direction
    /// </summary>
    protected virtual void HandleFacingDirection()
    {
        if (!FaceReticleDirection
            || _condition.CurrentState == CharacterStates.CharacterConditions.ControlledMovement
            || _condition.CurrentState == CharacterStates.CharacterConditions.Frozen
            || _condition.CurrentState == CharacterStates.CharacterConditions.Dead)
        {
            return;
        }
        
        if (_characterHorizontalMovement != null)
        {
            _characterHorizontalMovement.FlipCharacterToFaceDirection = false;
            _charHztlMvmtFlipInitialSettingSet = true;
        }

        if (_reticle.transform.position.x > _character.transform.position.x)
        {
            if (!_character.IsFacingRight)
            {
                _character.Face(Character.FacingDirections.Right);
            }
        }
        else if (_reticle.transform.position.x < _character.transform.position.x)
        {
            if (_character.IsFacingRight)
            {
                _character.Face(Character.FacingDirections.Left);
            }
        }
    }

    public void SetReticleUseableState(bool b)
    {
        SetReticleHideState(b);
        _shouldHide = !b;
        if (b == false)
        {
            if (_characterHorizontalMovement != null)
            {
                _characterHorizontalMovement.FlipCharacterToFaceDirection = true;
                _charHztlMvmtFlipInitialSettingSet = false;
            }
        }
    }

    public virtual void ReticleColliderInteract(Reticle2DColliderInteractType t, ReticleBase r)
    {
        switch (t)
        {
            case Reticle2DColliderInteractType.Enter:
                ReticleEnterSomething(r);
                break;
            case Reticle2DColliderInteractType.Stay:
                ReticleStaySomething(r);
                break;
            case Reticle2DColliderInteractType.Exit:
                ReticleLeaveSomething(r);
                break;
            default:
                break;
        }
    }

    public void ReticleEnterSomething(ReticleBase r)
    {
        r.OnReticleEnter();
        LayerIndexListAdd(r);
    }

    public void ReticleStaySomething(ReticleBase r)
    {
        switch (_reticleButtonState)
        {
            case ReticleButtonState.Down:
                if (_layerList.Count != 0)
                {
                    _layerList[0].OnReticleButtonDown();
                    _prePressedReticleBase = _layerList[0];
                    _preReticleButtonState = ReticleButtonState.Down;
                }
                break;
            case ReticleButtonState.Pressed:
                if (_layerList.Count != 0)
                {
                    if (_preReticleButtonState == ReticleButtonState.Pressed)
                    {
                        if (_prePressedReticleBase == null)
                        {
                            _prePressedReticleBase = _layerList[0];
                        }
                    }
                    else
                    {
                        _prePressedReticleBase = _layerList[0];
                        _preReticleButtonState = ReticleButtonState.Pressed;
                    }
                }
                break;
            case ReticleButtonState.Up:
                if (_layerList.Count != 0)
                {
                    _prePressedReticleBase.OnReticleButtonUp();
                    _preReticleButtonState = ReticleButtonState.Up;
                }
                break;
            case ReticleButtonState.Off:
                if (_layerList.Count != 0)
                {
                    _layerList[0].OnReticleButtonHighlight();
                }
                _preReticleButtonState = ReticleButtonState.Off;
                break;
        }
    }

    public void ReticleLeaveSomething(ReticleBase r)
    {
        r.OnReticleLeave();
        LayerIndexListRemove(r);
    }

    protected void HandleDrag()
    {
        if (_prePressedReticleBase && _prePressedReticleBase.ReticlePressEventType == PressEventType.Drag)
        {
            if (_preReticleButtonState == ReticleButtonState.Pressed)
            {
                if (_reticleButtonState == ReticleButtonState.Off)
                {
                    _prePressedReticleBase = null;
                    _isDraging = false;
                    return;
                }
                _prePressedReticleBase.OnReticleButtonPress(_reticle.transform.position);
                _isDraging = true;
            }
            else
            {
                _isDraging = false;
            }
        }
        else
        {
            _isDraging = false;
        }
    }

    protected void HandleClickActionTrigger()
    {
        if (_prePressedReticleBase && _prePressedReticleBase.ReticlePressEventType == PressEventType.ClickTriggerAction)
        {
            if (_preReticleButtonState == ReticleButtonState.Pressed)
            {
                if (_reticleButtonState == ReticleButtonState.Off)
                {
                    _prePressedReticleBase = null;
                    return;
                }
                _prePressedReticleBase.OnReticleButtonPress(_reticle.transform.position);
            }
        }

    }

    protected void HandleComment()
    {
        if (_prePressedReticleBase && _prePressedReticleBase.ReticlePressEventType != PressEventType.Drag)
        {
            if (_preReticleButtonState == ReticleButtonState.Pressed)
            {
                if (Vector3.Distance(_character.transform.position, _prePressedReticleBase.transform.position) > InteractDistance)
                {
                    if (_layerList.Count <= 0)
                    {
                        _preCommentReticleBase = null;
                        _playerReticleObject.ClearComment();
                    }
                    else if (_layerList[0] != _preCommentReticleBase)
                    {
                        // _preCommentReticleBase = _layerList[0];
                        if (_layerList[0].ReticlePressEventType != PressEventType.Drag && _reticleButtonState == ReticleButtonState.Pressed)
                        {
                            _preCommentReticleBase = _layerList[0];
                            _playerReticleObject.ShowComment(GetCurrentLanguageFarRangeComment(_preCommentReticleBase));
                        }
                        else
                        {
                            _preCommentReticleBase = null;
                            _playerReticleObject.ClearComment();
                        }
                    }
                }
                else
                {
                    if (_layerList.Count <= 0)
                    {
                        _preCommentReticleBase = null;
                        _playerReticleObject.ClearComment();
                    }
                    else if (_layerList[0] != _preCommentReticleBase)
                    {
                        // _preCommentReticleBase = _layerList[0];
                        if (_layerList[0].ReticlePressEventType != PressEventType.Drag && _reticleButtonState == ReticleButtonState.Pressed)
                        {
                            _preCommentReticleBase = _layerList[0];
                            _playerReticleObject.ShowComment(GetCurrentLanguageCloseRangeComment(_preCommentReticleBase));
                        }
                        else
                        {
                            _preCommentReticleBase = null;
                            _playerReticleObject.ClearComment();
                        }
                    }
                }
            }
        }
        
        if (_reticleButtonState == ReticleButtonState.Off || _reticleButtonState == ReticleButtonState.Up)
        {
            if (_preCommentReticleBase)
            {
                if (_layerList.Count > 0)
                {
                    if (_preCommentReticleBase != _layerList[0])
                    {
                        _preCommentReticleBase = null;
                        _playerReticleObject.ClearComment();
                    }
                }
                else
                {
                    _preCommentReticleBase = null;
                    _playerReticleObject.ClearComment();
                }
            }
        }
    }

    protected void HandleDisplayName()
    {
        if (!_isDraging)
        {
            UpdateDisplayName();
        }
    }

    protected void UpdateDisplayName()
    {
        if (_layerList.Count == 0)
        {
            _playerReticleObject.ClearName();
            _playerReticleObject.ClearHitButton();
        }
        else if (_layerList.Count != 0 && _layerList[0].ShowName)
        {
            _playerReticleObject.ShowName(GetCurrentLanguageDisplayName(_layerList[0]));
            _playerReticleObject.ShowHitButton();
        }
    }

    protected string GetCurrentLanguageDisplayName(ReticleBase r)
    {
        switch (I2.Loc.LocalizationManager.CurrentLanguage)
        {
            case "English":
                if (_layerList[0].ObjectInfo.DisplayName.Count >= 1)
                {
                    return r.ObjectInfo.DisplayName[0];
                }
                else
                {
                    Debug.LogError("DisplayName missing: " + _layerList[0].ObjectInfo.Id);
                }
                break;
            case "Chinese (Simplified)":
                if (_layerList[0].ObjectInfo.DisplayName.Count >= 2)
                {
                    return r.ObjectInfo.DisplayName[1];
                }
                else
                {
                    Debug.LogError("DisplayName missing: " + _layerList[0].ObjectInfo.Id);
                }
                break;
            case "Chinese (Traditional)":
                if (_layerList[0].ObjectInfo.DisplayName.Count >= 3)
                {
                    return r.ObjectInfo.DisplayName[2];
                }
                else
                {
                    Debug.LogError("DisplayName missing: " + _layerList[0].ObjectInfo.Id);
                }
                break;
        }
        Debug.LogError("DisplayName Language missing: " + _layerList[0].ObjectInfo.Id);
        return "ERROR";
    }
    
    protected string GetCurrentLanguageFarRangeComment(ReticleBase r)
    {
        switch (I2.Loc.LocalizationManager.CurrentLanguage)
        {
            case "English":
                if (_layerList[0].ObjectInfo.FarRangeComment.Count >= 1)
                {
                    return r.ObjectInfo.FarRangeComment[0];
                }
                else
                {
                    Debug.LogError("PlayerComment missing: " + _layerList[0].ObjectInfo.Id);
                }
                break;
            case "Chinese (Simplified)":
                if (_layerList[0].ObjectInfo.FarRangeComment.Count >= 2)
                {
                    return r.ObjectInfo.FarRangeComment[1];
                }
                else
                {
                    Debug.LogError("PlayerComment missing: " + _layerList[0].ObjectInfo.Id);
                }
                break;
            case "Chinese (Traditional)":
                if (_layerList[0].ObjectInfo.FarRangeComment.Count >= 3)
                {
                    return r.ObjectInfo.FarRangeComment[2];
                }
                else
                {
                    Debug.LogError("PlayerComment missing: " + _layerList[0].ObjectInfo.Id);
                }
                break;
        }
        Debug.LogError("PlayerComment Language missing: " + _layerList[0].ObjectInfo.Id);
        return "ERROR";
    }
    
    protected string GetCurrentLanguageCloseRangeComment(ReticleBase r)
    {
        switch (I2.Loc.LocalizationManager.CurrentLanguage)
        {
            case "English":
                if (_layerList[0].ObjectInfo.CloseRangeComment.Count >= 1)
                {
                    return r.ObjectInfo.CloseRangeComment[0];
                }
                else
                {
                    Debug.LogError("PlayerComment missing: " + _layerList[0].ObjectInfo.Id);
                }
                break;
            case "Chinese (Simplified)":
                if (_layerList[0].ObjectInfo.CloseRangeComment.Count >= 2)
                {
                    return r.ObjectInfo.CloseRangeComment[1];
                }
                else
                {
                    Debug.LogError("PlayerComment missing: " + _layerList[0].ObjectInfo.Id);
                }
                break;
            case "Chinese (Traditional)":
                if (_layerList[0].ObjectInfo.CloseRangeComment.Count >= 3)
                {
                    return r.ObjectInfo.CloseRangeComment[2];
                }
                else
                {
                    Debug.LogError("PlayerComment missing: " + _layerList[0].ObjectInfo.Id);
                }
                break;
        }
        Debug.LogError("PlayerComment Language missing: " + _layerList[0].ObjectInfo.Id);
        return "ERROR";
    }

    protected void LayerIndexListAdd(ReticleBase r)
    {
        _layerList.Add(r);
        
        if (_layerList.Count != 0)
        {
            _layerList.Sort((x, y) => { return -x.ObjectInfo.LayerIndex.CompareTo(y.ObjectInfo.LayerIndex); });
        }
    }
    
    protected void LayerIndexListRemove(ReticleBase r)
    {
        if (_layerList.Remove(r))
        {
            _layerList.Sort((x, y) => { return -x.ObjectInfo.LayerIndex.CompareTo(y.ObjectInfo.LayerIndex); });
        }
    }
    
    protected void LayerIndexListClear(ReticleBase r)
    {
        _layerList.Clear();
    }

    public void OnMMEvent(CorgiEngineEvent eventType)
    {
        switch (eventType.EventType)
        {
            case CorgiEngineEventTypes.Pause:
                _reticle.gameObject.SetActive(false);
                _isHiding = true;
                break;
            case CorgiEngineEventTypes.UnPause:
                if (!_shouldHide)
                {
                    _reticle.gameObject.SetActive(true);
                    _isHiding = false;
                }
                break;
            case CorgiEngineEventTypes.PauseNoMenu:
                _reticle.gameObject.SetActive(false);
                _isHiding = true;
                break;
        }
    }

    protected virtual void InitiateReticle()
    {
        _reticle = Instantiate(Reticle);
        _isHiding = false;
        
        _playerReticleObject = _reticle.MMGetComponentNoAlloc<PlayerReticleObject>();
        _playerReticleObject.Reticle = this;
        _reticle.transform.localPosition = ReticleDistance * Vector3.right;
    }

    protected virtual void DeathDestroyReticle()
    {
        Destroy(_reticle);
    }

    protected override void OnRespawn()
    {
        base.OnRespawn();
        InitiateReticle();
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        DeathDestroyReticle();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.MMEventStartListening<CorgiEngineEvent>();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        this.MMEventStopListening<CorgiEngineEvent>();
    }
}
