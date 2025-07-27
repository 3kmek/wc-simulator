using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;                       // ← TextMeshPro kullandık

[DefaultExecutionOrder(-100)]
public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; }

    #region Inspector
    [Header("Config")]
    [SerializeField] int   startingDay    = 1;
    [SerializeField] int   maxDay         = 7;
    [SerializeField] float secondsPerDay  = 180f;     // 3 dk

    [Header("UI  (TMP)")]
    [SerializeField] TextMeshProUGUI dayText;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] GameObject      endDayPanel;
    [SerializeField] TextMeshProUGUI gradeText;
    #endregion

    #region Runtime
    public  int  CurrentDay  { get; private set; }
    float        dayTimer;
    bool         dayRunning;
    #endregion

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        BeginDay(startingDay);                         // Gün başlat
        LifeTokenManager.Instance.OnOutOfTokens += HandleOutOfTokens;
    }

    void Update()
    {
        if (!dayRunning) return;

        dayTimer -= Time.deltaTime;
        timerText.SetText($"{Mathf.Max(0, Mathf.CeilToInt(dayTimer))}");

        if (dayTimer <= 0f) EndDay();
    }

    #region Public API
    public void BeginDay(int dayIndex)
    {
        CurrentDay = Mathf.Clamp(dayIndex, 1, maxDay);
        dayTimer   = secondsPerDay;
        dayRunning = true;

        LifeTokenManager.Instance.StartDay(CurrentDay);

        dayText.SetText($"DAY {CurrentDay}");
        timerText.SetText($"{Mathf.CeilToInt(dayTimer)}");
        endDayPanel.SetActive(false);

        Debug.Log($"[DayManager] Gün {CurrentDay} başladı.");
    }
    #endregion

    #region Private
    void EndDay()
    {
        dayRunning = false;

        int grade = GradeCalculator.Instance.CalculateCurrentGrade();
        gradeText.SetText($"GRADE {ConvertToLetter(grade)}");
        endDayPanel.SetActive(true);

        Debug.Log($"[DayManager] Gün {CurrentDay} bitti → {grade}");

        if (grade >= 50 && CurrentDay < maxDay)
            Invoke(nameof(ProceedToNextDay), 3f);      // D ve üstü → sonraki gün
        else
            Invoke(nameof(RetryDay), 3f);              // Aksi hâlde tekrar dene
    }

    void HandleOutOfTokens()
    {
        Debug.LogWarning("[DayManager] Jeton bitti – günü yeniden başlatılıyor.");
        RetryDay();
    }

    void ProceedToNextDay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        BeginDay(CurrentDay + 1);
    }

    void RetryDay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        BeginDay(CurrentDay);
    }

    string ConvertToLetter(int grade)
    {
        if (grade >= 90) return "A";
        if (grade >= 75) return "B";
        if (grade >= 65) return "C";
        if (grade >= 50) return "D";
        return "F";
    }
    #endregion
}
