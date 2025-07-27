using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TokenEvent
{
    NPCDeath,
    ToiletBrokenTimeout,
    ExtremeDirt,
    MafiaUnpaid,
    BonusObjective,
    DebugGive
}

public class LifeTokenManager : MonoBehaviour
{
    public static LifeTokenManager Instance { get; private set; }

    [Header("Day Config")]
    [SerializeField] int dayStartTokens = 5;   // Gün 1
    [SerializeField] int minTokensLastDay = 3; // Gün 7
    [SerializeField] int currentDay = 1;       // DayManager set eder
    [SerializeField] int maxDay = 7;

    [Header("Runtime")]
    [SerializeField] int tokensLeft;

    // UI / GameOver abonelik
    public event Action<int,int,TokenEvent> OnTokensChanged; // kalan, kaybedilen, sebep
    public event Action OnOutOfTokens;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    #region Public API
    public void StartDay(int dayIndex)
    {
        currentDay = dayIndex;
        tokensLeft = Mathf.Max(dayStartTokens - (dayIndex - 1), minTokensLastDay);
        Debug.Log($"[LifeToken] Gün {dayIndex} başladı → Jeton: {tokensLeft}");
        OnTokensChanged?.Invoke(tokensLeft, 0, TokenEvent.DebugGive);
    }

    public void LoseTokens(int amount = 1, TokenEvent reason = TokenEvent.DebugGive)
    {
        if (amount <= 0) return;
        tokensLeft -= amount;
        Debug.Log($"[LifeToken] –{amount} ({reason}) → {tokensLeft} kaldı");
        OnTokensChanged?.Invoke(tokensLeft, -amount, reason);

        if (tokensLeft <= 0)
        {
            OnOutOfTokens?.Invoke();
            HandleGameOver();
        }
    }

    public void AddTokens(int amount = 1, TokenEvent reason = TokenEvent.BonusObjective)
    {
        if (amount <= 0) return;
        tokensLeft += amount;
        Debug.Log($"[LifeToken] +{amount} ({reason}) → {tokensLeft}");
        OnTokensChanged?.Invoke(tokensLeft, amount, reason);
    }

    public int GetTokensLeft() => tokensLeft;
    #endregion

    void HandleGameOver()
    {
        Debug.LogWarning("[LifeToken] GAME OVER – Jeton bitti");
        // Basit çözüm → aynı günü yeniden yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // Daha şık akış için: DayManager.ShowGameOverPanel();
    }
}
