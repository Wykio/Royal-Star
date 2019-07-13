using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataVisualizerScript : MonoBehaviour
{
    [Header("Composants")]
    [SerializeField] string cheminFichierData;
    [SerializeField] Material tirBasique;
    [SerializeField] Material tirBleu;
    [SerializeField] Material tirVert;
    [SerializeField] Material tirRouge;
    [SerializeField] Material kill;
    [SerializeField] Material mort;

    [SerializeField] GameObject cam;
    [SerializeField] GameObject dataVisualized;
    [SerializeField] float camSpeed;
    [SerializeField] float camRotateSpeed;
    [SerializeField] float biomeSpeed;

    [Header("Interface")]
    [SerializeField] Text biome;
    [SerializeField] Text nbPartiesText;
    [SerializeField] Text nbBasicText;
    [SerializeField] Text nbBleuText;
    [SerializeField] Text nbVertText;
    [SerializeField] Text nbRougeText;
    [SerializeField] Text nbKillText;
    [SerializeField] Text nbMortText;
    [SerializeField] Text nbMortBiomeText;
    [SerializeField] Text tirsBasiquesParPartieText;
    [SerializeField] Text tirsBleusParPartieText;
    [SerializeField] Text tirsVertsParPartieText;
    [SerializeField] Text tirsRougesParPartieText;
    [SerializeField] Text killParPartieText;
    [SerializeField] Text mortsParPartieText;
    [SerializeField] Text mortsBiomeParPartieText;

    int nbParties = 0;
    int nbBasic = 0;
    int nbBleu = 0;
    int nbVert = 0;
    int nbRouge = 0;
    int nbKill = 0;
    int nbMort = 0;
    int nbMortBiome = 0;
    int currentBiome = 1;

    // Start is called before the first frame update
    void Start()
    {
        //curseur de la souris locké et non visible
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        string[] dataFromFile;

        try
        {
            dataFromFile = File.ReadAllLines(@cheminFichierData);
        }
        catch(Exception e)
        {
            dataFromFile = null;
        }

        if(dataFromFile != null)
        {
            foreach(var ligne in dataFromFile)
            {
                if(ligne == "NEWGAME")
                {
                    nbParties++;
                }
                else
                {
                    var data = ligne.Split('_');

                    switch(data[0])
                    {
                        case "basic":
                            GameObject cube = (GameObject)Instantiate(dataVisualized);
                            DataVisualizedExposerScript exposer = cube.GetComponent<DataVisualizedExposerScript>();
                            exposer.setMaterial(tirBasique);
                            cube.name = "tir basique";
                            cube.transform.localScale = new Vector3(5f, 5f, 5f);

                            Vector3 position = new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
                            Quaternion rotation = new Quaternion(float.Parse(data[4]), float.Parse(data[5]), float.Parse(data[6]), float.Parse(data[7]));
                            cube.transform.position = position;
                            cube.transform.rotation = rotation;
                            nbBasic++;

                            break;

                        case "bleue":

                            GameObject cubeb = (GameObject)Instantiate(dataVisualized);
                            DataVisualizedExposerScript exposerb = cubeb.GetComponent<DataVisualizedExposerScript>();
                            exposerb.setMaterial(tirBleu);
                            cubeb.name = "tir Bleu";
                            cubeb.transform.localScale = new Vector3(5f, 5f, 5f);

                            Vector3 positionb = new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
                            Quaternion rotationb = new Quaternion(float.Parse(data[4]), float.Parse(data[5]), float.Parse(data[6]), float.Parse(data[7]));
                            cubeb.transform.position = positionb;
                            cubeb.transform.rotation = rotationb;
                            nbBleu++;
                            break;

                        case "verte":

                            GameObject cubev = (GameObject)Instantiate(dataVisualized);
                            DataVisualizedExposerScript exposerv = cubev.GetComponent<DataVisualizedExposerScript>();
                            exposerv.setMaterial(tirVert);
                            cubev.name = "tir vert";
                            cubev.transform.localScale = new Vector3(5f, 5f, 5f);

                            Vector3 positionv = new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
                            Quaternion rotationv = new Quaternion(float.Parse(data[4]), float.Parse(data[5]), float.Parse(data[6]), float.Parse(data[7]));
                            cubev.transform.position = positionv;
                            cubev.transform.rotation = rotationv;
                            nbVert++;

                            break;

                        case "rouge":

                            GameObject cuber = (GameObject)Instantiate(dataVisualized);
                            DataVisualizedExposerScript exposerr = cuber.GetComponent<DataVisualizedExposerScript>();
                            exposerr.setMaterial(tirRouge);
                            cuber.name = "tir rouge";
                            cuber.transform.localScale = new Vector3(5f, 5f, 5f);

                            Vector3 positionr = new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
                            Quaternion rotationr = new Quaternion(float.Parse(data[4]), float.Parse(data[5]), float.Parse(data[6]), float.Parse(data[7]));
                            cuber.transform.position = positionr;
                            cuber.transform.rotation = rotationr;
                            nbRouge++;

                            break;

                        case "kill":

                            GameObject cubek = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            var meshk = cubek.GetComponent<MeshRenderer>();
                            meshk.material = kill;
                            cubek.name = "kill";
                            cubek.transform.localScale = new Vector3(5f, 5f, 5f);

                            Vector3 positionk = new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
                            cubek.transform.position = positionk;

                            nbKill++;

                            break;

                        case "mort":

                            GameObject cubem = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            var meshm = cubem.GetComponent<MeshRenderer>();
                            meshm.material = mort;
                            cubem.name = "mort";
                            cubem.transform.localScale = new Vector3(5f, 5f, 5f);

                            Vector3 positionm = new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
                            cubem.transform.position = positionm;

                            nbMort++;

                            break;
                    }
                }
            }

            nbPartiesText.text = nbParties + " Parties jouées";
            nbBasicText.text = nbBasic + " Tirs de laser basique : " + (float)nbBasic/(float)(nbBasic + nbBleu + nbVert + nbRouge) * 100 + "%";
            nbBleuText.text = nbBleu + " Tirs d'armes bleues : " + (float)nbBleu / (float)(nbBasic + nbBleu + nbVert + nbRouge) * 100 + "%";
            nbVertText.text = nbVert + " Tirs d'armes vertes : " + (float)nbVert / (float)(nbBasic + nbBleu + nbVert + nbRouge) * 100 + "%";
            nbRougeText.text = nbRouge + " Tirs d'armes rouges : " + (float)nbRouge / (float)(nbBasic + nbBleu + nbVert + nbRouge) * 100 + "%";
            nbKillText.text = nbKill + " Kills";
            nbMortText.text = nbMort + " Morts";
            nbMortBiomeText.text = (nbMort - nbKill) + " Morts par l'environnement : " + (nbMort - nbKill) / nbMort * 100 + "%";
            tirsBasiquesParPartieText.text = (float)nbBasic / (float)nbParties + " Tirs de laser basique par partie";
            tirsBleusParPartieText.text = (float)nbBleu / (float)nbParties + " Tirs d'armes bleues par partie";
            tirsVertsParPartieText.text = (float)nbVert / (float)nbParties + " Tirs d'armes vertes par partie";
            tirsRougesParPartieText.text = (float)nbRouge / (float)nbParties + " Tirs d'armes rouges par partie";
            killParPartieText.text = (float)nbKill / (float)nbParties + " Kills par partie";
            mortsParPartieText.text = (float)nbMort / (float)nbParties + " Morts par partie";
            mortsBiomeParPartieText.text = (float)(nbMort - nbKill) / (float)nbParties + " Morts par l'environnement par partie";
        }
    }

    private void Update()
    {
        float sourisHorizontale = Input.GetAxis("Mouse X");
        float sourisVerticale = Input.GetAxis("Mouse Y");

        if (Input.GetKey(KeyCode.Z))
        {
            cam.transform.Translate(Vector3.forward * camSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S))
        {
            cam.transform.Translate(-Vector3.forward * camSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            cam.transform.Translate(Vector3.right * camSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            cam.transform.Translate(-Vector3.right * camSpeed * Time.deltaTime);
        }

        if(sourisHorizontale != 0 && Input.GetMouseButton(1))
        {
            cam.transform.Rotate(0, sourisHorizontale * camRotateSpeed * Time.deltaTime, 0);
            sourisHorizontale = 0;
        }

        if (sourisVerticale != 0 && Input.GetMouseButton(1))
        {
            cam.transform.Rotate(-sourisVerticale * camRotateSpeed * Time.deltaTime, 0, 0);
            sourisVerticale = 0;
        }

        if(Input.GetKeyDown(KeyCode.A))
        {
            if(cam.transform.position.y < 4500f)
            {
                cam.transform.position = (new Vector3(cam.transform.position.x, cam.transform.position.y - cam.transform.position.y + 200, cam.transform.position.z));
            }
            else
            {
                cam.transform.position = (new Vector3(cam.transform.position.x, cam.transform.position.y - 5000, cam.transform.position.z));
                currentBiome--;
                biome.text = "Biome " + currentBiome;
            }
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            if(cam.transform.position.y > 15000f)
            {
                cam.transform.position = (new Vector3(cam.transform.position.x, cam.transform.position.y - cam.transform.position.y + 15200, cam.transform.position.z));
            }
            else
            {
                cam.transform.position = (new Vector3(cam.transform.position.x, cam.transform.position.y + 5000, cam.transform.position.z));
                currentBiome++;
                biome.text = "Biome " + currentBiome;
            }
        }
    }
}
