using DialogueScreen;
using Persistence;
using TMPro;
using UI;
using UnityEngine;

namespace MainScreen
{
    public class SaveEntry : MonoBehaviour
    {
        [SerializeField] private StateSaver _saver;
        [SerializeField] private TextMeshProUGUI _number;
        [SerializeField] private TextMeshProUGUI _dateAndLocation;
        [Space]
        [SerializeField] private int _order;

        public void Init(SaveData data, int order, StateSaver saver = null)
        {
            _number.text = order > 0 ? $"Save № {order}" : "AutoSave";
            _dateAndLocation.text = $"{data.Time.ToShortDateString()} {data.Time.ToShortTimeString()} - {data.Position}";
            _order = order;
            _saver = saver;
        }

        public void Init(string description, int order, StateSaver saver)
        {
            _number.text = order > 0 ? $"Save № {order}" : "AutoSave";
            _order = order;
            _dateAndLocation.text = description;
            _saver = saver;
        }

        public void Save()
        {
            _saver.SaveGame(_order);
            UIRouter.HideUrl($"{UIDirectory.DialogueRoot}/{UIDirectory.DialogueSave}");
        }

        public void Load()
        {
            UIRouter.OpenUrl($"{UIDirectory.DialogueRoot}?{UIDirectory.DialogueSaveParam}={_order}");
        }
    }
}
