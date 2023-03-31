using System;
using System.Collections.Generic;
using MainScreen;
using Persistence;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueScreen.UI
{
    public class SaveWindow : UIWindow
    {
        [SerializeField] private StateSaver _saver;
        [SerializeField] private SaveEntry _savePrefab;
        [SerializeField] private Transform _savesRoot;
        [SerializeField] private List<SaveEntry> _saveEntries = new ();

        private const string NewEntryDescriptionString = "<b>New save</b>";

        private SaveWindow() => URL = $"{UIDirectory.DialogueRoot}/{UIDirectory.DialogueSave}";

        public override void Open(Dictionary<string, string> parameters)
        {
            gameObject.SetActive(true);

            var saves = SaveLoad.Saves;
            int min = Math.Min(_saveEntries.Count, saves.Count);
            for (int i = 0; i < min; i++)
            {
                _saveEntries[i].Init(saves[i], i, _saver);
            }
            for (int i = min; i < saves.Count; i++)
            {
                _saveEntries.Add(Instantiate(_savePrefab, _savesRoot));
                _saveEntries[i].Init(saves[i], i, _saver);
            }
            _saveEntries[0].GetComponent<Button>().interactable = false;
            if (_saveEntries.Count <= saves.Count)
                _saveEntries.Add(Instantiate(_savePrefab, _savesRoot));
            _saveEntries[^1].Init(NewEntryDescriptionString, _saveEntries.Count - 1, _saver);
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
