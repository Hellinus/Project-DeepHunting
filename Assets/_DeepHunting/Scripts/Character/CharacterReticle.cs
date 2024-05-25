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

public class CharacterReticle : CharacterAbility
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
    /// if set to true, the reticle will be placed at the mouse's position (like a pointer)
    [Tooltip("if set to true, the reticle will be placed at the mouse's position (like a pointer)")]
    [MMCondition("DisplayReticle")]
    public bool ReticleAtMousePosition;
    /// if set to true, the reticle will replace the mouse pointer
    [Tooltip("if set to true, the reticle will replace the mouse pointer")]
    [MMCondition("DisplayReticle")]
    public bool ReplaceMousePointer = true;

    /// whether or not the reticle should be hidden when the character is dead
    [Tooltip("whether or not the reticle should be hidden when the character is dead")] [MMCondition("DisplayReticle")]
    public bool DisableReticleOnDeath = true;

    public float InteractDistance = 2f;
    
    [Range(1, 10)] public int GamepadMoveSpeed = 5;
    [Range(1, 10)] public int MouseMoveSpeed = 5;
    
    
    protected Vector3 _currentAim = Vector3.zero;
    protected Vector3 _mousePosition;
    protected Vector3 _direction;
    protected Camera _mainCamera;
    protected Vector3 _reticlePosition;
    protected GameObject _reticle;
    protected bool _charHztlMvmtFlipInitialSettingSet = false;
    protected bool _charHztlMvmtFlipInitialSetting;
    protected PlayerReticleObject _playerReticleObject;
    protected bool _isDraging = false;
    
    [Header("Debug")]
    
    [MMReadOnly]public List<ReticleBase> _layerList;

    [MMReadOnly]public ReticleButtonState _reticleButtonState = ReticleButtonState.Off;
    [MMReadOnly]public ReticleBase _prePressedReticleBase = null;
    [MMReadOnly]public ReticleButtonState _preReticleButtonState = ReticleButtonState.Off;
    [MMReadOnly]public ReticleBase _preCommentReticleBase = null;
    
    protected override void Initialization()
    {
        base.Initialization();

        Cursor.lockState = CursorLockMode.Locked;
        
        _mainCamera = Camera.main;
        
        if (Reticle == null) { return; }
        if (!DisplayReticle) { return; }

        _reticle = (GameObject)Instantiate(Reticle);
        _playerReticleObject = _reticle.MMGetComponentNoAlloc<PlayerReticleObject>();
        _playerReticleObject.Reticle = this;
        
        if (LevelManager.Instance.Players[0] != null)
        {
            _reticle.transform.SetParent(LevelManager.Instance.Players[0].transform);
        }

        _reticle.transform.localPosition = ReticleDistance * Vector3.right;
        
        if (_characterHorizontalMovement != null)
        {
            _charHztlMvmtFlipInitialSetting = _characterHorizontalMovement.FlipCharacterToFaceDirection;
        }

        _layerList = new List<ReticleBase>();
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
        HandleFacingDirection();
        
        HandleDrag();
        HandleComment();
        HandleDisplayName();
    }

    protected virtual void GetCurrentAim()
    {
        if (InputIconsManagerSO.CurrentInputDeviceIsKeyboard())
        {
// #if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
//             _mousePosition = Input.mousePosition;
// #else
//         _mousePosition = Mouse.current.position.ReadValue();
// #endif
        
            _mousePosition.x += _inputManager.ReticleMovement.x * (Screen.width / 2000f) * MouseMoveSpeed;
            _mousePosition.x = Mathf.Clamp(_mousePosition.x, 0f, Screen.width);
        
            _mousePosition.y += _inputManager.ReticleMovement.y * (Screen.height / 2000f) * MouseMoveSpeed;
            _mousePosition.y = Mathf.Clamp(_mousePosition.y, 0f, Screen.height);
            
            _mousePosition.z = _mainCamera.transform.position.z * -1;

            _direction = _mainCamera.ScreenToWorldPoint (_mousePosition);
            _direction.z = transform.position.z;

            if (LevelManager.Instance.Players[0].IsFacingRight)
            {
                _currentAim = _direction - transform.position;
            }
            else
            {
                _currentAim = transform.position - _direction;
            }
        }
        else
        {
            _mousePosition.x += _inputManager.ReticleMovement.x * (Screen.width / 1000f) * GamepadMoveSpeed;
            _mousePosition.x = Mathf.Clamp(_mousePosition.x, 0f, Screen.width);
        
            _mousePosition.y += _inputManager.ReticleMovement.y * (Screen.height / 1000f) * GamepadMoveSpeed;
            _mousePosition.y = Mathf.Clamp(_mousePosition.y, 0f, Screen.height);
        
            _mousePosition.z = _mainCamera.transform.position.z * -1;

            _direction = _mainCamera.ScreenToWorldPoint (_mousePosition);
            // Debug.Log(_mousePosition);
        
            _direction.z = transform.position.z;

            if (LevelManager.Instance.Players[0].IsFacingRight)
            {
                _currentAim = _direction - transform.position;
            }
            else
            {
                _currentAim = transform.position - _direction;
            }
        }
    }
    
    protected virtual void MoveReticle()
    {
        if (_reticle == null) { return; }

        // if we're in follow mouse mode and the current control scheme is mouse, we move the reticle to the mouse's position
        if (ReticleAtMousePosition)
        {
            _reticlePosition = _mainCamera.ScreenToWorldPoint (_mousePosition);
            _reticlePosition.z = ReticleZPosition;

            _reticle.transform.position = Vector3.Lerp(_reticle.transform.position, _reticlePosition, ReticlePositionLerpSpeed);
        }
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
                }
                else
                {
                    _reticle.gameObject.SetActive(true);
                }
            }
        }

        if (GameManager.Instance.Paused)
        {
            _reticle.gameObject.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            return;
        }
        else
        {
            _reticle.gameObject.SetActive(true);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    
    /// <summary>
    /// If FaceWeaponDirection is true, will force the character to face the weapon direction
    /// </summary>
    protected virtual void HandleFacingDirection()
    {
        if ((_characterHorizontalMovement != null) && FaceReticleDirection)
        {
            _characterHorizontalMovement.FlipCharacterToFaceDirection = false;
            _charHztlMvmtFlipInitialSettingSet = true;
        }

        if (_charHztlMvmtFlipInitialSettingSet)
        {
            _characterHorizontalMovement.FlipCharacterToFaceDirection = _charHztlMvmtFlipInitialSetting;
        }

        // if we're not in FaceWeaponDirection mode, if we don't have a HztalMvmt ability, or a weapon aim, we do nothing and exit
        if (!FaceReticleDirection || (_characterHorizontalMovement == null))
        {
            return;
        }

        if (_character.IsFacingRight && _reticle.transform.position.x < _character.transform.position.x)
        {
            _character.Flip();
        }
        else if (!_character.IsFacingRight && _reticle.transform.position.x > _character.transform.position.x)
        {
            _character.Flip();
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
        // if (!_isDraging)
        // {
        //     if (_layerList.Count != 0
        //         && _layerList[0] != _prePressedReticleBase)
        //     {
        //         _prePressedReticleBase = null;
        //     }
        //     else
        //     {
        //         _prePressedReticleBase = null;
        //     }
        // }
    }

    protected void HandleDrag()
    {
        if (_prePressedReticleBase && _prePressedReticleBase.ReticlePressEventType == PressEventType.Drag)
        {
            if (_preReticleButtonState == ReticleButtonState.Pressed)
            {
                _prePressedReticleBase.OnReticleButtonPress(_reticle.transform.position);
                _isDraging = true;
            }
            else
            {
                _isDraging = false;
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
}
