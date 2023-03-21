using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using static SaveData;
using System.Collections.Generic;
using System.Text;

public static class SaveSystem
{
    public static int CreateNewSave(string shipName)
    {
        List<SaveData> allSaves = LoadAllSaves();
        int saveIndex = allSaves.Count;

        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + $"/abyssalDepths{saveIndex}.sav";
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData saveData = new SaveData(shipName);

        formatter.Serialize(stream, saveData);
        stream.Close();

        //Left here on purpose
        Debug.Log("Saved! " + path);

        return saveIndex;
    }

    public static void Save(int saveIndex)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + $"/abyssalDepths{saveIndex}.sav";
        FileStream stream = new FileStream(path, FileMode.Create);

        ShipData shipData = Ship.Instance.ShipData;
        SaveData saveData = new SaveData(shipData);

        ScreenshotHandler.Instance.TakeScreenshot(1080, 1080, (Texture2D screenshotTexture) =>
        {
            byte[] screenshotByteArray = screenshotTexture.EncodeToPNG();

            saveData.ScreenShotBytes = screenshotByteArray;

            formatter.Serialize(stream, saveData);
            stream.Close();
            //Left here on purpose
            Debug.Log("Saved! " + path);
        });
    }

    public static SaveData Load(int saveIndex)
    {
        string path = Application.persistentDataPath + $"/abyssalDepths{saveIndex}.sav";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData saveData = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            return saveData;    
        }
        else
        {
            return null;
        }
    }

    public static List<SaveData> LoadAllSaves()
    {
        List<SaveData> allSaves = new List<SaveData>();

        for (int i = 0; i < 20; i++)
        {
            SaveData loadedData = Load(i);

            if(loadedData == null) { break; }

            allSaves.Add(loadedData);
        }

        return allSaves;
    }

    public static void SaveSettings(SettingsData settingsData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/abyssalDepths.options";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, settingsData);
        stream.Close();

        //Left here on purpose
        Debug.Log("Saved Settings! " + path);
    }

    public static SettingsData LoadSettings()
    {
        string path = Application.persistentDataPath + "/abyssalDepths.options";

        Debug.Log(Application.persistentDataPath);

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SettingsData saveData = formatter.Deserialize(stream) as SettingsData;
            stream.Close();

            return saveData;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
