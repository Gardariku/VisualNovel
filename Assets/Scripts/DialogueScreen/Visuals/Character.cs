using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueScreen.Visuals
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _image;
        [field: SerializeField] public string ID { get; private set; }
        [field: SerializeField] public string Name { get; private set; }

        private const float MoveTime = 0.5f;
        private const float ShowTime = 0.25f;

        public Tween Show(float pos)
        {
            gameObject.SetActive(true);
            SetAnchor(pos);
            _rectTransform.anchoredPosition = Vector3.zero;

            var imageColor = _image.color;
            imageColor.a = 0f;
            _image.color = imageColor;
            return _image.DOFade(1, ShowTime);
        }

        private void SetAnchor(float pos)
        {
            var rectTransformAnchorMax = _rectTransform.anchorMax;
            rectTransformAnchorMax.x = pos;
            _rectTransform.anchorMax = rectTransformAnchorMax;

            var rectTransformAnchorMin = _rectTransform.anchorMin;
            rectTransformAnchorMin.x = pos;
            _rectTransform.anchorMin = rectTransformAnchorMin;
        }

        public Tween Move(float pos)
        {
            var sequence = DOTween.Sequence();
            sequence.Join(_rectTransform.DOAnchorMax(new(pos, 0.5f), MoveTime).SetEase(Ease.InOutQuad));
            sequence.Join(_rectTransform.DOAnchorMin(new(pos, 0.5f), MoveTime).SetEase(Ease.InOutQuad));
            return sequence;
        }

        public float GetPosition() => _rectTransform.anchorMax.x;
    }
}
