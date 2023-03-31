using UnityEngine;

namespace UI
{
    // GameObject with this component should be placed in any scene containing UI windows
    public class UIRegistrator : MonoBehaviour
    {
        private UIWindow[] _windows;
        private void Start()
        {
            _windows = FindObjectsOfType<UIWindow>(true);
            foreach (var window in _windows)
            {
                UIRouter.RegistrateWindow(window);
                window.gameObject.SetActive(false);
            }

            UIRouter.OpenUrl(UIDirectory.MainRoot);
        }

        private void OnDestroy()
        {
            foreach (var window in _windows)
            {
                UIRouter.DeleteWindow(window);
            }
        }

        public void OpenUrl(string url) => UIRouter.OpenUrl(url);
        public void HideUrl(string url) => UIRouter.HideUrl(url);
        public void SwitchUrl(string url) => UIRouter.SwitchUrl(url);
    }
}
