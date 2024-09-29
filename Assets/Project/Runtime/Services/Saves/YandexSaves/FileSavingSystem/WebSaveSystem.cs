using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace Project.Runtime.Services.Saves.YandexSaves.FileSavingSystem
{
    public class WebSaveSystem : MonoBehaviour
    {
        private const string ConfigSaveFileName = "saveData.txt";
        private const string ConfigSaveFileNameEditor = "saveData.txt"; //.json
        private const string ConfigWebDirectoryName = "Qiyas.Survive";

        [DllImport("__Internal")]
        private static extern void SyncFiles();

        [DllImport("__Internal")]
        private static extern void WindowAlert(string message);

        private static bool _initialized = false;

        private static bool IsWebPlayer => Application.platform == RuntimePlatform.WebGLPlayer;

        private static string _webFullSavePath = "";
        private static string _webFolderPath = "";
        private static string _standAloneFullSavePath = "";

        private static PlayerProgressData _playerProgressData;

        public static PlayerProgressData PlayerProgressData
        {
            get
            {
                if (!_initialized) Initialize();
                return _playerProgressData;
            }
            set => _playerProgressData = value;
        }


        //TODO: The profile is being loaded twice on "LoadProfile" initial call. Once on Initialize(), once on LoadProfile().
        public static void Initialize()
        {
            if (_initialized)
            {
                Debug.Log("[SaveSystem] Save system already initialized.");
                return;
            }

            PlatformSafeMessage("[SaveSystem] Initializing.");
            _initialized = true;

            //PlatformSafeMessage("[SaveSystem] Is web player: " + IsWebPlayer);
            if (IsWebPlayer)
            {
                //web_fullSavePath = Application.persistentDataPath + Path.DirectorySeparatorChar + config_SaveFileName;
                _webFolderPath = Path.DirectorySeparatorChar + "idbfs" + Path.DirectorySeparatorChar +
                                 ConfigWebDirectoryName + Path.DirectorySeparatorChar;
                _webFullSavePath = _webFolderPath + ConfigSaveFileName;

#if UNITY_EDITOR
                _webFullSavePath = _webFolderPath + ConfigSaveFileNameEditor;
#endif

                if (!Directory.Exists(_webFolderPath))
                {
                    PlatformSafeMessage("[SaveSystem] Creating idbfs directory.");
                    Directory.CreateDirectory(_webFolderPath);
                }

                LoadProfile();
            }
            else
            {
                _standAloneFullSavePath = Application.persistentDataPath + Path.DirectorySeparatorChar + ConfigSaveFileName;

#if UNITY_EDITOR
                _standAloneFullSavePath = Application.persistentDataPath + Path.DirectorySeparatorChar +
                                          ConfigSaveFileNameEditor;
#endif

                if (!File.Exists(_standAloneFullSavePath))
                {
                    PlatformSafeMessage("[SaveSystem] File does not exists. Creating.");
                    PlayerProgressData = new PlayerProgressData();
                    SaveProfile();
                }
                else
                {
                    LoadProfile();
                }
            }
        }

        private static void LoadWebProfile()
        {
            if (!_initialized)
                Initialize();

            try
            {
                PlatformSafeMessage("[SaveSystem] Starting file loading.");
                if (!File.Exists(_webFullSavePath))
                {
                    PlatformSafeMessage("[SaveSystem] File does not exists. Creating.");
                    PlayerProgressData = new PlayerProgressData();
                    SaveProfile();
                }
                else
                {
                    PlatformSafeMessage("[SaveSystem] Loading file.");
                    PlayerProgressData = JsonUtility.FromJson<PlayerProgressData>(File.ReadAllText(_webFullSavePath));
                }
            }
            catch (Exception e)
            {
                PlatformSafeMessage("[SaveSystem] Failed to Load Profile: " + e.Message);
            }
        }

        private static void LoadStandAloneProfile()
        {
            if (!_initialized)
                Initialize();

            try
            {
                //PlatformSafeMessage("[SaveSystem] Loading file.");
                PlayerProgressData = JsonUtility.FromJson<PlayerProgressData>(File.ReadAllText(_standAloneFullSavePath));
            }
            catch (Exception e)
            {
                PlatformSafeMessage("[SaveSystem] Failed to Load Profile: " + e.Message);
            }

            /*
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = File.Open(dataPath, FileMode.Open);

        gameDetails = (PlayerInfo)binaryFormatter.Deserialize(fileStream);
        fileStream.Close();
        saveData = gameDetails;
        */
        }

        private static void SaveWebProfile(bool wipeProfile = false)
        {
            try
            {
                PlatformSafeMessage("[SaveSystem] Saving profile.");

                File.WriteAllText(_webFullSavePath, JsonUtility.ToJson(PlayerProgressData, true));
                if (IsWebPlayer)
                {
                    SyncFiles();
                    //Debug.LogWarning("[SaveSystem] Saving disabled!");
                }
            }
            catch (Exception e)
            {
                PlatformSafeMessage("[SaveSystem] Failed to Save Profile: " + e.Message);
            }
        }

        private static void SaveStandAloneProfile(bool wipeProfile = false)
        {
            PlatformSafeMessage("[SaveSystem] Saving profile.");
            //PlatformSafeMessage("[SaveSystem] Full save path: " + standAlone_fullSavePath);
            File.WriteAllText(_standAloneFullSavePath, JsonUtility.ToJson(PlayerProgressData, true));
        }

        private static void LoadProfile()
        {
            if (IsWebPlayer)
            {
                LoadWebProfile();
            }
            else
            {
                LoadStandAloneProfile();
            }
        }

        public static void SaveProfile(bool wipeProfile = false)
        {
            if (IsWebPlayer)
            {
                SaveWebProfile(wipeProfile);
            }
            else
            {
                SaveStandAloneProfile(wipeProfile);
            }
        }

#if UNITY_EDITOR
        [MenuItem("GPG/Wipe Save Data")]
#endif
        public static void WipeSave()
        {
            Debug.LogWarning("[SS] Wiping save.");

            PlayerProgressData = new PlayerProgressData();
            SaveProfile(true);
            _initialized = false;
        }

#if UNITY_EDITOR
        [MenuItem("GPG/Print Save Data")]
#endif
        public static void PrintSave()
        {
            Debug.LogWarning(JsonUtility.ToJson(PlayerProgressData));
        }

#if UNITY_EDITOR
        [MenuItem("GPG/Delete Save Data")]
#endif
        public static void DeleteSave()
        {
            try
            {
                WipeSave();

                _standAloneFullSavePath = Application.persistentDataPath + Path.DirectorySeparatorChar + ConfigSaveFileName;

#if UNITY_EDITOR
                _standAloneFullSavePath = Application.persistentDataPath + Path.DirectorySeparatorChar +
                                          ConfigSaveFileNameEditor;
#endif

                File.Delete(_standAloneFullSavePath);
                Debug.LogWarning("[SaveSystem] Trying to delete save file.");
            }
            catch (Exception e)
            {
                Debug.LogWarning("[SaveSystem] Save file deletion fail: " + e.Message);
            }
            finally
            {
                Debug.LogWarning("[SaveSystem] Save file successfully deleted.");
            }
        }

#if UNITY_EDITOR
        [MenuItem("GPG/Save Profile")]
#endif
        public static void SaveProfileOnEditor()
        {
            if (IsWebPlayer)
            {
                SaveWebProfile(false);
            }
            else
            {
                SaveStandAloneProfile(false);
            }
        }


        private static void PlatformSafeMessage(string message)
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                //WindowAlert(message);
                Debug.LogWarning(message);
            }
            else
            {
                Debug.LogWarning(message);
            }
        }
    }
}