﻿using System;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earth : Component {

    [SerializeField]
    private GameObject initialGameObject;

    [SerializeField]
    private GameObject[] listOfAttacks;

    private float webAttackProb = 0.85f;

    private float documentAttackProb = 0.3f;

    private IEnumerator coroutine;

    private void Start() {
        // Some initialization stuff
        Name = "Earth";
        Debug.Log("Start Shitty script...");
        this.coroutine = StartSpawningAttacks(5.0f);
        StartCoroutine(this.coroutine);
    }

    private IEnumerator StartSpawningAttacks(float time) {
        Debug.Log("Starting to spawn attacks...");
        // TODO This loop freezes the game
        // TODO Find another solution for this!!!
        while (true) { 
            Debug.Log("Waiting...");
            Debug.Log("Before: Current thread: " + System.Threading.Thread.CurrentThread);
            CreateRandomEnemy();
            yield return new WaitForSeconds(time);
            Debug.Log("Done Waiting!");
            Debug.Log("After: Current thread: " + System.Threading.Thread.CurrentThread);
            // Create random attack
            
        }
        // For testing
    }

    private void CreateRandomEnemy() {
        // TODO Set a condition which stops creation of enemy when true
        Debug.Log("Spawning random enemy");
        int randomInt = 0; //UnityEngine.Random.Range(0, 2);
        float rand = UnityEngine.Random.Range(0f, 1.0f);
        Debug.Log("randomInt: " + randomInt);
        Debug.Log("rand: " + rand);
        // If condition is true create Web Attack
        //if (randomInt == 0 && (rand < this.webAttackProb)) {
        //    Debug.Log("Creating WebAttack...");
        //    WebAttack webAttack = (new GameObject("WebAttack")).AddComponent<WebAttack>();
        //    webAttack.Run((Component) this.initialGameObject.GetComponent(typeof(Component)));
        //}

        //WebAttack webAttack = (new GameObject("WebAttack")).AddComponent<WebAttack>();
        //webAttack.Run((Component) this.initialGameObject.GetComponent(typeof(Component)));

        //// If condition is true, create document attack
        //else if (randomInt == 1 && (rand) < this.documentAttackProb) {
        //    Debug.Log("Creating DocumentAttack...");
        //    DocumentAttack documentAttack = (new GameObject("DocumentAttack")).AddComponent<DocumentAttack>();
        //    documentAttack.Run((Component) this.initialGameObject.GetComponent(typeof(Component)));
        //}
    }

    /* TODO Add ransom decision consequence and variable related stuff in this script */

}

