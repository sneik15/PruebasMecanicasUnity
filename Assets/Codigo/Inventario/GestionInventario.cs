using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestionInventario : MonoBehaviour
{
    private SlotInventario[] inventario;
    public int CapacidadInventarioActual;
    public int CapacidadInventarioMax;
    private int slotAc;

    private List<GameObject> ultObjectoDetec;
    private Transform posItemJug;
    private bool agregando;

    private GameObject PanelMensajesUI;

    //InventarioUI
    private GameObject inventarioUI;
    public GameObject[] SlotsBarraRapida { get; set; }
    public int numSlotsBarraRapida;
    public GameObject[] SlotsInventario { get; set; }
    public int numSlotsInventario;
    public Sprite InvSelec;
    public Sprite InvNormal;

    //CofreUI
    private GameObject cofreUI;
    public GameObject[] SlotsCofre { get; set; }
    public int numSlotsCofre;
    public Cofre CofreAc { get; set; }
    public bool CofreAbierto { get; set; }

    private List<GameObject> ultCofreDetec;

    void Start()
    {
        CargaGameObjects();
        ultObjectoDetec = new List<GameObject>();
        ultCofreDetec = new List<GameObject>();
        inventario = new SlotInventario[CapacidadInventarioActual];
        DesactivarSlotsNoValidos();
        PanelMensajesUI.SetActive(false);
        inventarioUI.SetActive(false);
        cofreUI.SetActive(false);
        slotAc = 0;
        Global.InventarioAbierto = false;
        EliminarDragsInventario();
        EliminarDragsCofre();
        agregando = false;
        CofreAbierto = false;
    }

    void Update()
    {
        CogerYCambiarObjetos();
        SoltarObjeto();
        AccionesInventario();
        ActualizarUI();    
    }

    #region Inventario
    public void DesactivarSlotsNoValidos()
    {
        for (int i = 0; i < SlotsInventario.Length; i++)
        {
            if(i >= (CapacidadInventarioActual - 9))
            {
                SlotsInventario[i].SetActive(false);
            }
        }
    }

    #region Intercambio de slots
    public void IntercambiarSlotsInvtoInv(int slot1, int slot2)
    {
        if(slotAc == slot1 || slotAc == slot2)
        {
            if (Global.ItemSelec != null)
            {
                Global.ItemSelec.ObjetoRelacionado.SetActive(false);
            }
        }
        SlotInventario copSlot1 = inventario[slot1];
        SlotInventario copSlot2 = inventario[slot2];

        inventario[slot1] = copSlot2;
        inventario[slot2] = copSlot1;
        EliminarDragsInventario();
        AgregarDragsInventario();
    }
    public void IntercambiarSlotsInvtoCofr(int slot1, int slot2)
    {
        if (slotAc == slot1)
        {
            if (Global.ItemSelec != null)
            {
                Global.ItemSelec.ObjetoRelacionado.SetActive(false);
            }
        }
        SlotInventario copSlot1 = inventario[slot1];
        SlotInventario copSlot2 = CofreAc.Inventario[slot2];

        inventario[slot1] = copSlot2;
        CofreAc.Inventario[slot2] = copSlot1;
        EliminarDragsInventario();
        AgregarDragsInventario();
        EliminarDragsCofre();
        AgregarDragsCofre();
    }
    public void IntercambiarSlotsCofrtoInv(int slot1, int slot2)
    {
        if (slotAc == slot2)
        {
            if (Global.ItemSelec != null)
            {
                Global.ItemSelec.ObjetoRelacionado.SetActive(false);
            }
        }
        SlotInventario copSlot1 = CofreAc.Inventario[slot1];
        SlotInventario copSlot2 = inventario[slot2];

        CofreAc.Inventario[slot1] = copSlot2;
        inventario[slot2] = copSlot1;
        EliminarDragsInventario();
        AgregarDragsInventario();
        EliminarDragsCofre();
        AgregarDragsCofre();
    }

    public void IntercambiarSlotsCofrtoCofr(int slot1, int slot2)
    {
        SlotInventario copSlot1 = CofreAc.Inventario[slot1];
        SlotInventario copSlot2 = CofreAc.Inventario[slot2];

        CofreAc.Inventario[slot1] = copSlot2;
        CofreAc.Inventario[slot2] = copSlot1;
        EliminarDragsCofre();
        AgregarDragsCofre();
    }
    #endregion

    public bool AgregarObjeto(IItemBase item) 
    {
        bool agregado = false;
        for (int i = 0; i < inventario.Length; i++)
        {
            if (inventario[i] != null)
            {
                if (inventario[i].AgregarItem(item) > 0)
                {
                    agregado = true;
                    break;
                }
            }
        }

        if (!agregado)
        {
            for (int i = 0; i < inventario.Length; i++)
            {
                if (inventario[i] == null)
                {
                    SlotInventario nuevo = new SlotInventario();
                    nuevo.AgregarItem(item);
                    inventario[i] = nuevo;
                    agregado = true;
                    break;
                }
            }
        }

        if (agregado)
        {
            item.Autodestruir = false;
            item.ObjetoRelacionado.transform.SetParent(posItemJug, false);
            Destroy(item.ObjetoRelacionado.GetComponent<Rigidbody>()); //TODO mirar si todos van a tener Rigidbody
            item.ObjetoRelacionado.transform.localPosition = Vector3.zero;
            item.ObjetoRelacionado.transform.localRotation = Quaternion.identity;
            item.ObjetoRelacionado.SetActive(false);
        }
        return agregado;
    }

    private void CogerYCambiarObjetos()
    {
        if(ultObjectoDetec.Count > 0 && Input.GetKeyDown(KeyCode.F) && !Global.InventarioAbierto && !agregando) //TODO: remaping de teclas
        {
            agregando = true;
            bool agregado = false;
            for (int i = 0; i < inventario.Length; i++)
            {
                if(inventario[i] != null)
                {
                    if (inventario[i].AgregarItem(ultObjectoDetec[0].GetComponent<IItemBase>()) > 0)
                    {
                        agregado = true;
                        break;
                    }
                }
            }

            if (!agregado)
            {
                for (int i = 0; i < inventario.Length; i++)
                {
                    if (inventario[i] == null)
                    {
                        SlotInventario nuevo = new SlotInventario();
                        nuevo.AgregarItem(ultObjectoDetec[0].GetComponent<IItemBase>());
                        inventario[i] = nuevo;
                        agregado = true;
                        break;
                    }
                }
            }

            if (agregado)
            {
                ultObjectoDetec[0].GetComponent<IItemBase>().Autodestruir = false;
                ultObjectoDetec[0].transform.SetParent(posItemJug, false);
                Destroy(ultObjectoDetec[0].GetComponent<Rigidbody>()); //TODO mirar si todos van a tener Rigidbody
                ultObjectoDetec[0].transform.localPosition = Vector3.zero;
                ultObjectoDetec[0].transform.localRotation = Quaternion.identity;
                ultObjectoDetec[0].SetActive(false);
                ultObjectoDetec.RemoveAt(0);
                if (ultObjectoDetec.Count < 1)
                {
                    if(ultCofreDetec.Count > 0)
                    {
                        PanelMensajesUI.transform.GetChild(0).GetComponent<Text>().text = GestorTraducciones.TraducirTexto("{AbrCofre}");
                    }
                    else
                    {
                        PanelMensajesUI.SetActive(false);
                    }
                }
            }
            agregando = false;
        }

        SeleccionSlot();

        Global.ItemSelec = inventario[slotAc]?.ObtenerItem();
        if(Global.ItemSelec != null && !Global.ItemSelec.ObjetoRelacionado.activeSelf)
        {
            Global.ItemSelec.ObjetoRelacionado.transform.localPosition = Vector3.zero;
            Global.ItemSelec.ObjetoRelacionado.SetActive(true);
        }
    }

    void SeleccionSlot()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            slotAc++;
            if (slotAc > 8)
            {
                slotAc = 0;
            }
            if (Global.ItemSelec != null)
            {
                Global.ItemSelec.ObjetoRelacionado.SetActive(false);
            }
        }

        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            slotAc--;
            if (slotAc < 0)
            {
                slotAc = 8;
            }
            if (Global.ItemSelec != null)
            {
                Global.ItemSelec.ObjetoRelacionado.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            slotAc = 0;
            if (Global.ItemSelec != null)
            {
                Global.ItemSelec.ObjetoRelacionado.SetActive(false);
            }
        } 
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            slotAc = 1;
            if (Global.ItemSelec != null)
            {
                Global.ItemSelec.ObjetoRelacionado.SetActive(false);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            slotAc = 2;
            if (Global.ItemSelec != null)
            {
                Global.ItemSelec.ObjetoRelacionado.SetActive(false);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            slotAc = 3;
            if (Global.ItemSelec != null)
            {
                Global.ItemSelec.ObjetoRelacionado.SetActive(false);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            slotAc = 4;
            if (Global.ItemSelec != null)
            {
                Global.ItemSelec.ObjetoRelacionado.SetActive(false);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            slotAc = 5;
            if (Global.ItemSelec != null)
            {
                Global.ItemSelec.ObjetoRelacionado.SetActive(false);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            slotAc = 6;
            if (Global.ItemSelec != null)
            {
                Global.ItemSelec.ObjetoRelacionado.SetActive(false);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            slotAc = 7;
            if (Global.ItemSelec != null)
            {
                Global.ItemSelec.ObjetoRelacionado.SetActive(false);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            slotAc = 8;
            if (Global.ItemSelec != null)
            {
                Global.ItemSelec.ObjetoRelacionado.SetActive(false);
            }
        }
    }

    void SoltarObjeto()
    {
        if (Global.ItemSelec != null && Input.GetKeyDown(KeyCode.Q)) //TODO: remaping de teclas
        {
            Global.ItemSelec.ObjetoRelacionado.tag = "Item";
            Global.ItemSelec.ObjetoRelacionado.SetActive(true);
            Global.ItemSelec.ObjetoRelacionado.transform.SetParent(null);
            Global.ItemSelec.ObjetoRelacionado.AddComponent<Rigidbody>();
            Global.ItemSelec.ObjetoRelacionado.GetComponent<Rigidbody>().AddForce(transform.forward * 200);
            Global.ItemSelec.Autodestruir = true;
            if (inventario[slotAc].QuitarItem() < 1)
            {
                inventario[slotAc] = null;
            }
            Global.ItemSelec = null;
        }
    }

    public void EliminarObjetoSeleccionado()
    {
        if (Global.ItemSelec != null)
        {
            Destroy(Global.ItemSelec.ObjetoRelacionado, 1);
            if (inventario[slotAc].QuitarItem() < 1)
            {
                inventario[slotAc] = null;
            }
            Global.ItemSelec = null;
        }
    }

    private void AccionesInventario()
    {
        //Abrir y cerrar inventario
        if (Input.GetKeyDown(KeyCode.Tab)) //TODO: remaping de teclas
        {
            if (!Global.InventarioAbierto)
            {
                Global.InventarioAbierto = true;
                inventarioUI.SetActive(Global.InventarioAbierto);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                AgregarDragsInventario();
            }
            else
            {
                Global.InventarioAbierto = false;
                inventarioUI.SetActive(Global.InventarioAbierto);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                EliminarDragsInventario();
                if (CofreAbierto)
                {
                    CofreAbierto = false;
                    cofreUI.SetActive(CofreAbierto);
                    CofreAc = null;
                    EliminarDragsCofre();
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && Global.InventarioAbierto)
        {
            Global.InventarioAbierto = false;
            inventarioUI.SetActive(Global.InventarioAbierto);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            EliminarDragsInventario();
            if (CofreAbierto)
            {
                CofreAbierto = false;
                cofreUI.SetActive(CofreAbierto);
                CofreAc = null;
                EliminarDragsCofre();
            }
        }

        //Cofres
        if (Input.GetKeyDown(KeyCode.F) && ultCofreDetec.Count > 0 && ultObjectoDetec.Count < 1) //TODO: remaping de teclas
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Global.InventarioAbierto = true;
            inventarioUI.SetActive(Global.InventarioAbierto);
            CofreAbierto = true;
            cofreUI.SetActive(CofreAbierto);
            CofreAc = ultCofreDetec[0].GetComponent<Cofre>();
            AgregarDragsInventario();
            AgregarDragsCofre();
            DesactivarSlotsNoValidosCofres();
        }
    }
    
    private void ActualizarUI()
    {
        if (!Global.MoviendoItems)
        {
            //Dibujado de items en el inventario
            for (int i = 0; i < inventario.Length; i++)
            {
                if (i < 9)
                {
                    if (inventario[i] != null)
                    {
                        SlotsBarraRapida[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                        SlotsBarraRapida[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().sprite = inventario[i].ObtenerItem().Icono;
                        if (inventario[i].ObtenetNumeroItems() > 1)
                        {
                            SlotsBarraRapida[i].transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                            SlotsBarraRapida[i].transform.GetChild(0).GetChild(1).gameObject.GetComponent<Text>().text = inventario[i].ObtenetNumeroItems().ToString();
                        }
                        else
                        {
                            SlotsBarraRapida[i].transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        SlotsBarraRapida[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 0);
                        SlotsBarraRapida[i].transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                    }

                    if (i == slotAc)
                    {
                        SlotsBarraRapida[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = InvSelec;
                    }
                    else
                    {
                        SlotsBarraRapida[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = InvNormal;
                    }
                }
                else
                {
                    if (Global.InventarioAbierto)
                    {
                        int u = i - 9;
                        if (inventario[i] != null)
                        {
                            SlotsInventario[u].transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                            SlotsInventario[u].transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().sprite = inventario[i].ObtenerItem().Icono;
                            if (inventario[i].ObtenetNumeroItems() > 1)
                            {
                                SlotsInventario[u].transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                                SlotsInventario[u].transform.GetChild(0).GetChild(1).gameObject.GetComponent<Text>().text = inventario[i].ObtenetNumeroItems().ToString();
                            }
                            else
                            {
                                SlotsInventario[u].transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                            }
                        }
                        else
                        {
                            SlotsInventario[u].transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 0);
                            SlotsInventario[u].transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                        }
                    }
                }
            }
            //Dibujado de items en el cofre
            if (CofreAbierto)
            {
                for (int i = 0; i < CofreAc.CapacidadInventario; i++)
                {
                    if (CofreAc.Inventario[i] != null)
                    {
                        SlotsCofre[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                        SlotsCofre[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().sprite = CofreAc.Inventario[i].ObtenerItem().Icono;
                        if (CofreAc.Inventario[i].ObtenetNumeroItems() > 1)
                        {
                            SlotsCofre[i].transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                            SlotsCofre[i].transform.GetChild(0).GetChild(1).gameObject.GetComponent<Text>().text = CofreAc.Inventario[i].ObtenetNumeroItems().ToString();
                        }
                        else
                        {
                            SlotsCofre[i].transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        SlotsCofre[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 0);
                        SlotsCofre[i].transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    private void AgregarDragsInventario()
    {
        for (int i = 0; i < inventario.Length; i++)
        {
            if (i < 9)
            {
                if (inventario[i] != null)
                {
                    SlotsBarraRapida[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<ControladorDrag>().enabled = true;
                }
            }
            else
            {
                int u = i - 9;
                if (inventario[i] != null)
                {
                    SlotsInventario[u].transform.GetChild(0).GetChild(0).gameObject.GetComponent<ControladorDrag>().enabled = true;
                }
            }
        }
    }

    private void EliminarDragsInventario()
    {
        for (int i = 0; i < inventario.Length; i++)
        {
            if (i < 9)
            {
                SlotsBarraRapida[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<ControladorDrag>().enabled = false;
            }
            else
            {
                int u = i - 9;
                SlotsInventario[u].transform.GetChild(0).GetChild(0).gameObject.GetComponent<ControladorDrag>().enabled = false;
            }
        }
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item" && (Global.ItemSelec == null || other.transform.parent.gameObject.GetInstanceID() != Global.ItemSelec.ObjetoRelacionado.GetInstanceID()))
        {
            if (!ultObjectoDetec.Exists(x => x.GetInstanceID() == other.transform.parent.gameObject.GetInstanceID()))
            {
                ultObjectoDetec.Add(other.transform.parent.gameObject);
                PanelMensajesUI.transform.GetChild(0).GetComponent<Text>().text = GestorTraducciones.TraducirTexto("{RecogerObj}");
                PanelMensajesUI.SetActive(true);
            }
        }
        //Cofres
        if (other.gameObject.tag.StartsWith("Cofre"))
        {
            ultCofreDetec.Add(other.gameObject);
            if (ultObjectoDetec.Count < 1)
            {
                PanelMensajesUI.transform.GetChild(0).GetComponent<Text>().text = GestorTraducciones.TraducirTexto("{AbrCofre}"); 
                PanelMensajesUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Item")
        {
            for (int i = 0; i < ultObjectoDetec.Count; i++)
            {
                if (other.transform.parent.gameObject.GetInstanceID() == ultObjectoDetec[i].GetInstanceID())
                {
                    ultObjectoDetec.Remove(other.transform.parent.gameObject);
                    if (ultObjectoDetec.Count < 1)
                    {
                        PanelMensajesUI.SetActive(false);
                    }
                }
            }
        }
        //Cofres
        if (other.gameObject.tag.StartsWith("Cofre"))
        {
            ultCofreDetec.Remove(other.gameObject);
            if (ultCofreDetec.Count < 1)
            {
                PanelMensajesUI.SetActive(false);
            }
        }
    }

    #region Cofres
    private void AgregarDragsCofre()
    {
        for (int i = 0; i < CofreAc.Inventario.Length; i++)
        {
            if(CofreAc.Inventario[i] != null)
            {
                SlotsCofre[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<ControladorDrag>().enabled = true;
            }
        }
    }

    private void EliminarDragsCofre()
    {
        for (int i = 0; i < SlotsCofre.Length; i++)
        {
            SlotsCofre[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<ControladorDrag>().enabled = false;
        }
    }

    private void DesactivarSlotsNoValidosCofres()
    {
        for (int i = 0; i < 99; i++)
        {
            if (i >= CofreAc.CapacidadInventario)
            {
                SlotsCofre[i].SetActive(false);
            }
            else
            {
                SlotsCofre[i].SetActive(true);
            }
        }
    }
    #endregion

    private void CargaGameObjects()
    {
        PanelMensajesUI = GameObject.Find("PanelMensajes");
        inventarioUI = GameObject.Find("PanelInventario");
        cofreUI = GameObject.Find("CofresUI");
        posItemJug = GameObject.Find("PosItem").transform;

        List<GameObject> slotsBarraRapida = new List<GameObject>();
        for (int i = 1; i <= numSlotsBarraRapida; i++)
        {
            slotsBarraRapida.Add(GameObject.Find("SlotBarra" + i));
        }
        SlotsBarraRapida = slotsBarraRapida.ToArray();

        List<GameObject> slotsInventario = new List<GameObject>();
        for (int i = 1; i <= numSlotsInventario; i++)
        {
            slotsInventario.Add(GameObject.Find("SlotPanel" + i));
        }
        SlotsInventario = slotsInventario.ToArray();

        //Cofres
        List<GameObject> slotsCofre = new List<GameObject>();
        for (int i = 1; i <= numSlotsCofre; i++)
        {
            slotsCofre.Add(GameObject.Find("SlotCofre" + i));
        }
        SlotsCofre = slotsCofre.ToArray();
    }
}
