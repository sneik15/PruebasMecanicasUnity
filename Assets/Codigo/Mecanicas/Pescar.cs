using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pescar : MonoBehaviour
{
    private GameObject objetivoPesca;
    private bool pescando;
    private bool pezGen;
    private bool pezAcercado;
    private bool cLanzada;
    private bool lanzando;
    private float tiempoPasado;
    public float TiempoAnimLanzar;
    public float VelocidadPez;
    private float tiempoSpawnPez;

    private string tipoAgua;
    private Vector3 posicionAgua;
    private GameObject anzueloAc;
    private GameObject pezAc;
    private JObject objetoPescando;
    private string ceboAc;
    private System.Random rnd;

    private GameObject PanelMensajesMecanicasUI;


    void Start()
    {
        objetivoPesca = GameObject.Find("ObjetivoPesca");
        ceboAc = "";
        rnd = new System.Random();

        PanelMensajesMecanicasUI = GameObject.Find("PanelMensajesMecanicas");
        PanelMensajesMecanicasUI.SetActive(false); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !cLanzada && Global.JugadorSuelo && ComprobarPescaPosible()) //TODO: remaping de teclas
        {
            Lanzar();
        }
        else if (lanzando)
        {
            tiempoPasado += Time.deltaTime;
            if(tiempoPasado >= TiempoAnimLanzar)
            {
                FinLanzar();
            }
        }
        else if (cLanzada && !pezGen)
        {
            tiempoPasado += Time.deltaTime;
            if (tiempoPasado >= tiempoSpawnPez)
            {
                GenerarPez();
            }
        }
        else if (pezGen && !pezAcercado)
        {
            AcercarPez();
        }
        else if (pezAcercado && !pescando)
        {
            //el jugador le da a pescar
            PicarAnzuelo();
        }
        else if (pescando)
        {
            //pesca
            ControladorPesca();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && (cLanzada || pezGen || pescando)) //TODO: remaping de teclas
        {
            //Cancelar pesca
            Destroy(anzueloAc);
            if (pezGen)
            {
                Destroy(pezAc);
            }
            Global.MovBloq = false;
            cLanzada = false;
            pezGen = false;
            pescando = false;
            pezAcercado = false;
        }
    }

    private void ControladorPesca()
    {

    }

    private void PicarAnzuelo()
    {
        if (!PanelMensajesMecanicasUI.activeSelf)
        {
            PanelMensajesMecanicasUI.transform.GetChild(0).GetComponent<Text>().text = GestorTraducciones.TraducirTexto("{PezPicado}");
            PanelMensajesMecanicasUI.SetActive(true);
        }
        else if(Input.GetKeyDown(KeyCode.Mouse0)) //TODO: remaping de teclas
        {
            pescando = true;
            //Animacion de pesca empezada
        }
    }

    private void AcercarPez()
    {
        if((pezAc.transform.position - posicionAgua).magnitude > 0.3)
        {
            pezAc.transform.Translate(new Vector3(0, 0, VelocidadPez * Time.deltaTime));
        }
        else
        {
            pezAcercado = true;
        }
    }

    private void GenerarPez()
    {
        //Genera la posicion del pez
        bool posicionCorrecta = false;
        Vector3 posicionPez = new Vector3();
        while (!posicionCorrecta)
        {
            Vector2 direccion = new Vector2((float)rnd.NextDouble(), (float)rnd.NextDouble()).normalized;
            float distancia = (float)rnd.NextDouble();
            distancia += rnd.Next(1, 4);
            distancia = Mathf.Clamp(distancia, 1f, 3f);
            direccion *= distancia;
            if (rnd.Next(0, 2) == 0)
            {
                direccion = -direccion;
            }

            posicionPez = posicionAgua + new Vector3(direccion.x, 0, direccion.y);

            RaycastHit hit;
            if (Physics.Raycast(posicionPez + new Vector3(0, 1, 0), new Vector3(0, -1, 0), out hit))
            {
                if (hit.transform.gameObject.tag.StartsWith("Agua"))
                {
                    string tipoAguaHit = hit.transform.gameObject.tag;
                    if (tipoAguaHit == tipoAgua)
                    {
                        posicionCorrecta = true;
                    }
                }
            }
        }

        GameObject pezPrefab = Resources.Load<GameObject>("Prefabs/AyudaMecanicas/Pesca/Pez");
        pezAc = Instantiate(pezPrefab, posicionPez, pezPrefab.transform.rotation);
        pezAc.transform.LookAt(posicionAgua);

        //Genera el tipo de pez u objeto
        JObject dropsAgua = JObject.Parse(GestorDatos.Drops[tipoAgua].ToString());
        int prob = rnd.Next(1, 101);
        JToken dropElegido = null;

        foreach (JProperty property in dropsAgua.Properties())
        {
            if (prob >= int.Parse(property.Value["ProbabilidadIni"].ToString()) && prob <= int.Parse(property.Value["ProbabilidadFin"].ToString()))
            {
                dropElegido = property.Value;
                break;
            }
        }

        JObject tipoDropEle = JObject.Parse(dropElegido.ToString());
        bool dropFinalEle = false;
        while (!dropFinalEle)
        {
            int posibleDrop = rnd.Next(2, tipoDropEle.Count);
            int probabilidad = rnd.Next(1, 101);
            int c = 0;
            foreach (JProperty property in tipoDropEle.Properties())
            {
                if(c == posibleDrop)
                {
                    if (JObject.Parse(property.Value.ToString()).ContainsKey("Cebo") && property.Value["Cebo"].ToString() == ceboAc)
                    {
                        if (probabilidad <= int.Parse(property.Value["ProbabilidadConC"].ToString()))
                        {
                            objetoPescando = JObject.Parse(property.Value.ToString());
                            dropFinalEle = true;
                        }
                    }
                    else
                    {
                        if (probabilidad <= int.Parse(property.Value["Probabilidad"].ToString()))
                        {
                            objetoPescando = JObject.Parse(property.Value.ToString());
                            Debug.Log(objetoPescando["Nombre"].ToString());
                            dropFinalEle = true;
                        }
                    }
                    break;
                }
                c++;
            }
        }

        pezGen = true;
    }

    private void FinLanzar()
    {
        lanzando = false;
        cLanzada = true;
        GameObject anzueloPrefab = Resources.Load<GameObject>("Prefabs/AyudaMecanicas/Pesca/Anzuelo");
        //Conexion hilo anzuelo con caña
        anzueloAc = Instantiate(anzueloPrefab, posicionAgua, anzueloPrefab.transform.rotation);
        tiempoPasado = 0;
        tiempoSpawnPez = rnd.Next(2, 6);
        tiempoSpawnPez += (float)rnd.NextDouble();
        tiempoSpawnPez = Mathf.Clamp(tiempoSpawnPez, 2f, 5f);
    }

    private void Lanzar()
    {
        Global.MovBloq = true;
        //Animacion de lanzar
        tiempoPasado = 0;
        lanzando = true;
    }

    //Raycast para comprobar si debajo del objetivo hay agua
    private bool ComprobarPescaPosible()
    {
        bool posible = false;
        if (Global.ItemSelec != null && Global.ItemSelec is IHerramientaBase && (Global.ItemSelec as IHerramientaBase).TipoHerramientaU == TipoHerramienta.CanyaPescar)
        {
            RaycastHit hit;
            if (Physics.Raycast(objetivoPesca.transform.position, objetivoPesca.transform.forward, out hit))
            {
                if (hit.transform.gameObject.tag.StartsWith("Agua"))
                {
                    tipoAgua = hit.transform.gameObject.tag;
                    posible = true;
                    posicionAgua = hit.point;
                }
            }
        }
        return posible;
    }
}
