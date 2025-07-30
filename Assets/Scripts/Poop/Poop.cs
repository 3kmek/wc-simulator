using System;
using UnityEngine;
using DG.Tweening;

namespace WC
{
    public class Poop : MonoBehaviour, IInteractable, IHoldable
    {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private MeshCollider _meshCollider;
        [SerializeField] private Transform holder;
        [SerializeField] private bool holdingAnimDone = false;
        [SerializeField] private GameObject player;
        [SerializeField] private Vector3 offset = new Vector3(0, 0.5f, 0);

        private void Start()
        {
            Installation();
        }
        
        void Installation()
        {
            rb = GetComponent<Rigidbody>();
            _meshCollider = GetComponent<MeshCollider>();
            holder = GameObject.Find("PlayerHolder").transform;
            player = GameObject.FindGameObjectWithTag("Player");
        }

        public string GetInteractionText()
        {
            return "Hold the poop\nPress [E]";
        }

        public void Interact()
        {
            
        }

        private void Update()
        {
            if (holdingAnimDone)
            {
                transform.position = holder.position + offset;
                transform.rotation = holder.rotation;
            }
        }

        public void OnPickup(Transform holdPosition)
        {
            rb.isKinematic = true;
            _meshCollider.enabled = false;
        
            // Önce parent'tan bağımsız hareket et
            transform.SetParent(null);
    
            // World pozisyonunda animasyon yap
            transform.DOMove(holder.position + offset, 0.3f) 
                .OnComplete(() => 
                {
                    // Animasyon bittiğinde parent'a ata
                    transform.DORotate(holder.rotation.eulerAngles, 0.2f)
                        .OnComplete(() =>
                        {
                            transform.SetParent(holder);
                            transform.position = holder.position + offset; // Sadece bir kez offset ekle
                            transform.rotation = holder.rotation;
                            player.GetComponent<PlayerInteraction>().isHoldingSomething = true;
                            holdingAnimDone = true;
                        });

                });
        }

        public void OnDrop()
        {
            if (holdingAnimDone)
            {
                transform.SetParent(null);
                rb.isKinematic = false;
                _meshCollider.enabled = true;
                
                holdingAnimDone = false;
            
                player.GetComponent<PlayerInteraction>()._currentHeldObject = null;
                player.GetComponent<PlayerInteraction>().isHoldingSomething = false;
            }
        }

        public bool IsHolding { get; }
        public void Use()
        {
            
        }
    }
}
