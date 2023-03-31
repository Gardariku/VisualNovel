using System;
using Persistence;
using UnityEngine;

namespace Launch
{
    public class GameLauncher : MonoBehaviour
    {
        private void Awake()
        {
            SaveLoad.LoadSaves();
        }
    }
}
