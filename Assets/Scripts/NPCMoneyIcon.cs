using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NPC;
using UnityEngine;


public class NPCMoneyIcon : MonoBehaviour
{
    Transform _playerTransform;
    NPCController _npcController;
    SpriteRenderer _spriteRenderer;
    
    void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _npcController = transform.parent.GetComponent<NPCController>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.enabled = false;
        
        transform.DOScale(Vector3.one * .1f, 0.5f) // 1.2x büyüt
            .SetLoops(-1, LoopType.Yoyo)  // Sonsuz tekrar et, ileri geri
            .SetEase(Ease.InOutSine);  // Yumuşak geçiş sağla
    }

    // Update is called once per frame
    void Update()
    {
        if (_npcController.currentState == NPCState.KeyGiver)
        {
            HandleScaleIcon();
        }
        else
        {
            _spriteRenderer.enabled = false;
        }
        
    }

    void HandleScaleIcon()
    {
        _spriteRenderer.enabled = true;
        
        Vector3 directionToPlayer = _playerTransform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(directionToPlayer) * Quaternion.Euler(0, 180, 0);
    }
}
