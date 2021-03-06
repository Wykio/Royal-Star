﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyController : MonoBehaviour
{
    [Header ("Référence")]
    [SerializeField] private WeaponManagerScript[] weaponManagerScript;
    [SerializeField] private EnemyExposer[] botExposers;
    [SerializeField] private PhotonView photonView;
    
    [Header ("Déplacement")]
    private GameObject[] targets;
    [SerializeField] private float lookRange = 1000.0f;
    [SerializeField] private float maxSpeed = 5.0f;
    [SerializeField] private float targetDistance;
    
    [Header ("Noise")]
    private Vector3 targetingNoise;

    private float perlinNoiseX = 0.0f;
    private float perlinNoiseY = 0.0f;
    private float perlinNoiseZ = 0.0f;
    private float noiseMultiplier = 100.0f;
    private float elapsedTime = 0.0f;
    
    private void Start()
    {
        targets = EnemyManager.instance.AiTargets;
        targetingNoise = new Vector3(0.0f,0.0f,0.0f);
    }

    private void Update()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            int indice = 0;

            foreach(var bot in botExposers)
            {
                if (bot.rootGameObject.activeSelf)
                {
                    int idTargetLocked = getClosestTargetId(targets, indice);

                    targetDistance = getDistanceBetween(targets[idTargetLocked].transform.position, botExposers[indice].transform.position);

                    UpdateNoise();

                    if (targetDistance <= lookRange)
                    {
                        botExposers[indice].transform.LookAt(targets[idTargetLocked].transform.position + targetingNoise);

                        if (targetDistance <= lookRange / 2)
                        {
                            photonView.RPC("BotShootRPC", RpcTarget.All, indice);

                            if (targetDistance >= 50)
                            {
                                botExposers[indice].transform.position = Vector3.MoveTowards(botExposers[indice].transform.position, targets[idTargetLocked].transform.position + targetingNoise, maxSpeed * (targetDistance / lookRange));
                            }
                        }
                    }
                }
                indice++;
            }
        }
    }

    private int getClosestTargetId(GameObject[] myTargets, int indice)
    {
        int i = 0;
        float targetDistance = 0.0f;
        float targetDistanceMin = 10000.0f;     //besoin d'un nombre tres grand
        int idTargetDistanceMin = 0;


        for (i = 0; i < myTargets.Length; i++)    //20 c'est le nombre de joueur
        {
            if (targets[i].activeSelf == true)
            {
                targetDistance = getDistanceBetween(targets[i].transform.position, botExposers[indice].transform.position);
                if (targetDistance < targetDistanceMin)
                {
                    targetDistanceMin = targetDistance;
                    idTargetDistanceMin = i;
                }
            }
        }
        return idTargetDistanceMin;
    }

    private void UpdateNoise()
    {
        elapsedTime = Time.time;
        perlinNoiseX = Mathf.PerlinNoise(elapsedTime, 0);
        perlinNoiseY = Mathf.PerlinNoise(elapsedTime + 1, 0);
        perlinNoiseZ = Mathf.PerlinNoise(elapsedTime + 2, 0);
        targetingNoise.x = (perlinNoiseX * noiseMultiplier) - (noiseMultiplier/2);
        targetingNoise.y = (perlinNoiseY * noiseMultiplier) - (noiseMultiplier/2);
        targetingNoise.z = (perlinNoiseZ * noiseMultiplier) - (noiseMultiplier/2);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRange);
    }

    private float getDistanceBetween(Vector3 a, Vector3 b)
    {
        float res = (float)Math.Sqrt(Math.Pow(b.x - a.x,2)+Math.Pow(b.y - a.y,2)+Math.Pow(b.z - a.z,2));
        return res;
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
        }
    }

    [PunRPC]
    private void BotShootRPC(int indice)
    {
        weaponManagerScript[indice].Shoot(0);
    }
}
