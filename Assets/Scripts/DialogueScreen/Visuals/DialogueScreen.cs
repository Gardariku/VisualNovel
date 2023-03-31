using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Ink.Runtime;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DialogueScreen.Visuals
{
    // TODO: Split this into Screen, Characters and Choices separate classes
    public class DialogueScreen : UIWindow
    {
        [SerializeField] private DialogueSystem _dialogueSystem;
        [SerializeField] private DialogueInputTrigger _input;
        [Header("Text parameters")]
        [SerializeField] private float _typingSpeed = 0.04f;

        [Header("Dialogue UI")]
        [SerializeField] private RectTransform _dialoguePanel;
        [SerializeField] private RectTransform _charactersPanel;
        [SerializeField] private GameObject _continueIcon;
        [SerializeField] private TextMeshProUGUI _dialogueText;
        [SerializeField] private TextMeshProUGUI _speakerNameText;

        [Header("Choices UI")]
        [SerializeField] private GameObject[] _choices;
        private TextMeshProUGUI[] _choicesText;

        private bool _isWriting;
        private bool _skip;
        private Coroutine _displayLineCoroutine;

        public Dictionary<string, Character> ActiveCharacters { get; } = new ();
        private List<Tween> _activeTweens = new (10);
        private List<Tween> _futureTweens = new (10);

        private DialogueScreen() => URL = "dialogue";

        private void OnEnable()
        {
            _choicesText = new TextMeshProUGUI[_choices.Length];
            int index = 0;
            foreach (GameObject choice in _choices)
            {
                _choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
                index++;
            }

            _dialogueSystem.LineChanged += OnLinePlayed;
            _dialogueSystem.LineSkipped += OnLineSkipped;
            _dialogueSystem.SpeakerChanged += OnSpeakerChanged;
            _dialogueSystem.CharactersLoaded += OnCharactersLoaded;
            _dialogueSystem.CharacterShowed += OnCharacterShowed;
            _dialogueSystem.CharacterMoved += OnCharacterMoved;
            _dialogueSystem.CharacterHidden += OnCharacterHidden;
        }

        private void OnDisable()
        {
            _dialogueSystem.LineChanged -= OnLinePlayed;
            _dialogueSystem.LineSkipped -= OnLineSkipped;
            _dialogueSystem.SpeakerChanged -= OnSpeakerChanged;
            _dialogueSystem.CharactersLoaded -= OnCharactersLoaded;
            _dialogueSystem.CharacterShowed -= OnCharacterShowed;
            _dialogueSystem.CharacterMoved -= OnCharacterMoved;
            _dialogueSystem.CharacterHidden -= OnCharacterHidden;
        }

        public override void Open(Dictionary<string, string> parameters)
        {
            UIRouter.HideAllWindows();
            UIRouter.SetMainScreenRoute(URL);
            _dialoguePanel.gameObject.SetActive(true);
            for (int i = 0; i < _choices.Length; i++)
                _choices[i].gameObject.SetActive(false);

            if (parameters.TryGetValue(UIDirectory.DialoguePathParam, out var path))
                _dialogueSystem.JumpToPath(path);
            else if (parameters.TryGetValue(UIDirectory.DialogueSaveParam, out var saveFile))
            {
                _dialogueSystem.EnterDialogueMode(saveFile);
                _speakerNameText.text = _dialogueSystem.CurrentStory.variablesState["speaker"].ToString();
            }
            else
            {
                _dialogueSystem.EnterDialogueMode();
            }
        }

        public override void Close()
        {
            gameObject.SetActive(false);
            //StartCoroutine(PlayCloseAnimation());
        }

        public IEnumerator PlayCloseAnimation()
        {
            yield return new WaitForSeconds(0.2f);

            _dialoguePanel.gameObject.SetActive(false);
            _dialogueText.text = "";
        }

        private void OnSpeakerChanged(string newName)
        {
            _speakerNameText.text = newName;
        }

        private void OnLinePlayed(string line)
        {
            if (!gameObject.activeSelf)
                return;

            FinishAnimations();
            StartCoroutine(DisplayLine(line));
        }

        private void OnLineSkipped()
        {
            if (_isWriting)
                _skip = true;
        }

        private IEnumerator DisplayLine(string line)
        {
            _isWriting = true;
            _skip = false;
            _dialogueText.text = line;
            _dialogueText.maxVisibleCharacters = 0;

            _continueIcon.SetActive(false);
            _dialogueSystem.CanContinue = false;
            bool isAddingRichTextTag = false;

            if (_dialogueSystem.CurrentStory.currentChoices.Count > 0 && _dialogueSystem.IsSkipping)
                _dialogueSystem.SwitchSkip();

            foreach (char letter in line)
            {
                if (_skip)
                {
                    _dialogueText.maxVisibleCharacters = line.Length;
                    break;
                }

                if (letter == '<' || isAddingRichTextTag)
                {
                    isAddingRichTextTag = true;
                    if (letter == '>')
                        isAddingRichTextTag = false;
                }
                else
                {
                    _dialogueText.maxVisibleCharacters++;
                    yield return new WaitForSeconds(_typingSpeed);
                }
            }

            _continueIcon.SetActive(true);
            if (_dialogueSystem.CurrentStory.currentChoices.Count > 0)
                DisplayChoices(_dialogueSystem.CurrentStory.currentChoices);
            _dialogueSystem.CanContinue = true;
        }

        private void FinishAnimations()
        {
            for (int i = 0; i < _activeTweens.Count; i++)
                if (!_activeTweens[i].IsComplete())
                    _activeTweens[i].Complete();

            _activeTweens = _futureTweens;
            _futureTweens = new List<Tween>();
        }

        #region Choices

        private void HideChoices()
        {
            foreach (GameObject choiceButton in _choices)
                choiceButton.SetActive(false);
        }

        private void DisplayChoices(List<Choice> currentChoices)
        {
            if (currentChoices.Count > _choices.Length)
                Debug.LogError("More choices were given than the UI can support. Number of choices given: "
                               + currentChoices.Count);

            int index = 0;
            foreach(Choice choice in currentChoices)
            {
                _choices[index].gameObject.SetActive(true);
                _choicesText[index].text = choice.text;
                index++;
            }

            StartCoroutine(SelectFirstChoice());
        }

        private IEnumerator SelectFirstChoice()
        {
            EventSystem.current.SetSelectedGameObject(null);
            yield return new WaitForEndOfFrame();
            EventSystem.current.SetSelectedGameObject(_choices[0].gameObject);
        }

        public void MakeChoice(int choiceIndex)
        {
            for (int i = 0; i < _choices.Length; i++)
                _choices[i].gameObject.SetActive(false);

            if (_dialogueSystem.CanContinue)
            {
                _dialogueSystem.CurrentStory.ChooseChoiceIndex(choiceIndex);
                _dialogueSystem.ContinueStory();
            }
        }

        #endregion

        #region Characters

        private void OnCharactersLoaded(InkList actors)
        {
            foreach (var actor in actors)
            {
                string charID = actor.Key.itemName;
                if (!ActiveCharacters.ContainsKey(charID))
                {
                    var newChar = Instantiate(Resources.Load<Character>($"Characters/{charID}"), _charactersPanel);
                    ActiveCharacters.Add(charID, newChar);
                    newChar.gameObject.SetActive(false);
                }
            }
            foreach (var character in ActiveCharacters)
            {
                if (!actors.ContainsItemNamed(character.Key))
                    ActiveCharacters.Remove(character.Key);
            }
        }

        private void OnCharacterShowed(string charID, float position)
        {
            if (!ActiveCharacters.TryGetValue(charID, out var character))
                Debug.LogWarning($"Character <{charID}> isn't loaded");
            _futureTweens.Add(character.Show(position));
        }

        private void OnCharacterMoved(string charID, float newPos)
        {
            if (!ActiveCharacters.TryGetValue(charID, out var character))
                Debug.LogWarning($"Character <{charID}> isn't loaded");
            _futureTweens.Add(character.Move(newPos));
        }

        private void OnCharacterHidden(InkList charIDs)
        {
            foreach (var charID in charIDs)
            {
                ActiveCharacters[charID.Key.itemName].gameObject.SetActive(false);
            }
        }

        #endregion
    }
}
