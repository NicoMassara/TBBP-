using System;
using _Main.Custom.Pool;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Bubble
{
    [RequireComponent(typeof(BubbleMovement))]
    [RequireComponent(typeof(BubbleView))]
    public class BubbleModel : MonoBehaviour, IPoolable<BubbleModel>
    {
        [Range(1, 30)]
        [SerializeField] private float lifeTime = 3f;
        private BubbleMovement _bubbleMovement;
        private BubbleView _bubbleView;
        private float _lifeTimeTimer = -1f;
        private float _verticalTimer;
        
        public event UnityAction<BubbleModel> OnRecycle;

        private void Awake()
        {
            _bubbleMovement = GetComponent<BubbleMovement>();
            _bubbleView = GetComponent<BubbleView>();
        }

        private void Start()
        {
            _bubbleMovement.OnDirectionChange += Movement_OnDirectionChangeHandler;
        }

        private void Update()
        {
            if (_lifeTimeTimer > 0)
            {
                _lifeTimeTimer -= Time.deltaTime;
                if (_lifeTimeTimer <= 0)
                {
                    TriggerRecycle();
                }
            }
        }

        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }

        public void StartMovement(bool isFacingRight)
        {
            _bubbleMovement.StartMovement(isFacingRight);
        }

        public void Enable()
        {
            gameObject.SetActive(true);
            _bubbleView.Shrink();
        }

        public void Disable()
        {
            _bubbleMovement.EndMovement();
            gameObject.SetActive(false);
        }

        public void TriggerRecycle()
        {
            OnRecycle?.Invoke(this);
        }
        
        private void Movement_OnDirectionChangeHandler()
        {
            _lifeTimeTimer = lifeTime;
            _bubbleView.Expanded();
        }
    }
}