using System;
using System.Linq;
using Ink.Runtime;
using Persistence;
using UI;
using UnityEngine;

namespace DialogueScreen
{
    public class DialogueSystem : MonoBehaviour
    {
        [Header("Game Story JSON")]
        [SerializeField] private TextAsset _storyJSON;
        [Header("Load Globals JSON")]
        [SerializeField] private TextAsset _globalsJSON;

        public Story CurrentStory { get; private set; }
        private DialogueVariables _storyVariables;
        public bool CanContinue { get; set; }
        [field: SerializeField] public bool IsPlaying { get; private set; }
        [field: SerializeField] public bool IsSkipping { get; private set; }
        [SerializeField] private float _skipTime;
        private float _timePassed;

        public event Action<string> SpeakerChanged;
        public event Action<string> LineChanged;
        public event Action LineSkipped;

        public event Action<InkList> CharactersLoaded;
        public event Action<string, float> CharacterShowed;
        public event Action<string, float> CharacterMoved;
        public event Action<InkList> CharacterHidden;

        public event Action<string[], string[]> SitesOpened;
        public event Action<string[]> SitesClosed;

        private void Start()
        {
            IsPlaying = false;
        }

        // TODO: Maybe this control flow should belong to screen, or separate class?
        private void Update()
        {
            if (!IsPlaying)
                return;

            if (IsSkipping)
            {
                _timePassed += Time.deltaTime;
                if (_timePassed > _skipTime)
                {
                    _timePassed = 0f;
                    ContinueStory();
                }
            }
        }

        public void TryContinue()
        {
            if (!IsPlaying)
                return;

            if (IsSkipping)
            {
                SwitchSkip();
            }
            else
            {
                if (CanContinue && CurrentStory.currentChoices.Count == 0)
                    ContinueStory();
                else if (!CanContinue)
                    LineSkipped?.Invoke();
            }
        }

        public void SwitchSkip()
        {
            _timePassed = 0f;
            IsSkipping = !IsSkipping;
        }

        public void EnterDialogueMode(string saveID = null)
        {
            IsPlaying = true;
            CurrentStory = new Story(_storyJSON.text);
            CurrentStory.allowExternalFunctionFallbacks = true;
            _storyVariables = new DialogueVariables(_globalsJSON);

            SubscribeToVariables();
            BindFunctions();

            if (saveID != null)
            {
                var save = SaveLoad.LoadGame(int.Parse(saveID));
                CurrentStory.state.LoadJson(save.Data);
                CharactersLoaded?.Invoke((InkList) CurrentStory.variablesState["actors"]);

                foreach (var character in save.ActiveCharacters)
                    CharacterShowed?.Invoke(character.ID, character.Position);
                LineChanged?.Invoke(CurrentStory.currentText);
            }
            else
            {
                ContinueStory();
                ContinueStory();
            }
        }

        public void JumpToPath(string path)
        {
            CurrentStory.ChoosePathString(path);
            Resume();
        }

        public void Resume()
        {
            IsPlaying = true;
            ContinueStory();
        }

        private void BindFunctions()
        {
            CurrentStory.BindExternalFunction("Load", (InkList actors) => CharactersLoaded?.Invoke(actors));
            CurrentStory.BindExternalFunction("Show", (string id, float x) => CharacterShowed?.Invoke(id, x));
            CurrentStory.BindExternalFunction("Move", (string id, float x) => CharacterMoved?.Invoke(id, x));
            CurrentStory.BindExternalFunction("Hide", (InkList ids) => CharacterHidden?.Invoke(ids));

            // TODO: fix next line view handle coroutine start after map opening (wait before disabling dialogue screen or smth)
            CurrentStory.BindExternalFunction("OpenMap", (InkList sites, string paths) =>
            {
                IsPlaying = false;
                UIRouter.OpenUrl(UIDirectory.MapRoot);
                SitesOpened?.Invoke(sites.Select(x => x.Key.itemName).ToArray(), paths.Split(' '));
            });
        }

        private void SubscribeToVariables()
        {
            CurrentStory.ObserveVariable("speaker", (_, newValue) =>
                SpeakerChanged?.Invoke(((InkList)newValue).First().Key.itemName));

            CurrentStory.ObserveVariable("visitedSites", (_, newValue) =>
                SitesClosed?.Invoke(((InkList)newValue).Select(x => x.Key.itemName).ToArray()));
        }

        private void ExitDialogueMode()
        {
            IsPlaying = false;
            _storyVariables.StopListening(CurrentStory);
            UIRouter.HideCurrentScreen();
        }

        public void ContinueStory()
        {
            if (CurrentStory.canContinue)
                LineChanged?.Invoke(CurrentStory.Continue());
            else
                ExitDialogueMode();
        }

        public void SetVariableState(string variableName, Ink.Runtime.Object variableValue)
        {
            if (_storyVariables.variables.ContainsKey(variableName))
            {
                _storyVariables.variables.Remove(variableName);
                _storyVariables.variables.Add(variableName, variableValue);
            }
            else
                Debug.LogWarning("Tried to update variable that wasn't initialized by globals.ink: " + variableName);
        }

        public Ink.Runtime.Object GetVariableState(string variableName)
        {
            _storyVariables.variables.TryGetValue(variableName, out var variableValue);
            if (variableValue == null)
                Debug.LogWarning("Ink Variable was found to be null: " + variableName);

            return variableValue;
        }

        public void OnApplicationQuit()
        {
            _storyVariables.SaveVariables();
        }
    }
}
