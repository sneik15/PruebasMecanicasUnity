using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoCamara : MonoBehaviour
{
    //Movimiento camara
    public float velocidadRotacion = 1;
    public Transform Jugador;
    public Transform Objetivo;
    private Quaternion CopObjetivoRot;
    private float ratonX;
    private float ratonY;
    private float copRatonX;
    private float copRatonY;
    private bool movSinPJugador;

    //Colisiones
    public float velocidadZoomCam = 10f;
    private Vector3 dirJugador;
    private float distanciaDeseada;
    public float DistanciaCamMinColision = 0.75f;
    private float distancia;

    //ZOOM
    public float distanciaMovCam = 1f;
    public float DistanciaCamMin = 3.5f;
    public float DistanciaCamMax = 6.5f;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        movSinPJugador = false;
        dirJugador = transform.localPosition.normalized;
        distancia = transform.localPosition.magnitude;
        distanciaDeseada = transform.localPosition.magnitude;
    }

    void Update()
    {
        if (!Global.InventarioAbierto)
        {
            ControlCamara();
            ControlDistanciaCamara();
        }
        ControlColision();
    }

    void ControlCamara()
    {
        if (!Global.MovBloq)
        {
            ratonX += Input.GetAxis("Mouse X") * velocidadRotacion;
        }
        ratonY -= Input.GetAxis("Mouse Y") * velocidadRotacion;
        ratonY = Mathf.Clamp(ratonY, -35, 60);

        transform.LookAt(Objetivo);

        if (Input.GetKey(KeyCode.RightShift)) //TODO: remaping de teclas
        {
            if (!movSinPJugador)
            {
                movSinPJugador = true;
                CopObjetivoRot = new Quaternion(Objetivo.rotation.x, Objetivo.rotation.y, Objetivo.rotation.z, Objetivo.rotation.w);
                copRatonX = ratonX;
                copRatonY = ratonY;
            }
            Objetivo.rotation = Quaternion.Euler(ratonY, ratonX, 0);
        }
        else
        {
            if (movSinPJugador)
            {
                movSinPJugador = false;
                Objetivo.rotation = CopObjetivoRot;
                ratonX = copRatonX;
                ratonY = copRatonY;
            }
            Objetivo.rotation = Quaternion.Euler(ratonY, ratonX, 0);
            Jugador.rotation = Quaternion.Euler(0, ratonX, 0);
        }
    }

    void ControlDistanciaCamara()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus) && (Vector3.Distance(transform.position, Objetivo.position) + distanciaMovCam) > DistanciaCamMin) //TODO: remaping de teclas
        {
            transform.Translate(Vector3.forward * distanciaMovCam);
            distanciaDeseada = transform.localPosition.magnitude;
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus) && (Vector3.Distance(transform.position, Objetivo.position) + distanciaMovCam) < DistanciaCamMax) //TODO: remaping de teclas
        { 
            transform.Translate(Vector3.back * distanciaMovCam);
            distanciaDeseada = transform.localPosition.magnitude;
        }
    }

    void ControlColision()
    {
        Vector3 posicionObjCam = transform.parent.TransformPoint(dirJugador * distanciaDeseada);
        RaycastHit hit;
        if (Physics.Linecast(transform.parent.position, posicionObjCam, out hit))
        {
            if (!(hit.collider.isTrigger && hit.transform.gameObject.CompareTag("Player")))
            {
                distancia = Mathf.Clamp(hit.distance * 0.8f, DistanciaCamMinColision, distanciaDeseada);
            }
        }
        else
        {
            distancia = distanciaDeseada;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, dirJugador * distancia, Time.deltaTime * velocidadZoomCam);
    }
}
