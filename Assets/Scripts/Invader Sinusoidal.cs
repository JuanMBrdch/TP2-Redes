using System;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class InvaderSinusoidal : NetworkBehaviour
{
    public float amplitudX = 2f;
    public float velocidadX = 2f;
    public float velocidadY = 1f;
    public GameObject tpPlace;

    private float tiempo = 0f;
    private Vector3 startPosition;  
    private bool correcto = true;
    [SerializeField] private float tiempoSeguir = 0f;
    [SerializeField] private float tiempoLimite = 0.01f;
    private ulong _id;

    private float hitCount;
    
    void Start()
    {
        startPosition = transform.position;
        hitCount = 35f;
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
            if (tpPlace != null)
            {
                transform.position = tpPlace.transform.position;
            }
            tiempo = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider other)
    {
        if (other.CompareTag("Laser"))
        {
            
        }
    }

    public void TakeDamage()
    {
        hitCount--;
        if (hitCount <= 0)
        {
            var netObj = GetComponent<NetworkObject>();
            netObj.Despawn();
        }
    }
    
    public void ComeBack()
    {
        if (tpPlace != null)
        {
            correcto = false;
            startPosition = tpPlace.transform.position;
            transform.position = startPosition;
        }
       
    }
}
