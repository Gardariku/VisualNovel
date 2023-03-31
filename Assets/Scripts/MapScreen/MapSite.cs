using System;
using UnityEngine;

namespace MapScreen
{
    public class MapSite : MonoBehaviour
    {
        [SerializeField] private Map _map;
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }

        public event Action<bool> StatusChanged;

        [field: SerializeField] public string Path { get; set; }
        [SerializeField] private bool _status;
        public bool Status
        {
            get => _status;
            set
            {
                if (value == _status) return;
                _status = value;
                StatusChanged?.Invoke(value);
            }
        }
    }
}
