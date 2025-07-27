using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class JetonHUD : MonoBehaviour
{
    [SerializeField] Image tokenPrefab;
    Image[] tokenImages;

    void Start()
    {
        LifeTokenManager.Instance.OnTokensChanged += RefreshHUD;
        int maxTokens = LifeTokenManager.Instance.GetTokensLeft();    // Gün başındaki sayı
        tokenImages = new Image[maxTokens];

        for(int i = 0; i < maxTokens; i++)
            tokenImages[i] = Instantiate(tokenPrefab, transform);
    }

    void RefreshHUD(int left, int delta, TokenEvent reason)
    {
        for(int i = 0; i < tokenImages.Length; i++)
            tokenImages[i].enabled = i < left;

        if(delta < 0) PunchMissingToken(left); // Kayıp feedback
    }

    void PunchMissingToken(int index)
    {
        if(index < 0 || index >= tokenImages.Length) return;
        tokenImages[index].transform
              .DOPunchScale(Vector3.one * 0.5f, .4f, 8)
              .SetEase(Ease.OutBounce);
    }
}
