
using System;
using UnityEngine;

namespace _Main.Scripts.Character
{
    [RequireComponent(typeof(CharacterMovement))]
    public class CharacterMotor : MonoBehaviour
    {
        private CharacterMovement _movement;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            _movement = GetComponent<CharacterMovement>();
        }

        private void Start()
        {
            _movement.OnFacingChanged += Movement_OnFacingChangeHandler;
        }
        public void MoveRight(float direction)
        {
            _movement.MoveRight(direction);
        }

        public void DoJump()
        {
            _movement.DoJump();
        }
        
        
        private void Movement_OnFacingChangeHandler(bool isFacingRight)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = !isFacingRight;
            }
        }
    }
}