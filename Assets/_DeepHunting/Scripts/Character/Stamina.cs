using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;

public struct StaminaChangeEvent
{
	public Stamina AffectedStamina;
	public float NewStamina;
		
	public StaminaChangeEvent(Stamina affectedStamina, float newStamina)
	{
		AffectedStamina = affectedStamina;
		NewStamina = newStamina;
	}

	static StaminaChangeEvent e;
	public static void Trigger(Stamina affectedStamina, float newStamina)
	{
		e.AffectedStamina = affectedStamina;
		e.NewStamina = newStamina;
		MMEventManager.TriggerEvent(e);
	}
}

public class Stamina : MMMonoBehaviour
{
    [MMInspectorGroup("Status", true, 1)]
		
    /// the current stamina of the character
    [MMReadOnly] [Tooltip("the current stamina of the character")]
    public float CurrentStamina;
    [MMReadOnly] public bool TemporarilyNeedNoStamina = false;
    
    		
    [MMInspectorGroup("Stamina", true, 2)]
		
    /// the initial amount of stamina of the object
    [Tooltip("the initial amount of stamina of the object")]
    public float InitialStamina = 10;

    /// the maximum amount of stamina of the object
    [Tooltip("the maximum amount of stamina of the object")]
    public float MaximumStamina = 10;
    
    public bool NeedNoStamina = false;

    [MMInspectorGroup("Control", true, 3)]
    
    [Header("Jump")]
    public float JumpCost = 4f;

    public bool ShouldRecoverWaitAfterJump = true;
    
    [Header("Dash")]
    public float DashCost = 0f;
    
    public bool ShouldRecoverWaitAfterDash = false;
    
    [Header("Roll")]
    public float RollCost = 6f;
    
    public bool ShouldRecoverWaitAfterRoll = true;
    
    [Header("Run")]

    public float RunConstantlyCost = 3f;
    
    public bool ShouldRecoverWaitAfterRun = true;
    
    [Header("Climb")]
    
    public float ClimbConstanlyCost = 3f;
    
    public bool ShouldRecoverWaitAfterClimb = true;

    [MMInspectorGroup("Recover", true, 4)]
    
    public float RecoverSpeed = 2f;

    public float RecoverWaitTime = 1f;

    public float MinExhaustLimit = 10f;
    
    
    
    protected Character _character;
    protected CorgiController _controller;
    protected GameObject _thisObject;
    protected CharacterPersistence _characterPersistence = null;
    protected MMStateMachine<CharacterStates.MovementStates> _movement;
    protected MMStateMachine<CharacterStates.CharacterConditions> _condition;

    protected bool _isDashed = false;
    protected bool _isJumped = false;
    protected bool _isRolled = false;
    protected bool _shouldWait = false;
    protected float _shouldWaitCurretTime = 0f;
    
    protected virtual void Start()
    {
	    Initialization();
	    UpdateStamina(InitialStamina);
    }

    protected virtual void Initialization()
    {
	    _character = this.gameObject.GetComponent<Character>();
	    if (_character != null)
	    {
		    _thisObject = _character.gameObject;
		    _characterPersistence = _character.FindAbility<CharacterPersistence>();	
	    }
	    else
	    {
		    _thisObject = this.gameObject;
	    }
	    
	    _controller = _thisObject.GetComponent<CorgiController>();
	    _movement = _character.MovementState;
	    _condition = _character.ConditionState;
    }
    
    protected virtual void Update()
    {
	    HandleCost();
	    HandleExhaust();
	    HandleRecover();
    }

    protected virtual void HandleCost()
    {
	    if (NeedNoStamina || TemporarilyNeedNoStamina) return;
	    
	    if (_movement.CurrentState == CharacterStates.MovementStates.Dashing)
	    {
		    if (!_isDashed)
		    {
			    UpdateStamina(CurrentStamina -= DashCost);
			    _isDashed = true;
			    if (ShouldRecoverWaitAfterDash)
			    {
				    _shouldWait = true;
				    _shouldWaitCurretTime = 0f;
			    }
		    }
	    }
	    else
	    {
		    _isDashed = false;
	    }

	    if (_movement.CurrentState == CharacterStates.MovementStates.Jumping)
	    {
		    if (!_isJumped)
		    {
			    UpdateStamina(CurrentStamina -= JumpCost);
			    _isJumped = true;
			    if (ShouldRecoverWaitAfterJump)
			    {
				    _shouldWait = true;
				    _shouldWaitCurretTime = 0f;
			    }
		    }
	    }
	    else
	    {
		    _isJumped = false;
	    }
	    
	    if (_movement.CurrentState == CharacterStates.MovementStates.Rolling)
	    {
		    if (!_isRolled)
		    {
			    UpdateStamina(CurrentStamina -= RollCost);
			    _isRolled = true;
			    if (ShouldRecoverWaitAfterRoll)
			    {
				    _shouldWait = true;
				    _shouldWaitCurretTime = 0f;
			    }
		    }
	    }
	    else
	    {
		    _isRolled = false;
	    }

	    if (_movement.CurrentState == CharacterStates.MovementStates.Running)
	    {
		    SetStamina(CurrentStamina -= Time.deltaTime * RunConstantlyCost);
		    if (ShouldRecoverWaitAfterRun)
		    {
			    _shouldWait = true;
			    _shouldWaitCurretTime = 0f;
		    }
		    if (_condition.CurrentState == CharacterStates.CharacterConditions.Exhausted)
		    {
			    _movement.ChangeState(CharacterStates.MovementStates.Idle);
			    _character.GetComponent<CharacterHorizontalMovement>().ResetHorizontalSpeed();
		    }
	    }

	    if (_movement.CurrentState == CharacterStates.MovementStates.LadderClimbing)
	    {
		    SetStamina(CurrentStamina -= Time.deltaTime * ClimbConstanlyCost);
		    if (ShouldRecoverWaitAfterClimb)
		    {
			    _shouldWait = true;
			    _shouldWaitCurretTime = 0f;
		    }
		    if (_condition.CurrentState == CharacterStates.CharacterConditions.Exhausted)
		    {
			    _character.GetComponent<CharacterLadder>().GetOffTheLadder();
		    }
	    }
    }
    
