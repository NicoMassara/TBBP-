using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Main.Scripts.Character
{
    [RequireComponent(typeof(CharacterMotor))]
    public class CharacterInputs : MonoBehaviour
    {
        private CharacterMotor _motor;
        private CharacterMovemetInputAction _inputAction;
        private CharacterMovemetInputAction.MovementActions _movementAction;

        private float _horizontalAxis;

        private void Awake()
        {
            _motor = GetComponent<CharacterMotor>();
        }

        private void Start()
        {
            _inputAction = new CharacterMovemetInputAction();
            _movementAction = _inputAction.Movement;
            
            _movementAction.Enable();
            
            //Handlers
            _movementAction.Bubble.performed += IA_Movement_Bubble_PerformedHandler;
            _movementAction.Jump.performed += IA_Movement_Jump_PerformedHandler;
        }

        private void IA_Movement_Bubble_PerformedHandler(InputAction.CallbackContext obj)
        {
            
        }
        
        private void IA_Movement_Jump_PerformedHandler(InputAction.CallbackContext obj)
        {
            _motor.DoJump();
        }

        private void Update()
        {
            var hAxis = _movementAction.HorizontalMovement.ReadValue<float>();
            _motor.MoveRight(hAxis);
        }
        
        private void FixedUpdate()
        {

        }
    }
}