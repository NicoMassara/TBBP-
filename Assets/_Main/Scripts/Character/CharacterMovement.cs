using System;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Character
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterMovement : MonoBehaviour
    {
        [Header("Horizontal Movement")]
        [Range(1, 10)]
        [SerializeField] private float hAcceleration = 5;
        [Range(1, 10)]
        [SerializeField] private float maxHSpeed = 5;
        [Range(1, 5)]
        [SerializeField] private float maxDrag = 5;
        [Range(0, 2)]
        [SerializeField] private float minDrag = 5;
        [Header("Vertical Movement")]
        [Range(1, 30)]
        [SerializeField] private float jumpForce = 10;
        [SerializeField] private LayerMask landLayer;
        [Range(0, 0.1f)] 
        [SerializeField] private float checkDistance = 0.01f;
        [SerializeField] private Transform groundCheck;
        
        private const int SpeedMultiplier = 5;
        private Rigidbody2D _rigidbody;
        private bool _wasMoving;
        private bool _hasJumped;
        private bool _isInAir;
        private float _jumpTimer;
        
        private float _verticalVelocity;
        private float _horizontalVelocity;
        
        public Vector2 Velocity => _rigidbody.velocity;

        
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
                    _hasJumped = false;
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
                    _rigidbody.drag = maxDrag;
                    _wasMoving = false;
                }
                else if(_wasMoving == false)
                {
                    _rigidbody.drag = minDrag;
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
            var fixedSpeed = (hAcceleration * SpeedMultiplier);
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
                _rigidbody.AddForce(new Vector2(_rigidbody.velocity.x, jumpForce), ForceMode2D.Impulse);
            
                _hasJumped = true;
                _jumpTimer = 0.5f;
                SetAirValues();
            }
        }

        private void SetAirValues()
        {
            _isInAir = true;
            _rigidbody.drag = 4;
        }

        private void CheckForLanding()
        {
             var rayHit = Physics2D.Raycast(groundCheck.position, Vector2.down, checkDistance, landLayer);
             if (rayHit)
             {
                 _isInAir = false;
                 _rigidbody.drag = minDrag;
                 Debug.Log("Landed");
             }
             else
             {
                 SetAirValues();
             }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_isInAir)
            {
                CheckForLanding(); 
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (_isInAir)
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