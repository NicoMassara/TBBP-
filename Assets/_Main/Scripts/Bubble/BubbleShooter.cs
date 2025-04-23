using System;
using _Main.Custom.Pool;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Bubble
{
    public class BubbleShooter : MonoBehaviour
    {
        [SerializeField] private BubbleModel bubblePrefab;
        [SerializeField] private Transform shootPoint;
        [Range(0, 5)] 
        [SerializeField] private float shootDelay = 1f;
        private GenericPool<BubbleModel> _bubblePool;
        private float _shootDelayTimer;

        public UnityAction OnShoot;

        private void Awake()
        {
            
        }

        private void Start()
        {
            _bubblePool = new GenericPool<BubbleModel>(
                () => Instantiate(bubblePrefab));
        }

        private void Update()
        {
            if (_shootDelayTimer > 0)
            {
                _shootDelayTimer -= Time.deltaTime;
            }
        }

        public void TryShoot(bool isFacingRight)
        {
            if (_shootDelayTimer > 0)
            {
                return;
            }

            var bubbleToUse = _bubblePool.GetPoolable();
            
            bubbleToUse.SetPosition(shootPoint.position);
            bubbleToUse.StartMovement(isFacingRight);
            _shootDelayTimer = shootDelay;
            OnShoot?.Invoke();
        }
    }
}