using System;
using _Main.Scripts._Tools.DebugManager;
using _Main.Scripts.Bubble;
using UnityEngine;

namespace _Main.Scripts.Character
{
    [RequireComponent(typeof(CharacterMovement))]
    [RequireComponent(typeof(BubbleShooter))]
    public class CharacterMotor : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        private CharacterMovement _movement;
        private BubbleShooter _bubbleShooter;
        private bool _isFacingRight;

        private void Awake()
        {
            _movement = GetComponent<CharacterMovement>();
            _bubbleShooter = GetComponent<BubbleShooter>();
        }

        private void Start()
        {
            _movement.OnFacingChanged += Movement_OnFacingChangeHandler;
            _isFacingRight = true;

        }
        public void MoveRight(float direction)
        {
            _movement.MoveRight(direction);
        }

        public void DoJump()
        {
            _movement.DoJump();
        }

        public void DoShootBubble()
        {
            _bubbleShooter.TryShoot(_isFacingRight);
        }

        private void Movement_OnFacingChangeHandler(bool isFacingRight)
        {
            _isFacingRight = isFacingRight;
            
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = !_isFacingRight;
            }
        }

        private void OnEnable()
        {
            DebugManager.Instance.AddDebug(DebugKeys.Player.Velocity,
                "Player Velocity", () => _movement.Velocity.ToString());
            DebugManager.Instance.AddDebug(DebugKeys.Player.InAir,
                "In Air", () => _movement.IsInAir.ToString());
            DebugManager.Instance.AddDebug(DebugKeys.Player.Drag, "Drag", () => _movement.BodyDrag.ToString());
        }

        private void OnDisable()
        {
            DebugManager.Instance.RemoveDebug(DebugKeys.Player.Velocity);
            DebugManager.Instance.RemoveDebug(DebugKeys.Player.InAir);
        }
    }
}