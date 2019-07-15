using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusExposer : ItemExposerScript
{
    [SerializeField] private bool soinOuBouclier;
    [SerializeField] private int montantBonus;
    [SerializeField] private AudioClip sonBonusRamasse;

    void OnTriggerEnter(Collider other)
    {
        if (ramasse) return;

        //si le collider est un joueur
        if (other.gameObject.tag == "Player")
        {
            var vaisseau = other.attachedRigidbody.gameObject.GetComponent<ShipExposer>();

            //en fonction du type du bonus, on recharge les PV ou le bouclier du joueur
            if (soinOuBouclier)
            {
                if(vaisseau.getPV() < 200)
                {
                    vaisseau.lecteurSon.clip = sonBonusRamasse;
                    vaisseau.lecteurSon.volume = gestionSon.GetParametreBruitages();
                    vaisseau.lecteurSon.Play();
                    vaisseau.Soins(montantBonus);
                    ramasse = true;
                    SetPose(true);
                    DesactivationItem();
                }
            }
            else
            {
                if (vaisseau.getBouclier() < 100)
                {
                    vaisseau.lecteurSon.clip = sonBonusRamasse;
                    vaisseau.lecteurSon.volume = gestionSon.GetParametreBruitages();
                    vaisseau.lecteurSon.Play();
                    vaisseau.RechargeBouclier(montantBonus);
                    ramasse = true;
                    SetPose(true);
                    DesactivationItem();
                } 
            }
        }
        else
        {
            if(other.gameObject.tag == "Bullet")
            {
                ramasse = true;
                SetPose(true);
                DesactivationItem();
            }
        }
    }
}
