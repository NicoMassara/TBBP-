using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Bubble
{
    public class BubbleMovement : MonoBehaviour
    {
        [Header("Speed Values")] 
        [Range(1, 10)] 
        [SerializeField] private float verticalSpeed = 1;
        [Range(1, 50)] 
        [SerializeField] private float horizontalSpeed = 10;
        [Space(10)]
        [Header("Errant Vertical Movement")] 
        [Range(0,10)]
        [SerializeField] private float maxErrantMovement = 1;
        [Range(0,10)]
        [SerializeField] private float errantSpeed = 1;
        [Range(0,10)]
        [SerializeField] private float errantMovementVariation = 1;
        [Header("Distance Values")]
        [Space] 
        [Range(0, 10)]
        [SerializeField] private float distanceToChangeDirection = 5;
        [Range(0,2)]
        [SerializeField] private float distanceNeededVariation = 1;
        
        
        private BubbleMovementTypeEnum _bubbleMovementType;
        private Vector2 _direction;
        private Vector2 _originPosition;
        private bool _hasChangedDirection = false;
        private float _distanceNeededToChangeDirection;
        private float _errantMovement;
        
        public UnityAction OnDirectionChange;
        
        private enum BubbleMovementTypeEnum
        {
            None,
            Right,
            Left,
            Vertical
        }

        private void Update()
        {
            if(_bubbleMovementType == BubbleMovementTypeEnum.None) return;
            
            var position = transform.position;
            var finalSpeed = _direction * Time.deltaTime;

            if (_hasChangedDirection)
            {
                float min = -_errantMovement;
                float max = _errantMovement;
                float midpoint = (min + max) / 2f;
                float amplitude = (max - min) / 2f;
                float oscillatingValue = midpoint + 
                                         Mathf.Sin(Time.time * errantSpeed) * amplitude;
                position.x += oscillatingValue;
            }

            transform.position = position + (Vector3)finalSpeed;


            if (CalculateCurrentDistanceFromOrigin() >= _distanceNeededToChangeDirection 
                && !_hasChangedDirection)
            {
                ChangeDirection();
            }
        }

        private float CalculateCurrentDistanceFromOrigin()
        {
            var position = transform.position;
            var distance = Vector2.Distance(position, _originPosition);
            
            return distance;
        }

        public void StartMovement(bool isFacingRight)
        {
            SetMovementType(isFacingRight ? 
                BubbleMovementTypeEnum.Right : BubbleMovementTypeEnum.Left);
            _originPosition = transform.position;
            _hasChangedDirection = false;
            _distanceNeededToChangeDirection = distanceToChangeDirection + 
                                               Random.Range(-distanceNeededVariation, distanceNeededVariation);
            
            // Set errant movement value
            
            var variation  = Random.Range(-errantMovementVariation, errantMovementVariation)/2;

            _errantMovement = (maxErrantMovement + variation) / 2000;
            
            // Flip Direction
            if (Random.Range(0, 1) >= 0.5f)
            {
                _errantMovement *= -1;
            }
        }

        public void EndMovement()
        {
            SetMovementType(BubbleMovementTypeEnum.None);
        }

        public void ChangeDirection()
        {
            SetMovementType(BubbleMovementTypeEnum.Vertical);
            OnDirectionChange.Invoke();
            _hasChangedDirection = true;
        }

        private void SetMovementType(BubbleMovementTypeEnum bubbleMovementType)
        {
            _bubbleMovementType = bubbleMovementType;
            
            switch (_bubbleMovementType)
            {
                case BubbleMovementTypeEnum.Right:
                    _direction = Vector2.right * horizontalSpeed;
                    break;
                case BubbleMovementTypeEnum.Left:
                    _direction = Vector2.left * horizontalSpeed;
                    break;
                case BubbleMovementTypeEnum.Vertical:
                    _direction = Vector2.up * (verticalSpeed/5);
                    break;
                default:
                    _direction = Vector2.zero;
                    break;
            }
        }
    }
}