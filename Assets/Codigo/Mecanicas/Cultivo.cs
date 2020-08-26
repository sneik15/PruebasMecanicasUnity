using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text.RegularExpressions;
using UnityEngine;

public class Cultivo : MonoBehaviour
{
    public GameObject ObjetivoCultivo;
    private GameObject ultTierra;
    private Semilla ultSemilla;
    private GameObject tierraBase;

    private Vector3 posicionTierraCul;

    void Start()
    {
        tierraBase = Resources.Load<GameObject>("Prefabs/Colocables/Tierra");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Global.JugadorSuelo && ComprobarRiegoPosible()) //TODO: remaping de teclas
        {
            Regar();
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && Global.JugadorSuelo && ComprobarAradoPosible()) //TODO: remaping de teclas
        {
            Arar();
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && Global.JugadorSuelo && ComprobarPlantadoPosible()) //TODO: remaping de teclas
        {
            Plantar();
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && Global.JugadorSuelo && ComprobarRecogidaPosible()) //TODO: remaping de teclas
        {
            Recoger();
        }
    }

    private void Recoger()
    {
        string cultivo = ultTierra.GetComponent<Tierra>().RecogerPlantaLista(out int cantidad);
        GameObject cultivoBase = Resources.Load<GameObject>("Prefabs/Drops/Cultivos/" + cultivo);
        GestionInventario inventario = gameObject.GetComponent<GestionInventario>();
        for (int i = 0; i < cantidad; i++)
        {
            GameObject cultivoInventario = Instantiate(cultivoBase, transform.position, transform.rotation);
            ItemGenerico culAgregar = cultivoInventario.GetComponent<ItemGenerico>();
            inventario.AgregarObjeto(culAgregar);
        }
        //Animacion de recoger
    }

    private void Plantar()
    {
        ultTierra.GetComponent<Tierra>().Plantar(ultSemilla.tipoSemilla);
        ultSemilla = null;
        gameObject.GetComponent<GestionInventario>().EliminarObjetoSeleccionado();
    }

    private void Regar()
    {
        Regadera regadera = (Global.ItemSelec as Regadera);
        if (regadera.AguaActual > 0)
        {
            regadera.Usar();
            ultTierra.GetComponent<Tierra>().Regado = true;
        }
    }

    private void Arar()
    {
        Vector3 posTierraCulFinal = new Vector3();
        posTierraCulFinal.y = posicionTierraCul.y + 0.5f;
        posTierraCulFinal.x = Mathf.RoundToInt(posicionTierraCul.x);
        posTierraCulFinal.z = Mathf.RoundToInt(posicionTierraCul.z);
        RaycastHit hit;
        if (Physics.Raycast(posTierraCulFinal, ObjetivoCultivo.transform.forward, out hit))
        {
            if (!hit.transform.gameObject.CompareTag("TierraCultivos"))
            {
                posTierraCulFinal.y = posicionTierraCul.y;
                Instantiate(tierraBase, posTierraCulFinal, Quaternion.identity);
            }
        }
    }

    //Raycast para comprobar si delante del jugador hay tierra
    private bool ComprobarRiegoPosible()
    {
        bool posible = false;
        if (Global.ItemSelec != null && Global.ItemSelec is IHerramientaBase && (Global.ItemSelec as IHerramientaBase).TipoHerramientaU == TipoHerramienta.Regadera)
        {
            RaycastHit hit;
            if (Physics.Raycast(ObjetivoCultivo.transform.position, ObjetivoCultivo.transform.forward, out hit))
            {
                if (hit.transform.gameObject.CompareTag("TierraCultivos"))
                {
                    posible = true;
                    ultTierra = hit.transform.gameObject;
                }
            }
        }
        return posible;
    }

    private bool ComprobarPlantadoPosible()
    {
        bool posible = false;
        if (Global.ItemSelec != null && Global.ItemSelec is Semilla)
        {
            RaycastHit hit;
            if (Physics.Raycast(ObjetivoCultivo.transform.position, ObjetivoCultivo.transform.forward, out hit))
            {
                if (hit.transform.gameObject.CompareTag("TierraCultivos"))
                {
                    if (!hit.transform.gameObject.GetComponent<Tierra>().Plantada)
                    {
                        posible = true;
                        ultTierra = hit.transform.gameObject;
                        ultSemilla = (Global.ItemSelec as Semilla);
                    }
                }
            }
        }
        return posible;
    }

    private bool ComprobarRecogidaPosible()
    {
        bool posible = false;
        RaycastHit hit;
        if (Physics.Raycast(ObjetivoCultivo.transform.position, ObjetivoCultivo.transform.forward, out hit))
        {
            if (hit.transform.gameObject.CompareTag("TierraCultivos"))
            {
                if (hit.transform.gameObject.GetComponent<Tierra>().PlantaLista)
                {
                    posible = true;
                    ultTierra = hit.transform.gameObject;
                }
            }
        }
        return posible;
    }

    //Raycast para comprobar si delante del jugador hay terreno cultivable
    private bool ComprobarAradoPosible()
    {
        bool posible = false;
        if (Global.ItemSelec != null && Global.ItemSelec is IHerramientaBase && (Global.ItemSelec as IHerramientaBase).TipoHerramientaU == TipoHerramienta.Azada)
        {
            RaycastHit hit;
            if (Physics.Raycast(ObjetivoCultivo.transform.position, ObjetivoCultivo.transform.forward, out hit))
            {
                if (hit.transform.gameObject.CompareTag("TierraEdificable"))
                {
                    posible = true;
                    posicionTierraCul = hit.point;
                }
            }
        }
        return posible;
    }
}

public enum TiposCultivos
{
    NORMAL,
    PALO,
    ARBOL
}
