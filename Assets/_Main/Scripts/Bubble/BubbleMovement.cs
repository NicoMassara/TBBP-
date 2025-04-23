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
        [SerializeField] private float verticalSpeed = 5;
        [Range(1, 50)] 
        [SerializeField] private float horizontalSpeed = 15;
        [Space(10)]
        [Header("Errant Vertical Movement")] 
        [Range(0,5)]
        [SerializeField] private float maxErrantMovement = 1f;
        [Range(0,5)]
        [SerializeField] private float errantSpeed = 1.5f;
        [Range(0,2)]
        [SerializeField] private float errantMovementVariation = 1.25f;
        [Header("Distance Values")]
        [Space] 
        [Range(0, 10)]
        [SerializeField] private float distanceToChangeDirection = 5;
        [Range(0,2)]
        [SerializeField] private float distanceNeededVariation = 1.5f;
        
        
        private BubbleMovementTypeEnum _bubbleMovementType;
        private Vector2 _direction;
        private Vector2 _originPosition;
        private bool _hasChangedDirection = false;
        private float _distanceNeededToChangeDirection;
        private float _errantAngle;
        
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
                position.x += CalculateErraticMovement();
            }

            transform.position = position + (Vector3)finalSpeed;


            if (CalculateCurrentDistanceFromOrigin() >= _distanceNeededToChangeDirection 
                && !_hasChangedDirection)
            {
                ChangeDirection();
            }
        }

        private float CalculateErraticMovement()
        {
            _errantAngle += Time.deltaTime * errantSpeed * Mathf.PI * 2f; // Full sine wave cycle
            
            if (_errantAngle > Mathf.PI * 2f)
            {
                _errantAngle -= Mathf.PI * 2f; 
            }
            
            var variation  = Random.Range(-errantMovementVariation, errantMovementVariation);
            
            float min = (-maxErrantMovement + variation) / 2000;
            float max = (maxErrantMovement + variation) / 2000;
            float midpoint = (min + max) / 2f;
            float amplitude = (max - min) / 2f;
            float oscillatingValue = midpoint + Mathf.Sin(_errantAngle) * amplitude;
            
            return oscillatingValue;
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
            _errantAngle = 0;
            _distanceNeededToChangeDirection = distanceToChangeDirection + 
                                               Random.Range(-distanceNeededVariation, distanceNeededVariation);
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