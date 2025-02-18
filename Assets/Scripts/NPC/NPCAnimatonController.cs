using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace NPC
{
    public enum NPCAnimationState
    {
        IDLE,
        IDLERUSH,
        WALKING,
        SHITTING,
        SHITTINGALATURKA,
        RUNNING,
        NONE
    }
    
    public class NPCAnimatonController : MonoBehaviour
    {
        [SerializeField] NPCAnimationState _currentAnimState = NPCAnimationState.IDLE; // Varsayılan animasyon
        

        [SerializeField]Animator animator;
        
        [SerializeField] Transform _playerTransform;
        [SerializeField] Transform _NPCHeadTransform;

        

        private void Start()
        {
            animator = gameObject.GetComponent<Animator>();
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void ResetAllAnimations()
        {
            this.animator.SetBool("isIdle", false);
            this.animator.SetBool("isIdleRush", false);
            this.animator.SetBool("isWalking", false);
            this.animator.SetBool("isRunning", false);
            this.animator.SetBool("isShitting", false);
        }
        private void Update()
        {
            UpdateAnimationState();
        }

        // Animasyon durumunu güncelleyen metod
        private void UpdateAnimationState()
        {
            
            // Yeni durumu aktifleştir
            switch (_currentAnimState)
            {
                case NPCAnimationState.IDLE:
                    animator.SetBool("isIdle", true);
                    transform.DORotate(new Vector3(0,180f,0f), 1f).OnComplete(() =>
                    {
                        RotateHeadTowardsPlayer();
                    });
                    break;

                case NPCAnimationState.IDLERUSH:
                    animator.SetBool("isIdleRush", true); // IDLERUSH için aynı animasyon
                    transform.DORotate(new Vector3(0,180f,0f), 1f).OnComplete(() =>
                    {
                        RotateHeadTowardsPlayer();
                    });
                    break;

                case NPCAnimationState.WALKING:
                    animator.SetBool("isWalking", true);
                    break;

                case NPCAnimationState.SHITTING:
                    animator.SetBool("isShitting", true);
                    break;

                case NPCAnimationState.SHITTINGALATURKA:
                    break;

                case NPCAnimationState.RUNNING:
                    animator.SetBool("isRunning", true);
                    break;

                default:
                    Debug.LogWarning("Unknown Animation State");
                    break;
            }
        }
        private void RotateHeadTowardsPlayer()
        {
            
        }


        // Animasyon durumunu dışarıdan değiştirmek için bir metod
        public void ChangeAnimationState(NPCAnimationState newState)
        {
            if (_currentAnimState != newState) // Aynı durumu tekrar tetikleme
            {
                _currentAnimState = newState;
                ResetAllAnimations();
                UpdateAnimationState();
            }
        }
        
        
        
    }
}

