using UnityEngine;
using System.IO;

public static class SaveAndLoad {
    private static string optionsFileName = "options";

    public static void SaveOptions() {
        //Debug.Log("Saving options");
        string s = "";
        s += "volume=" + Options.volume;

        SaveFile(optionsFileName, s);
    }

    public static void LoadOptions() {
        //Debug.Log("Loading options");
        string path = Application.dataPath + optionsFileName + ".txt";
        StreamReader sr = new StreamReader(path);

        string line = "";
        while ((line = sr.ReadLine()) != null) {
            string[] words = line.Split('=');
            switch (words[0]) {
                case "volume":
                    Options.volume = int.Parse(words[1]);
                    break;
            }
        }
    }

    static void SaveFile(string fileName, string infoToSave) {
        //Debug.Log("Saving file " + fileName);
        string path = Application.dataPath + fileName + ".txt";
        StreamWriter sw = new StreamWriter(path, false);
        sw.WriteLine(infoToSave);
        sw.Close();
    }
}