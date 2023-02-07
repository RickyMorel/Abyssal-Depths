using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
}
