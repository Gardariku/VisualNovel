using System.Collections.Generic;
using UI;
using UnityEngine;

namespace MapScreen
{
    public class MapScreen : UIWindow
    {
        [SerializeField] private Map _mapState;

        private MapScreen() => URL = "map";

        public override void Open(Dictionary<string, string> parameters)
        {
            UIRouter.HideAllWindows();
            UIRouter.SetMainScreenRoute(URL);
            _mapState.gameObject.SetActive(true);
        }

        public override void Close()
        {
            gameObject.SetActive(false);
            _mapState.gameObject.SetActive(false);
        }
    }
}
