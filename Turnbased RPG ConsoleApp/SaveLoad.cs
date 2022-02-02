using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Turnbased_RPG_ConsoleApp
{
    public static class SaveLoad
    {
        public static string persistentDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Conranex Quest/";//Windows will correct "/" to "\" 
        public const int maxSaveSlots = 9;

        public static void InitializeDataPath()
        {
            var d = Directory.CreateDirectory(persistentDataPath);
            persistentDataPath = d.FullName;//Corrects slashes
        }

        public static bool SaveExists(int slotNum, string key, string extension = ".sav")
        {
            bool successful = false;
            if (extension != "")
                successful = File.Exists(persistentDataPath + "Saves\\" + $"Slot {slotNum}\\" + key + extension)
                    || File.Exists(persistentDataPath + "Saves/" + $"Slot {slotNum}/" + key + extension);
            else
                successful = Directory.Exists(persistentDataPath + "Saves\\" + $"Slot {slotNum}\\" + key)
                    || Directory.Exists(persistentDataPath + "Saves/" + $"Slot {slotNum}/" + key);

            return successful;
        }
        public static List<int> GetUsedSaveSlots()
        {
            List<int> output = new List<int>();
            for (int i = 1; i <= maxSaveSlots; i++)
            {
                if(Directory.Exists(persistentDataPath + "Saves/Slot " + i))
                {
                    output.Add(i);
                }
            }

            return output;
        }

        public static void Save<T>(T objToSave, int slotNum, string key, string extension = ".sav")
        {
            string path = persistentDataPath + "Saves/" + $"Slot {slotNum}/";
            Directory.CreateDirectory(path);
            string data = JsonConvert.SerializeObject(objToSave, Formatting.Indented);

            using (StreamWriter writer = new StreamWriter(path + key + extension))
            {
                writer.WriteLine(data);
            }
            Basic.print("Saved " + key + extension + " to " + path);
        }
        public static T Load<T>(int slotNum, string key, string extension = ".sav")
        {
            string path = persistentDataPath + "Saves/" + $"Slot {slotNum}/";
            string data = "";

            using (StreamReader reader = new StreamReader(path + key + extension))
            {
                data = reader.ReadToEnd();
            }

            Basic.print("Loaded " + key + extension + " from " + path);
            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}
