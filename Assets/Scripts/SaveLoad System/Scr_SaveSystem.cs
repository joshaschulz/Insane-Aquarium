using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class Scr_SaveSystem
{
    public static void SaveBusiness(Scr_GameManager gameManager, string businessName)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + businessName + ".fishy";
        FileStream stream = new FileStream(path, FileMode.Create);

        Scr_ProgressData progress = new Scr_ProgressData(gameManager);

        formatter.Serialize(stream, progress);
        stream.Close();
    }

    public static Scr_ProgressData LoadBusiness(string businessName)
    {
        string path = Application.persistentDataPath + "/" + businessName + ".fishy";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Scr_ProgressData progress = formatter.Deserialize(stream) as Scr_ProgressData;
            stream.Close();

            return progress;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