    protected virtual void HandleExhaust()
    {
	    if (CurrentStamina <= 0 
	        && (_condition.CurrentState is CharacterStates.CharacterConditions.Normal or CharacterStates.CharacterConditions.ControlledMovement ))
	    {
		    _condition.ChangeState(CharacterStates.CharacterConditions.Exhausted);
		    // GUIManager.Instance.ExhaustedFeedbacks.PlayFeedbacks();
	    }
	    if (_condition.CurrentState == CharacterStates.CharacterConditions.Exhausted && CurrentStamina >= MinExhaustLimit)
	    {
		    _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
		    GUIManager.Instance.RecoverFromExhaustedFeedbacks.PlayFeedbacks();
	    }
    }
    
    protected virtual void HandleRecover()
    {
	    if (_condition.CurrentState is CharacterStates.CharacterConditions.Dead
	        or CharacterStates.CharacterConditions.Paused
	        or CharacterStates.CharacterConditions.Stunned
	       )
	    {
		    return;
	    }
	    
	    if (_movement.CurrentState != CharacterStates.MovementStates.Running)
	    {
		    if (CurrentStamina <= MaximumStamina && !_shouldWait)
		    {
			    SetStamina(CurrentStamina += RecoverSpeed * Time.deltaTime);
		    }
	    
		    if (_shouldWait)
		    {
			    _shouldWaitCurretTime += Time.deltaTime;
			    if (_shouldWaitCurretTime >= RecoverWaitTime)
			    {
				    _shouldWait = false;
				    _shouldWaitCurretTime = 0f;
			    }
		    }
	    }
    }
    
    public virtual void UpdateStamina(float newStamina)
    {
	    CurrentStamina = Mathf.Min(newStamina, MaximumStamina);
	    CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, MaximumStamina);
	    if (CurrentStamina <= 0f)
	    {
		    GUIManager.Instance.ExhaustedFeedbacks.PlayFeedbacks();
	    }
	    UpdateStaminaBar();
	    StaminaChangeEvent.Trigger(this, newStamina);
    }
    
    public virtual void SetStamina(float newStamina)
    {
	    CurrentStamina = Mathf.Min(newStamina, MaximumStamina);
	    CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, MaximumStamina);
	    if (CurrentStamina <= 0f)
	    {
		    GUIManager.Instance.ExhaustedFeedbacks.PlayFeedbacks();
	    }
	    SetStaminaBar();
	    StaminaChangeEvent.Trigger(this, newStamina);
    }
    
    public virtual void ResetStaminaToMaxHealth()
    {
	    CurrentStamina = MaximumStamina;
	    UpdateStaminaBar();
	    StaminaChangeEvent.Trigger(this, CurrentStamina);
    }
    
    public virtual void UpdateStaminaBar()
    {
	    if (_character != null)
	    {
		    if (_character.CharacterType == Character.CharacterTypes.Player)
		    {
			    // We update the stamina bar
			    if (GUIManager.HasInstance)
			    {
				    GUIManager.Instance.UpdateStaminaBar(CurrentStamina, 0f, MaximumStamina, _character.PlayerID);
			    }
		    }
	    }
    }
    
    public virtual void SetStaminaBar()
    {
	    if (_character != null)
	    {
		    if (_character.CharacterType == Character.CharacterTypes.Player)
		    {
			    // We update the stamina bar
			    if (GUIManager.HasInstance)
			    {
				    GUIManager.Instance.SetStaminaBar(CurrentStamina, 0f, MaximumStamina, _character.PlayerID);
			    }
		    }
	    }
    }

    public void StaminaCostEnabled()
    {
	    TemporarilyNeedNoStamina = false;
    }
    
    public void StaminaCostDisabled()
    {
	    TemporarilyNeedNoStamina = true;
    }
}
