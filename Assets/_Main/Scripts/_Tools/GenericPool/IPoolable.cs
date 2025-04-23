using System;
using UnityEngine.Events;

namespace _Main.Custom.Pool
{
    public interface IPoolable<T>
    {
        event UnityAction<T> OnRecycle;
        void Enable();
        void Disable();
    }
}