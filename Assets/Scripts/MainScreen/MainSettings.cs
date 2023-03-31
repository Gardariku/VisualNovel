using System.Collections.Generic;
using UI;
using UnityEngine;

namespace MainScreen
{
    public class MainSettings : UIWindow
    {
        private MainSettings() => URL = "main/settings";

        public override void Open(Dictionary<string, string> parameters)
        {

        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
