using System.Collections.Generic;
using Persistence;
using UI;
using UnityEngine;

namespace MainScreen
{
    public class LoadWindow : UIWindow
    {
        [SerializeField] private SaveEntry _savePrefab;
        [SerializeField] private Transform _savesRoot;
        [SerializeField] private List<SaveEntry> _saveEntries = new ();

        private LoadWindow() => URL = $"{UIDirectory.MainRoot}/{UIDirectory.MainLoading}";

        public override void Open(Dictionary<string, string> parameters)
        {
            var saves = SaveLoad.Saves;

            for (int i = 0; i < _saveEntries.Count; i++)
            {
                _saveEntries[i].Init(saves[i], i);
            }
            for (int i = _saveEntries.Count; i < saves.Count; i++)
            {
                _saveEntries.Add(Instantiate(_savePrefab, _savesRoot));
                _saveEntries[i].Init(saves[i], i);
            }
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
