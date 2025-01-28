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
        NPCAnimationState _previousAnimState = NPCAnimationState.NONE; // Bir önceki animasyon

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
                    Debug.Log("Current State: IDLE");
                    break;

                case NPCAnimationState.IDLERUSH:
                    animator.SetBool("isIdleRush", true); // IDLERUSH için aynı animasyon
                    Debug.Log("Current State: IDLERUSH");
                    break;

                case NPCAnimationState.WALKING:
                    animator.SetBool("isWalking", true);
                    Debug.Log("Current State: WALKING");
                    break;

                case NPCAnimationState.SHITTING:
                    animator.SetBool("isShitting", true);
                    Debug.Log("Current State: SHITTING");
                    break;

                case NPCAnimationState.SHITTINGALATURKA:
                    Debug.Log("Current State: SHITTINGALATURKA (No Animation)");
                    break;

                case NPCAnimationState.RUNNING:
                    animator.SetBool("isRunning", true);
                    Debug.Log("Current State: RUNNING");
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

