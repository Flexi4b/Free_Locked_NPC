using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CageBehavior : MonoBehaviour
{
    public GameObject LockedSkin;
    public bool CageIsOpen = true;
    public float SetFreeProgress;

    [SerializeField] private float _unlockSpeed;
    private float _unlockDoneWhen = 30f;
    private bool isCageUnlocked;
    private bool isSettingFree;
    private bool startClosing;

    [SerializeField] private float _timerUntilClosed = 10.0f;

    private NPC_Behavior _NPC;

    private void Start()
    {
        SetFreeProgress = 0;
        isCageUnlocked = false;
        isSettingFree = false;
        startClosing = false;
    }

    private void Update()
    {
        CageHasOpened();
        UnlockingTriggerd();
        ClosingCage();
    }

    private void OnTriggerStay(Collider other)
    {
        //only do this when the other object has the tag Survivor or NPC
        if (other.gameObject.CompareTag("Survivor") || other.gameObject.CompareTag("NPC"))
        {
            Debug.Log(3);
            //Getting the needed components from the other object
            _NPC = other.gameObject.GetComponent<NPC_Behavior>();
            LockedUpNPC _lockedNPC = this.gameObject.GetComponent<LockedUpNPC>();
            //as long as the state from the NPC is not MoveToCage, do this
            if (_NPC.State != NPC_Behavior.StateOfNPC.MoveToCage)
            {
                Debug.Log(4);
                isSettingFree = true;
                startClosing = false;

                //if SetFreeProgress is higher or the same as 30, change the NPC state
                if (SetFreeProgress >= 30f)
                {
                    _NPC.State = NPC_Behavior.StateOfNPC.FleePlayer;
                }
            }
            
            //when the state from the NPC is MoveToCage and the bool CageIs open, do the following
            if (_NPC.State == NPC_Behavior.StateOfNPC.MoveToCage && CageIsOpen)
            {
                LockedSkin.SetActive(true);
                _lockedNPC.LockedNPC.SetActive(false);
                _NPC.FreeNPC = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //NPC is no longer setting the lockedup NPC free
        isSettingFree = false;
        StartCoroutine(ClosingTimer(_timerUntilClosed));
    }

    /// <summary>
    /// SetFreeProgress gets increased to open the cage
    /// </summary>
    private void StartUnlocking()
    {
        Debug.Log(0);
        SetFreeProgress += _unlockSpeed * Time.deltaTime;
        Debug.Log(1);
    }

    /// <summary>
    /// When SetFreeProgress is bigger than _unlockDoneWhen, the cage is unlocked
    /// </summary>
    private void CageHasOpened()
    {
        if (SetFreeProgress >= _unlockDoneWhen)
        {
            isCageUnlocked = true;
            FreeingNPCDone();
        }
    }

    /// <summary>
    /// When NPC stops unlocking cage, decreases SetFreeProgress timer
    /// </summary>
    private void RemoveUnlockingProgress()
    {
        if (SetFreeProgress >= 10 && isSettingFree == false)
        {
            SetFreeProgress -= _unlockSpeed * Time.deltaTime;
        }
    }

    /// <summary>
    /// Starts the unlocking progress
    /// </summary>
    private void UnlockingTriggerd()
    {
        if (isSettingFree == true)
        {
            Debug.Log(2);
            StartUnlocking();
        }
    }

    /// <summary>
    /// Decreases the unlocking timer after a few seconds
    /// </summary>
    IEnumerator ClosingTimer(float time)
    {
        yield return new WaitForSeconds(time);
        startClosing = true;
    }

    /// <summary>
    /// Gate is closing
    /// </summary>
    private void ClosingCage()
    {
        if (startClosing == true)
        {
            RemoveUnlockingProgress();
        }
    }

    /// <summary>
    /// Lockedup NPC is set free
    /// </summary>
    private void FreeingNPCDone()
    {
        if (isCageUnlocked)
        {
            this.gameObject.GetComponent<BoxCollider>().isTrigger = false;
            this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            CageIsOpen = true;
        }
    }
}
