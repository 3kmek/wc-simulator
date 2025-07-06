using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class Voyeur : MonoBehaviour, IInteractable
{
    FirstPersonController player;

    [SerializeField] Transform camHolder;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetInteractionText()
    {
        return "Sapik time";
    }

    public void Interact()
    {
        player.previousRotation = player.playerCamera.transform.rotation;
        
        player.playerCamera.transform.SetParent(null);
        player.playerCamera.transform.SetParent(camHolder);
        
        player.playerCanMove = false;
        player.cameraCanMove = false;
        player.allowLookUpdate = false;
        
        player.playerCamera.transform.DOLocalRotate(new Vector3(0,0,-30f), 0.3f);

        player.playerCamera.transform.DOMove(camHolder.position + new Vector3(0,0,1f), .2f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            player.playerCamera.transform.DOMove(camHolder.position, 1f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                player.playerCamera.transform.DORotate(camHolder.localEulerAngles, .5f).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        player.cameraCanMove = true;
                        
                        player.VoyeurMode = true;
                        player.enableHeadBob = false;
                        
                        player.yaw = 0;
                        player.pitchX = 0;
                        
                        player.allowLookUpdate = true;
                    }
                    );
                
            });
        });
        
        
    //    Camera.main.transform.DORotate(camHolder.rotation.eulerAngles, 1f, RotateMode.FastBeyond360).OnComplete(() =>
    //    {
            //Camera.main.transform.rotation = camHolder.rotation;
    //    });
        
        //Camera.main.transform.position = camHolder.position;
        //Camera.main.transform.rotation = camHolder.rotation;


    }
}
