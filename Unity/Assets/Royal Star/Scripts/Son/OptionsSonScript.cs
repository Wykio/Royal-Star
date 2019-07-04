using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsSonScript : MonoBehaviour
{
    [SerializeField] private float parametreSonBruitages { get; set; }
    [SerializeField] private float parametreSonMusiques { get; set; }

    [Header("Interface Menu Option")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Slider sliderBruitages;
    [SerializeField] private Slider sliderMusiques;
    [SerializeField] private Text texteBruitages;
    [SerializeField] private Text texteMusiques;
    [SerializeField] private Button boutonRetour;

    [Header("Références")]
    [SerializeField] private InterfaceManager gestionInterface;

    private void Awake()
    {
        //quand on clique sur le bouton Retour, on appelle la fonction pour sauvegarder les paramètres
        boutonRetour.onClick.AddListener(SauvegarderParametres);
    }

    //fonction pour afficher le menu d'option
    public void AfficherMenuOptions()
    {
        sliderBruitages.gameObject.SetActive(true);
        sliderMusiques.gameObject.SetActive(true);
        texteBruitages.gameObject.SetActive(true);
        texteMusiques.gameObject.SetActive(true);
        boutonRetour.interactable = true;
        boutonRetour.gameObject.SetActive(true);
    }

    //fonction pour masquer le menu d'option
    public void MasquerMenuOptions()
    {
        sliderBruitages.gameObject.SetActive(false);
        sliderMusiques.gameObject.SetActive(false);
        texteBruitages.gameObject.SetActive(false);
        texteMusiques.gameObject.SetActive(false);
        boutonRetour.interactable = false;
        boutonRetour.gameObject.SetActive(false);
    }

    //fonction de sauvegarde des paramètres dans les variables;
    public void SauvegarderParametres()
    {
        parametreSonBruitages = sliderBruitages.value;
        parametreSonMusiques = sliderMusiques.value;
        MasquerMenuOptions();
        gestionInterface.AfficherMenuPrincipal(0,0);
    }
}
