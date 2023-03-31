using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MapScreen
{
    public class SiteInteraction : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private MapSite _site;
        [SerializeField] private SpriteRenderer _renderer;

        private static readonly Color InactiveColor = Color.gray;
        private static readonly Color ActiveColor = Color.white;

        private void Start()
        {
            _renderer.color = _site.Status ? ActiveColor : InactiveColor;

            _site.StatusChanged += OnStatusChanged;
        }

        private void OnStatusChanged(bool newStatus)
        {
            _renderer.color = newStatus ? ActiveColor : InactiveColor;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left && _site.Status)
            {
                UIRouter.HideCurrentScreen();
                UIRouter.OpenUrl($"{UIDirectory.DialogueRoot}?{UIDirectory.DialoguePathParam}={_site.Path}");
            }
        }
    }
}
