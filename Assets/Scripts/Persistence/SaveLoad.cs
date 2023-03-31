using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Persistence
{
    public static class SaveLoad
    {
        public static List<SaveData> Saves { get; private set; } = new();

        //TODO: Look at new save addition to the existing static list
        public static void SaveGame(string data, List<SaveData.CharacterData> characters, int ind = -1)
        {
            SaveData saveData = new SaveData(data, characters);

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using FileStream fileStream = ind < 0
                ? new FileStream(Application.persistentDataPath + $"/Save{Saves.Count}.dic", FileMode.OpenOrCreate)
                : ind > 0 ? new FileStream(Application.persistentDataPath + $"/Save{ind}.dic", FileMode.OpenOrCreate)
                    : new FileStream(Application.persistentDataPath + "/AutoSave.dic", FileMode.OpenOrCreate);

            if (ind >= Saves.Count)
                Saves.Add(saveData);
            binaryFormatter.Serialize(fileStream, saveData);
            fileStream.Close();
        }

        public static void LoadSaves()
        {
            foreach (String path in Directory.GetFiles(Application.persistentDataPath, "*.dic"))
            {
                using FileStream fs = new FileStream(path, FileMode.Open);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                SaveData saveData = binaryFormatter.Deserialize(fs) as SaveData;
                Saves.Add(saveData);
            }
        }

        public static SaveData LoadGame(int index)
        {
            return Saves[index];
        }
    }
}
