using System;
using System.Collections.Generic;
using DialogueScreen;

namespace Persistence
{
    [Serializable]
    public class SaveData
    {
        public string Data { get; }
        public string Position { get; }
        public DateTime Time { get; }
        public List<CharacterData> ActiveCharacters { get; }

        public SaveData(string data, List<CharacterData> characters)
        {
            Data = data;
            Time = DateTime.Now;
            Position = "Default Position";
            ActiveCharacters = characters;
        }

        [Serializable]
        public struct CharacterData
        {
            public string ID;
            public float Position;

            public CharacterData(string id, float position)
            {
                ID = id;
                Position = position;
            }
        }
    }
}
