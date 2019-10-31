﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingController : MonoBehaviour
{
    [SerializeField]
    BobberHitWater bobber;
    [SerializeField]
    HeaderText header;
    [SerializeField]
    ItemHolsterManager holster;
    [SerializeField]
    List<GameObject> fish = new List<GameObject>();
    RodReelBackBobber reelBack;
    [SerializeField]
    PlayerStatsSO playerstats;
    int index = 0;

    FMOD.Studio.EventInstance FishOn;

    Coroutine fishRoutine = null;

    enum FishState {WatingForBite, Bite, Missed}
    [SerializeField]
    FishState fishing;

    bool isWatingForBite;
    // Start is called before the first frame update
    void Start()
    {
        reelBack = FindObjectOfType<RodReelBackBobber>();
        //   holster = FindObjectOfType<ItemHolsterManager>();
        //bobber = FindObjectOfType<BobberHitWater>();
        reelBack.OnReel += HandleReel;
        bobber.OnHitWater += HandleMinigame;
        fishing = FishState.Missed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (fishing == FishState.Bite)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Fish Splash Out");
                StartCoroutine(CatchDelay());
              
            }
            else
            {
                fishing = FishState.Missed;
            }

        }

 
    }

    void HandleReel()
    {
        fishing = FishState.Missed;
        if(fishRoutine != null)
        {
            StopCoroutine(fishRoutine);
        }

    }

    IEnumerator CatchDelay()
    {
        yield return new WaitForSeconds(.1f);
        //  holster.SetItem(fish[Random.Range(0, fish.Count - 1)]);
        if (index >= fish.Capacity)
        {
            index = 0;
        }
        holster.SetItem(fish[index]);
        if(index == 1)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Bridle Stinger");
            Debug.Log("You caught the bridle!");
        }

        if(index >= fish.Capacity)
        {
            index = 0;
        }
        index++;
        header.WriteHeader("Place in Front Chest");
        playerstats.isOccupied = false;
    }

    void HandleMinigame()
    {
        fishing = FishState.WatingForBite;//isWatingForBite = true;
        fishRoutine = StartCoroutine(WaitForCatch());
    }

    IEnumerator WaitForCatch()
    {
        yield return new WaitForSeconds(Random.Range(5f,10f));
        Debug.Log("You got a Bite!");
        FishOn = FMODUnity.RuntimeManager.CreateInstance("event:/Fish On the Hook");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(FishOn, GetComponent<Transform>(), GetComponent<Rigidbody>());
        FishOn.start();
        fishing = FishState.Bite;
        yield return new WaitForSeconds(Random.Range(1f, 2f));
        Debug.Log("Too Slow!");
        fishing = FishState.Missed;
        FishOn.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
