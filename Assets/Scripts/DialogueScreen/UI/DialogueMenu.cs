using System.Collections.Generic;
using Persistence;
using UI;
using UnityEditor;
using UnityEngine;

namespace DialogueScreen.UI
{
    public class DialogueMenu : UIWindow
    {
        [SerializeField] private StateSaver _saver;

        private DialogueMenu() => URL = $"{UIDirectory.DialogueRoot}/{UIDirectory.DialogueMenu}";

        public override void Open(Dictionary<string, string> parameters)
        {
            gameObject.SetActive(true);
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }

        public void OpenMainMenu()
        {
            _saver.SaveGame(0);
            UIRouter.OpenUrl(UIDirectory.MainRoot);
        }

        public void ExitGame()
        {
            _saver.SaveGame(0);
            Application.Quit();
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
        }
    }
}
