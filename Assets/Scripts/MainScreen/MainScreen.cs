using System.Collections.Generic;
using DialogueScreen;
using Persistence;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace MainScreen
{
    [UIRoute("main")]
    public class MainScreen : UIWindow
    {
        [SerializeField] private DialogueSystem _dialogue;
        [SerializeField] private StateSaver _saver;
        [SerializeField] private Button _continueButton;

        private const int AutoSaveID = 0;

        private MainScreen() => URL = "main";

        public override void Open(Dictionary<string, string> parameters)
        {
            UIRouter.HideAllWindows();
            UIRouter.SetMainScreenRoute(URL);
            if (SaveLoad.Saves.Count == 0)
                _continueButton.interactable = false;
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }

        public void StartNewGame()
        {
            UIRouter.OpenUrl(UIDirectory.DialogueRoot);
            _saver.SaveGame(0);
        }

        public void ContinueGame()
        {
            UIRouter.OpenUrl($"{UIDirectory.DialogueRoot}?save={AutoSaveID}");
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
