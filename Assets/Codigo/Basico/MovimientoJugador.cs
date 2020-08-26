using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoJugador : MonoBehaviour
{
    public float multiplicadorGravedad;
    public float velocidad;
    public float velocidadCorrer;
    public float velocidadSalto;
    public float divisorVelSalto;

    private CharacterController controladorJugador;
    private Vector3 movimiento = new Vector3();
    // Start is called before the first frame update
    void Start()
    {
        controladorJugador = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!Global.MovBloq && !Global.InventarioAbierto)
        {
            EjecutarMovimiento();
        }
    }

    private void EjecutarMovimiento()
    {
        Global.JugadorSuelo = controladorJugador.isGrounded;

        float movY = 0;
        if (!Global.JugadorSuelo)
        {
            movY = movimiento.y;
        }

        movimiento = new Vector3();
        if (Input.GetKey(KeyCode.W)) //TODO: remaping de teclas
        {
            movimiento += transform.forward;
        }
        if (Input.GetKey(KeyCode.A)) //TODO: remaping de teclas
        {
            movimiento += -transform.right;
        }
        if (Input.GetKey(KeyCode.S)) //TODO: remaping de teclas
        {
            movimiento += -transform.forward;
        }
        if (Input.GetKey(KeyCode.D)) //TODO: remaping de teclas
        {
            movimiento += transform.right;
        }

        if (Global.JugadorSuelo)
        {
            movimiento = movimiento.normalized * (Input.GetKey(KeyCode.LeftShift) ? velocidadCorrer : velocidad); //TODO: remaping de teclas
        }
        else
        {
            movimiento = movimiento.normalized * (Input.GetKey(KeyCode.LeftShift) ? (velocidadCorrer / divisorVelSalto) : (velocidad / divisorVelSalto)); //TODO: remaping de teclas
            movimiento.y = movY;
        }

        if (Input.GetKey(KeyCode.Space) && Global.JugadorSuelo) //TODO: remaping de teclas
        {
            movimiento.y = velocidadSalto;
        }

        movimiento += Physics.gravity * multiplicadorGravedad * Time.deltaTime;

        controladorJugador.Move(movimiento * Time.deltaTime);
    }
}
