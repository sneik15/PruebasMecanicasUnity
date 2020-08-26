using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControladorDrag : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool siendoMovido;
    private GestionInventario inventarioAc;
    private int slotInicial;
    private int slotInicialCofre;
    private bool esSlotIniCofre;
    private GameObject copia;

    private void Start()
    {
        inventarioAc = GameObject.FindGameObjectWithTag("Player").GetComponent<GestionInventario>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (inventarioAc.CofreAbierto)
        {
            GameObject.Find("CofresUI").GetComponent<ScrollRect>().enabled = false;
        }
        slotInicial = ObtenerPosicionSlotAc(out esSlotIniCofre);
        if (esSlotIniCofre)
        {
            slotInicialCofre = slotInicial;
        }
        siendoMovido = true;
        Global.MoviendoItems = true;
        copia = Instantiate(gameObject, transform.position, transform.rotation, GameObject.Find("AyudaDrag").transform);
        gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 0);
        transform.parent.GetChild(1).gameObject.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (siendoMovido)
        {
            bool slotDestinoCofre;
            int slotDestino = ObtenerPosicionSlotAc(out slotDestinoCofre);
            Destroy(copia);
            gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            if (slotDestino >= 0)
            {
                if (!esSlotIniCofre && !slotDestinoCofre)
                {
                    inventarioAc.IntercambiarSlotsInvtoInv(slotInicial, slotDestino);
                }
                else if(esSlotIniCofre && !slotDestinoCofre)
                {
                    inventarioAc.IntercambiarSlotsCofrtoInv(slotInicialCofre, slotDestino);
                }
                else if (!esSlotIniCofre && slotDestinoCofre)
                {
                    inventarioAc.IntercambiarSlotsInvtoCofr(slotInicial, slotDestino);
                }
                else if (esSlotIniCofre && slotDestinoCofre)
                {
                    inventarioAc.IntercambiarSlotsCofrtoCofr(slotInicialCofre, slotDestino);
                }
            }
            siendoMovido = false;
            Global.MoviendoItems = false;
            if (inventarioAc.CofreAbierto)
            {
                GameObject.Find("CofresUI").GetComponent<ScrollRect>().enabled = true;
            }
        }
        
    }


    private void Update()
    {
        if (siendoMovido)
        {
            copia.transform.position = Input.mousePosition;
        }
    }

    private int ObtenerPosicionSlotAc(out bool slotCofre)
    {
        int slotAc = -1;
        bool encontrado = false;
        slotCofre = false;
        while (slotAc < 8 && !encontrado)
        {
            slotAc++;
            if (Input.mousePosition.x > (inventarioAc.SlotsBarraRapida[slotAc].transform.position.x - 33) && Input.mousePosition.x < (inventarioAc.SlotsBarraRapida[slotAc].transform.position.x + 33) &&  
               Input.mousePosition.y > (inventarioAc.SlotsBarraRapida[slotAc].transform.position.y - 33) && Input.mousePosition.y < (inventarioAc.SlotsBarraRapida[slotAc].transform.position.y + 33))
            {
                encontrado = true;
            }
        }

        while ((slotAc - 9) < (inventarioAc.CapacidadInventarioActual - 1) && !encontrado)
        {
            slotAc++;
            if (Input.mousePosition.x > (inventarioAc.SlotsInventario[(slotAc - 9)].transform.position.x - 33) && Input.mousePosition.x < (inventarioAc.SlotsInventario[(slotAc - 9)].transform.position.x + 33) &&
               Input.mousePosition.y > (inventarioAc.SlotsInventario[(slotAc - 9)].transform.position.y - 33) && Input.mousePosition.y < (inventarioAc.SlotsInventario[(slotAc - 9)].transform.position.y + 33))
            {
                encontrado = true;
            }
        }

        if (!encontrado && inventarioAc.CofreAbierto)
        {
            slotAc = -1;
            while (slotAc < (inventarioAc.CofreAc.CapacidadInventario - 1) && !encontrado)
            {
                slotAc++;
                if (Input.mousePosition.x > (inventarioAc.SlotsCofre[slotAc].transform.position.x - 33) && Input.mousePosition.x < (inventarioAc.SlotsCofre[slotAc].transform.position.x + 33) &&
                   Input.mousePosition.y > (inventarioAc.SlotsCofre[slotAc].transform.position.y - 33) && Input.mousePosition.y < (inventarioAc.SlotsCofre[slotAc].transform.position.y + 33))
                {
                    encontrado = true;
                    slotCofre = true;
                }
            }
        }

        if (encontrado)
        {
            return slotAc;
        }
        else
        {
            return -1;
        }
    }
}
