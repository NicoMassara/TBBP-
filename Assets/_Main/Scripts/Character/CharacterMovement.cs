using System;
using _Main.Scripts._Tools.DebugManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace _Main.Scripts.Character
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterMovement : MonoBehaviour
    {
        [Header("Movement Values")]
        [Range(1, 10)]
        [SerializeField] private float hAcceleration = 5;
        [Range(0.1f,1)]
        [SerializeField] private float hAccelerationMultiplierInAir = 1;
        [Range(1, 10)]
        [SerializeField] private float maxHSpeed = 5;
        [Range(1, 30)]
        [SerializeField] private float jumpForce = 10;
        [Range(0,0.5f)]
        [SerializeField] private float checkForMaxHeightDelay = 0.5f;
        [Header("Drag Values")]
        [Range(1, 5)]
        [SerializeField] private float groundMaxDrag = 5;
        [Range(0, 2)]
        [SerializeField] private float groundMinDrag = 5;
        [Range(0,10)]
        [SerializeField] private float airDrag = 5;
        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask landLayer;
        [Range(0, 0.1f)] 
        [SerializeField] private float checkDistance = 0.01f;
        
        
        private const int SpeedMultiplier = 5;
        private Rigidbody2D _rigidbody;
        private bool _wasMoving;
        private bool _hasJumped;
        private bool _isInAir;
        private float _jumpTimer;
        
        private float _verticalVelocity;
        private float _horizontalVelocity;
        
        public Vector2 Velocity => _rigidbody.velocity;
        public bool IsInAir => _isInAir;
        public float BodyDrag => _rigidbody.drag;
        private enum FacingDirectionEnum
        {
            None,
            Left,
            Right,
        }
        
        private FacingDirectionEnum _facingDirection;

        /// <summary>
        /// True: Right, False: Left
        /// </summary>
        public UnityAction<bool> OnFacingChanged;
        
        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            _facingDirection = FacingDirectionEnum.Right;
        }

        private void Update()
        {
            if (_hasJumped)
            {
                _jumpTimer -= Time.deltaTime;
                if (_jumpTimer <= 0)
                {
                    Debug.Log("Jump Timer Finished");
                    
                    if (Mathf.Abs(_verticalVelocity) < 0.1f)
                    {
                        _hasJumped = false;
                        _rigidbody.drag = airDrag;
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            var verVelocity = _rigidbody.velocity.y;
            var isFalling = _rigidbody.velocity.y < 0;
            
            if (Mathf.Abs(verVelocity) > jumpForce)
            {
                if (isFalling)
                {
                    verVelocity = -jumpForce;
                }
                else
                {
                    verVelocity = jumpForce;
                }
                
                Debug.Log("Vertical Speed Clamped");
            }

            _rigidbody.velocity = new Vector2(_horizontalVelocity, verVelocity);
        }

        public void MoveRight(float direction)
        {
            var currentFacing = FacingDirectionEnum.None;

            if (_isInAir == false)
            {
                if (Math.Abs(direction) < 0.1f)
                {
                    _rigidbody.drag = groundMaxDrag;
                    _wasMoving = false;
                }
                else if(_wasMoving == false)
                {
                    _rigidbody.drag = groundMinDrag;
                    _wasMoving = true;
                }
            }

            if (direction > 0.1)
            {
                currentFacing = FacingDirectionEnum.Right;
            }
            else if (direction < -0.1)
            {
                currentFacing = FacingDirectionEnum.Left;
            }

            if (currentFacing != FacingDirectionEnum.None)
            {
                if (currentFacing != _facingDirection)
                {
                    OnFacingChanged?.Invoke(currentFacing == FacingDirectionEnum.Right);
                }
                
                _facingDirection = currentFacing;
            }
            

            //Setup Acceleration
            var accelerationMultiplier = _isInAir ? hAccelerationMultiplierInAir : 1;
            var fixedSpeed = (hAcceleration * SpeedMultiplier) * accelerationMultiplier;
            var fixedDirection = (direction * fixedSpeed) * Time.fixedDeltaTime;
            var newDirection = (_rigidbody.velocity.x + fixedDirection);
            
            //Clamping to Max Speed
            if (Math.Abs(newDirection) > maxHSpeed)
            {
                newDirection = maxHSpeed * direction;
            }
            
            _horizontalVelocity = newDirection;
        }

        public void DoJump()
        {
            if (_isInAir == false)
            {
                _hasJumped = true;
                _isInAir = true;
                _jumpTimer = checkForMaxHeightDelay;
                _rigidbody.drag = groundMaxDrag;
                
                _rigidbody.AddForce(new Vector2(_rigidbody.velocity.x, jumpForce), ForceMode2D.Impulse);
            }
        }
        
        private void CheckForLanding()
        {
             var rayHit = Physics2D.Raycast(groundCheck.position, Vector2.down, checkDistance, landLayer);
             if (rayHit)
             {
                 _isInAir = false;
                 _rigidbody.drag = groundMinDrag;
             }
             else
             {
                 _isInAir = true;
                 _rigidbody.drag = airDrag;
             }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_isInAir && _hasJumped == false)
            {
                CheckForLanding(); 
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (_isInAir && _hasJumped == false)
            {
                CheckForLanding(); 
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (_hasJumped == false)
            {
                CheckForLanding();
            }
        }
    }
}