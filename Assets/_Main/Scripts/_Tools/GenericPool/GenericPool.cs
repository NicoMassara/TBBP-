using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Main.Custom.Pool
{
    public class GenericPool<T> where T : IPoolable<T>, new()
    {
        private List<T> _available = new List<T>();
        private List<T> _inUse = new List<T>();

        private readonly Func<T> _factoryMethod;
        private readonly bool _isDynamic;

        public int AvailableCount => _available.Count;
        public int InUseCount => _inUse.Count;

        public GenericPool(Func<T> factoryMethod, bool isDynamic = true, int initialStock = 10)
        {
            _factoryMethod = factoryMethod;
            _isDynamic = isDynamic;

            for (int i = 0; i < initialStock; i++)
            {
                var value = _factoryMethod();
                value.Disable();
                _available.Add(value);
            }
        }

        public T GetPoolable()
        {
            T poolable = default;

            if (AvailableCount > 0)
            {
                poolable = _available[0];
                _available.RemoveAt(0);
                _inUse.Add(poolable);
            }
            else if (_isDynamic)
            {
                poolable = _factoryMethod();
                _inUse.Add(poolable);
            }

            if (poolable != null)
            {
                poolable.OnRecycle += OnRecycleHandler;
                poolable.Enable();
            }

            return poolable;
        }

        private void Recycle(T poolable)
        {
            if (_inUse.Contains(poolable))
            {
                _inUse.Remove(poolable);
            }

            if (!_available.Contains(poolable))
            {
                _available.Add(poolable);
            }
        }

        public void Clear()
        {
            _available.Clear();
            _inUse.Clear();
        }
        
        private void OnRecycleHandler(T poolable)
        {
            poolable.OnRecycle -= OnRecycleHandler;
            Recycle(poolable);
            poolable.Disable();
        }
    }
}