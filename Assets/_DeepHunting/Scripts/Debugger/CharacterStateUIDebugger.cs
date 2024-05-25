using System.Collections;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;

public class CharacterStateUIDebugger : MonoBehaviour
{
    protected TextMeshProUGUI _UI;
 
    protected Character _character;
    protected Health _health;
    protected Stamina _stamina;
    protected MMStateMachine<CharacterStates.MovementStates> _movement;
    protected MMStateMachine<CharacterStates.CharacterConditions> _condition;
    protected MMStateMachine<CharacterStates.ExhaustedState> _exhausted;
    protected CharacterHorizontalMovement _characterHorizontalMovement;
    protected CorgiController _controller;
    
    void Start()
    {
        if(GetComponent<TextMeshProUGUI>()==null)
        {
            Debug.LogWarning ("CharacterStateUIDebugger requires a TextMeshProUGUI component.");
            return;
        }
        _UI = GetComponent<TextMeshProUGUI>();
    }
        
    void Update()
    {
        if (_character == null)
        {
            _character = FindObjectOfType<Character>();
            _health = _character.CharacterHealth;
            _stamina = _character.gameObject.GetComponent<Stamina>();
            _movement = _character.MovementState;
            _condition = _character.ConditionState;
            _exhausted = _character.ExhaustedState;
            _characterHorizontalMovement = _character.GetComponent<CharacterHorizontalMovement>();
            _controller = _character.GetComponent<CorgiController>();
        }

        _UI.text = "Movement State: " + _movement.CurrentState.ToString() + "\n" + 
                   "Condition State: " + _condition.CurrentState.ToString() + "\n" +
                   "Exhausted State: " + _exhausted.CurrentState.ToString() + "\n" +
                   "Speed: " + _controller.Speed.ToString() + "\n" +
                   "Force: " + _controller.ForcesApplied.ToString() + "\n" +
                   "MovementSpeed: " + _characterHorizontalMovement.MovementSpeed.ToString() + "\n" +
                   "HorizontalMovementForce: " + _characterHorizontalMovement.HorizontalMovementForce.ToString() + "\n" +
                   "Health: " + _health.CurrentHealth + " / " + _health.MaximumHealth + "\n" +
                   "Stamina: " + _stamina.CurrentStamina + " / " + _stamina.MaximumStamina;
    }
}
