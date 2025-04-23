using System;
using UnityEngine;

namespace _Main.Scripts.Bubble
{
    public class BubbleView : MonoBehaviour
    {
        [Range(0.1f,1)]
        [SerializeField] private float shrinkScale;
        [SerializeField] private GameObject spriteObject;

        public void Shrink()
        {
            spriteObject.transform.localScale = shrinkScale * Vector3.one;
        }

        public void Expanded()
        {
            spriteObject.transform.localScale = Vector3.one;
        }
    }
}