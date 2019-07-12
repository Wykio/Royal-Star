using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class DataVisualizerScript : MonoBehaviour
{
    [SerializeField] string cheminFichierData;
    [SerializeField] Material tirBasique;
    [SerializeField] Material tirBleu;
    [SerializeField] Material tirVert;
    [SerializeField] Material tirRouge;
    [SerializeField] Material kill;
    [SerializeField] Material mort;

    int nbParties = 0;

    // Start is called before the first frame update
    void Start()
    {
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
                            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            var mesh = cube.GetComponent<MeshRenderer>();
                            mesh.material = tirBasique;
                            cube.name = "tir basique";
                            cube.transform.localScale = new Vector3(5f, 5f, 5f);

                            Vector3 position = new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
                            cube.transform.position = position;
                            
                            break;

                        case "bleue":

                            GameObject cubeb = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            var meshb = cubeb.GetComponent<MeshRenderer>();
                            meshb.material = tirBleu;
                            cubeb.name = "tir Bleu";
                            cubeb.transform.localScale = new Vector3(5f, 5f, 5f);

                            Vector3 positionb = new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
                            cubeb.transform.position = positionb;

                            break;

                        case "verte":

                            GameObject cubev = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            var meshv = cubev.GetComponent<MeshRenderer>();
                            meshv.material = tirVert;
                            cubev.name = "tir vert";
                            cubev.transform.localScale = new Vector3(5f, 5f, 5f);

                            Vector3 positionv = new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
                            cubev.transform.position = positionv;

                            break;

                        case "rouge":

                            GameObject cuber = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            var meshr = cuber.GetComponent<MeshRenderer>();
                            meshr.material = tirRouge;
                            cuber.name = "tir rouge";
                            cuber.transform.localScale = new Vector3(5f, 5f, 5f);

                            Vector3 positionr = new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
                            cuber.transform.position = positionr;

                            break;

                        case "kill":

                            GameObject cubek = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            var meshk = cubek.GetComponent<MeshRenderer>();
                            meshk.material = kill;
                            cubek.name = "kill";
                            cubek.transform.localScale = new Vector3(5f, 5f, 5f);

                            Vector3 positionk = new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
                            cubek.transform.position = positionk;

                            break;

                        case "mort":

                            GameObject cubem = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            var meshm = cubem.GetComponent<MeshRenderer>();
                            meshm.material = mort;
                            cubem.name = "mort";
                            cubem.transform.localScale = new Vector3(5f, 5f, 5f);

                            Vector3 positionm = new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
                            cubem.transform.position = positionm;

                            break;
                    }
                }
            }
        }
    }
}
