using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Serialization;

public class InvaderSinusoidal : NetworkBehaviour
{
    public float amplitudX = 2f;
    public float velocidadX = 2f;
    public float velocidadY = 1f;
    public GameObject tpPlace;

    private float tiempo = 0f;
    private Vector3 startPosition;  // Almacena la posición inicial antes de iniciar el movimiento sinusoidal
    private bool correcto = true;
    private float tiempoSeguir = 0f;
    private float tiempoLimite = 10f;
    private ulong _id;

    void Start()
    {
        // Almacena la posición inicial antes de iniciar el movimiento sinusoidal
        startPosition = transform.position;
        
    }

    void Update()
    {
        if (correcto)
        {
            tiempo += Time.deltaTime;

            float desplazamientoX = Mathf.Sin(tiempo * velocidadX) * amplitudX;
            float desplazamientoY = -tiempo * velocidadY;

            Vector3 nuevaPosicion = startPosition + new Vector3(desplazamientoX, desplazamientoY, 0f);

            transform.position = nuevaPosicion;
            tiempoSeguir = 0;
        }

        if (!correcto)
        {
            tiempoSeguir += Time.deltaTime;
        }

        if (tiempoSeguir >= tiempoLimite)
        {
            correcto = true;

            // Mueve el objeto a la posición de tpPlace almacenada
            if (tpPlace != null)
            {
                transform.position = tpPlace.transform.position;
            }
            else
            {
                Debug.LogWarning("tpPlace no está asignado en el script SnakeInvader");
            }

            tiempo = 0f; // Reinicia el tiempo cuando vuelves al movimiento sinusoidal
        }
    }


    public void ComeBack()
    {
        if (tpPlace != null)
        {
            correcto = false;
            startPosition = tpPlace.transform.position;
            transform.position = startPosition;
            Debug.Log("LLAMA");
        }
       
    }
}
