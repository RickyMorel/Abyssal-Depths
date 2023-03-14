using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using static SaveData;

public static class SaveSystem
{
    public static void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/abyssalDepths.sav";
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData saveData = new SaveData(Ship.Instance.GetComponent<ShipData>());

        formatter.Serialize(stream, saveData);
        stream.Close();

        //Left here on purpose
        Debug.Log("Saved! " + path);
    }

    public static SaveData Load()
    {
        string path = Application.persistentDataPath + "/abyssalDepths.sav";

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
            Debug.LogError("Save file not found in " + path);
            return null;
        }
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
