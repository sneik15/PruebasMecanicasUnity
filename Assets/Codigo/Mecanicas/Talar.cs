using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talar : MonoBehaviour
{

    private bool talaComenzada;
    private int progreso;
    private int objetivo;

    private bool talaPosible;
    private bool arbolDetec;
    private GameObject arbol;

    public float tiempoTick;
    private float tiempoTickTrans;

    void Start()
    {
        ResetearTala();
    }

    // Update is called once per frame
    void Update()
    {
        talaPosible = ComprobarTalaPosible();

        if (Input.GetKey(KeyCode.Mouse0) && talaComenzada && talaPosible) //TODO: remaping de teclas
        {
            ContinuarTala();
        }
        else if(Input.GetKey(KeyCode.Mouse0) && talaPosible && Global.JugadorSuelo) //TODO: remaping de teclas
        {
            ComenzarTala();
        }
        else if(talaComenzada && (!talaPosible || !Input.GetKey(KeyCode.Mouse0))) //TODO: remaping de teclas
        {
            ResetearTala();
        }
    }

    private void ComenzarTala()
    {
        talaComenzada = true;
        IHerramientaBase hachaAc = (Global.ItemSelec as IHerramientaBase);
        objetivo = int.Parse(GestorDatos.Herramientas[Enum.GetName(typeof(TipoHerramienta), hachaAc.TipoHerramientaU)][Enum.GetName(typeof(Tier), hachaAc.Tier)]["Golpes"][arbol.tag].ToString());
        progreso++;
        Global.MovBloq = true;
        //Reproducir animacion golpe
    }

    private void ContinuarTala()
    {
        Global.MovBloq = true;
        if (progreso >= objetivo)
        {
            FinalizarTala();
            return;
        }
        tiempoTickTrans += Time.deltaTime;
        if (tiempoTickTrans >= tiempoTick)
        {
            tiempoTickTrans = 0;
            progreso++;
            //Reproducir animacion golpe
        }
    }

    private void FinalizarTala()
    {
        //AnimCaida o las siguientes 2 lineas
        arbol.transform.Translate(new Vector3(0, 0.1f, 0));
        arbol.layer = 12; //Elementos desapareciendo
        arbol.GetComponent<Rigidbody>().isKinematic = false;
        arbol.GetComponent<Rigidbody>().AddForce(transform.forward * 15);
        Destroy(arbol, 3);
        ResetearTala();
        CrearDrops();
    }

    private void CrearDrops()
    {
        System.Random rnd = new System.Random();
        JObject drops = JObject.Parse(GestorDatos.Drops["Arbol"].ToString());

        foreach(KeyValuePair<string, JToken> drop in drops)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Drops/Arboles/" + drop.Key);

            for (int i = 0; i < int.Parse((drop.Value as JObject)[arbol.tag].ToString()); i++)
            {
                if (rnd.Next(1, 101) <= int.Parse((drop.Value as JObject)["Probabilidad"].ToString()))
                {
                    GameObject ins = Instantiate(prefab, arbol.transform.position, arbol.transform.rotation);
                    ins.transform.Translate((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());
                    ins.tag = "Item";
                    ins.GetComponent<IItemBase>().Autodestruir = true;
                }
            }
        }

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.StartsWith("Arbol"))
        {
            arbolDetec = true;
            arbol = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.StartsWith("Arbol"))
        {
            arbolDetec = false;
        }
    }

    private bool ComprobarTalaPosible()
    {
        if (arbolDetec && Global.ItemSelec != null && Global.ItemSelec is IHerramientaBase && (Global.ItemSelec as IHerramientaBase).TipoHerramientaU == TipoHerramienta.Hacha)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ResetearTala()
    {
        talaComenzada = false;
        progreso = 0;
        objetivo = 0;
        tiempoTickTrans = 0;
        Global.MovBloq = false;
    }
}
