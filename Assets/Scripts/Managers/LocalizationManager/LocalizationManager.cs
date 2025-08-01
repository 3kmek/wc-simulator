using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace DialogSystem
{
    /// <summary>
    /// Unity Localization ile dialog sistemini yöneten ana sınıf
    /// </summary>
    public class LocalizationManager : MonoBehaviour
    {
        [Header("Localization Settings")]
        [SerializeField] private string dialogTableReference = "TraitDialogTable";
        
        // Singleton pattern
        public static LocalizationManager Instance { get; private set; }
        
        // Cache için
        private StringTable currentStringTable;
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            //Debug.Log(LocalizationSettings.StringDatabase.GetTable(dialogTableReference));
            // Localization sistemi hazır olduğunda string table'ı cache'le
            if (LocalizationSettings.InitializationOperation.IsDone)
            {
                CacheStringTable();
            }
            else
            {
                LocalizationSettings.InitializationOperation.Completed += (op) => CacheStringTable();
            }
        }

        private void CacheStringTable()
        {
            var tableRequest = LocalizationSettings.StringDatabase.GetTableAsync(dialogTableReference);
            if (tableRequest.IsDone)
            {
                currentStringTable = tableRequest.Result;
            }
            else
            {
                tableRequest.Completed += (op) => currentStringTable = op.Result;
            }
        }

        /// <summary>
        /// Verilen key için localized metni döndürür
        /// </summary>
        public string GetLocalizedText(string key)
        {
            if (string.IsNullOrEmpty(key)) return "Missing Key";

            // Önce cache'den dene
            if (currentStringTable != null)
            {
                var entry = currentStringTable.GetEntry(key);
                if (entry != null)
                {
                    return entry.GetLocalizedString();
                }
            }

            // Cache'de yoksa async olarak al
            var localizedString = new LocalizedString(dialogTableReference, key);
            return localizedString.GetLocalizedString();
        }

        /// <summary>
        /// Async olarak localized text alır (callback ile)
        /// </summary>
        public void GetLocalizedTextAsync(string key, System.Action<string> onComplete)
        {
            if (string.IsNullOrEmpty(key))
            {
                onComplete?.Invoke("Missing Key");
                return;
            }

            var localizedString = new LocalizedString(dialogTableReference, key);
            var operation = localizedString.GetLocalizedStringAsync();
            
            if (operation.IsDone)
            {
                onComplete?.Invoke(operation.Result);
            }
            else
            {
                operation.Completed += (op) => onComplete?.Invoke(op.Result);
            }
        }

        /// <summary>
        /// Birden fazla key için localized text'leri alır
        /// </summary>
        public void GetMultipleLocalizedTexts(string[] keys, System.Action<Dictionary<string, string>> onComplete)
        {
            Dictionary<string, string> results = new Dictionary<string, string>();
            int completedCount = 0;

            foreach (string key in keys)
            {
                GetLocalizedTextAsync(key, (localizedText) =>
                {
                    results[key] = localizedText;
                    completedCount++;

                    if (completedCount >= keys.Length)
                    {
                        onComplete?.Invoke(results);
                    }
                });
            }
        }
    }
}

namespace DialogSystem
{
    /// <summary>
    /// Dialog seçeneklerini yöneten sınıf
    /// </summary>
    [System.Serializable]
    public class DialogSequence
    {
        [Header("Sequence Info")]
        public string sequenceName;
        public DialogType dialogType;
        
        [Header("Dialog Keys")]
        public string[] dialogKeys;
        
        [Header("Conditions")]
        public bool isFirstMeeting = false;
        public bool requiresSpecificTrait = false;
        public string requiredTraitName;
        
        private int currentIndex = 0;
        
        public string GetCurrentDialogKey()
        {
            if (dialogKeys == null || dialogKeys.Length == 0) return "";
            
            if (currentIndex >= dialogKeys.Length)
                currentIndex = 0; // Loop back to start
                
            return dialogKeys[currentIndex];
        }
        
        public string GetNextDialogKey()
        {
            currentIndex++;
            return GetCurrentDialogKey();
        }
        
        public void ResetSequence()
        {
            currentIndex = 0;
        }
        
        public bool HasMoreDialogs()
        {
            return dialogKeys != null && currentIndex < dialogKeys.Length - 1;
        }
    }
    
    public enum DialogType
    {
        FirstMeeting,
        SubsequentMeeting,
        TraitSpecific,
        Generic,
        Special
    }
}

