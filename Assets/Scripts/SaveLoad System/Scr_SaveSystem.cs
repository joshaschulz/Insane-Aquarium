using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

public static class Scr_SaveSystem
{
    private const string ext = ".fishy";

    public static string[] GetAllBusinessSaves()
    {
        string dir = Application.persistentDataPath;
        if (!Directory.Exists(dir)) return new string[0];

        return Directory.GetFiles(dir, "*" + ext)
                        .Select(Path.GetFileNameWithoutExtension) // businessName
                        .OrderBy(n => n)
                        .ToArray();
    }
    public static void SaveBusiness(Scr_GameManager gameManager, string businessName)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/"  + gameManager.currentDifficulty + businessName + ext;

        Debug.Log("SAVED TO: " + path);
        FileStream stream = new FileStream(path, FileMode.Create);

        Scr_ProgressData progress = new Scr_ProgressData(gameManager);

        formatter.Serialize(stream, progress);
        stream.Close();
    }

    public static Scr_ProgressData LoadBusiness(string businessName, int difficulty)
    {
        string path = Application.persistentDataPath + "/" + difficulty + businessName + ext;
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
