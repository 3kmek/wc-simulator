using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    [Header("Dil Dosyaları")]
    [SerializeField] private TextAsset englishFile;
    [SerializeField] private TextAsset turkishFile;

    private Dictionary<string, string> localizedTexts = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLanguage("en"); // Başlangıç dili
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadLanguage(string lang)
    {
        localizedTexts.Clear();
        string[] lines;

        if (lang == "en")
            lines = englishFile.text.Split('\n');
        else
            lines = turkishFile.text.Split('\n');

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || !line.Contains("=")) continue;

            string[] splitLine = line.Split('=');
            string key = splitLine[0].Trim();
            string value = splitLine[1].Trim();

            localizedTexts[key] = value;
        }
    }

    public string GetText(string key)
    {
        if (localizedTexts.ContainsKey(key))
            return localizedTexts[key];
        return $"[MISSING:{key}]";
    }
}