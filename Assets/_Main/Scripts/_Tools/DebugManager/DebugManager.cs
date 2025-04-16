using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace _Main.Scripts._Tools.DebugManager
{
    public class DebugManager : MonoBehaviour
    {
        public static DebugManager Instance;

        [SerializeField] private TMP_Text debugText;
        
        private readonly Dictionary<int, DebugData> _debugValues = new Dictionary<int, DebugData>();

        private bool _isActive = true;

        public class DebugData
        {
            public Func<string> GetValue { get; private set; }
            public string DataName { get; private set; }

            public DebugData(Func<string> getValue, string dataName)
            {
                GetValue = getValue;
                DataName = dataName;
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                _isActive = !_isActive;

                debugText.gameObject.SetActive(_isActive);
            }

            if (_isActive)
            {
                UpdateText();
            }
        }

        public void AddDebug(int key, string dataName,  Func<string> valueFunc)
        {
            var newData = new DebugData(valueFunc, dataName);
            _debugValues.Add(key, newData);
        }

        public void RemoveDebug(int key)
        {
            if (_debugValues.ContainsKey(key))
            {
                _debugValues.Remove(key);
            }
        }

        private void UpdateText()
        {
            debugText.text = "";
            foreach (var data in _debugValues.Values)
            {
                debugText.text += $"{data.DataName}: {data.GetValue()}\n";
            }
        }
    }
}