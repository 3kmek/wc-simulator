using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        // Animator parametrelerini cache yap
        private int isIdleHash, isIdleRushHash;
        private int isWalkingHash;
        private int isRunningHash;
        private int isShittingHash;

        private void Start()
        {
            animator = gameObject.GetComponent<Animator>();
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
                    break;

                case NPCAnimationState.IDLERUSH:
                    animator.SetBool("isIdleRush", true); // IDLERUSH için aynı animasyon
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

