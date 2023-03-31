using System.Linq;
using DialogueScreen.Visuals;
using Persistence;
using UnityEngine;

namespace DialogueScreen
{
    public class StateSaver : MonoBehaviour
    {
        [SerializeField] private DialogueSystem _controller;
        [SerializeField] private Visuals.DialogueScreen _dialogueScreen;

        public void SaveGame(int ind)
        {
            SaveLoad.SaveGame(_controller.CurrentStory.state.ToJson(),
                _dialogueScreen.ActiveCharacters.Where(character => character.Value.gameObject.activeSelf)
                    .Select(character => new SaveData.CharacterData(
                        character.Value.ID, character.Value.GetPosition())).ToList(),
                ind);
        }
    }
}
