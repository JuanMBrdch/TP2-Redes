using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class LimitDown : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var enemyModel = other.GetComponent<InvaderSinusoidal>();
        if (enemyModel != null)
        {
            enemyModel.ComeBack();
            Debug.Log("toma");
        }
        else
        {
            Debug.Log("No se encontró el componente InvaderSinusoidal");
        }
    }
}
