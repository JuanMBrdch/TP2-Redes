using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Serialization;

public class InvaderSinusoidal : NetworkBehaviour
{
    public float amplitude = 1f;
    public float frequency;
    public float speed;
    public float speedY;

    private float time = 0f;
    
    [SerializeField] private Vector3 startPos;

    void Start()
    {
        if (!MasterManager.Singleton.IsServer)
        {
            this.enabled = false;
        }
        else
        {
            startPos = transform.position;
        }
    }

    void Update()
    {
        time += Time.deltaTime * speed;
        var posX = amplitude * Mathf.Sin(time * frequency);
        transform.position = new Vector3(posX, speedY, 0f);
        // if (transform.position.y <= Camera.main.transform.localScale.y/2)
        // {
        //     transform.position.y += Camera.main.transform.localScale.y;
        // }

    }
}
