using UnityEngine;

namespace _Main.Scripts._Tools.DebugManager
{
    public static class DebugKeys
    {
        public static class Player
        {
            public static readonly int Velocity = Animator.StringToHash("Player.Velocity");
            public static readonly int InAir = Animator.StringToHash("Player.InAir");
            public static readonly int Drag = Animator.StringToHash("Player.Drag");
        }
    }
}