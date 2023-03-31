using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public abstract class UIWindow : MonoBehaviour
    {
        [field: SerializeField] public string URL { get; protected set; }

        public abstract void Open(Dictionary<string, string> parameters);
        public abstract void Close();
    }
}
