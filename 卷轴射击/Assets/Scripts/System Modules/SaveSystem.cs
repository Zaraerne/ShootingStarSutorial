using UnityEngine;
using System.IO;

namespace SaveSystemTutorial
{
    public class SaveSystem : MonoBehaviour
    {
        #region PlayerPref
        public static void SaveByPlayerPrefs(string key, object data)
        {
            var json = JsonUtility.ToJson(data);

            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();

#if UNITY_EDITOR
            Debug.Log("Successfully saved data to PlayerPrefs");
#endif

        }

        public static string LoadFromPlayerPrefs(string key)
        {
            return PlayerPrefs.GetString(key);
        }
        #endregion

        #region JSON
        public static void SaveByJson(string saveFileName, object data)
        {
            var json = JsonUtility.ToJson(data);
            var path = Path.Combine(Application.persistentDataPath, saveFileName);
            try 
            {
                File.WriteAllText(path, json);
                #if UNITY_EDITOR
                Debug.Log($"Susscessfully saved data to {path}");
                #endif
            }
            catch (System.Exception excption)
            {
                #if UNITY_EDITOR
                Debug.Log($"Failed to save data to {path}. \n{excption}");
                #endif
            }
        }

        public static T LoadFromJson<T>(string saveFileName)
        {
            var path = Path.Combine(Application.persistentDataPath, saveFileName);
            try
            {
                var json = File.ReadAllText(path);

                var data = JsonUtility.FromJson<T>(json);

                return data;
            }
            catch (System.Exception excption)
            {
                #if UNITY_EDITOR
                Debug.Log($"Failed to load data from {path}. \n{excption}");
                #endif
                return default;
            }
        }

        public static void DeleteSaveFile(string saveFileName)
        {
            var path = Path.Combine(Application.persistentDataPath, saveFileName);

            try
            {
                File.Delete(path);
            }
            catch(System.Exception exception)
            {
#if UNITY_EDITOR
                Debug.Log($"Failed to delete {path}. \n{exception}");
#endif
            }
        }


        public static bool SaveFileExists(string saveFileName)
        {
            var path = Path.Combine(Application.persistentDataPath, saveFileName);
            return File.Exists(path);
        }
        #endregion

    }
}
