using UnityEngine;
using Unity.Netcode;
using UnityEngine.Serialization;

public class InvaderSinusoidal : NetworkBehaviour
{
    public float amplitudeX = 2f;
    public float speedX = 2f;
    public float speedY = 1f;
    public GameObject tpPlace;

    private float _time = 0f;
    private Vector3 _startPosition;  
    private bool _correct = true;
    [SerializeField] private float continueTime = 0f;
    [SerializeField] private float timeLimit = 0.01f;
    private ulong _id;

    private float _hitCount;

    private void Start()
    {
        _startPosition = transform.position;
        _hitCount = 35f;
    }

    private void Update()
    {
        if (_correct)
        {
            _time += Time.deltaTime;

            float movX = Mathf.Sin(_time * speedX) * amplitudeX;
            float movY = -_time * speedY;

            Vector3 newPos = _startPosition + new Vector3(movX, movY, 0f);

            transform.position = newPos;
            continueTime = 0;
        }

        if (!_correct)
        {
            continueTime += Time.deltaTime;
        }

        if (continueTime >= timeLimit)
        {
            _correct = true;
            if (tpPlace != null)
            {
                transform.position = tpPlace.transform.position;
            }
            _time = 0f;
        }
    }
    
    public void TakeDamage()
    {
        if (IsServer)
        {
            _hitCount--;
            if (_hitCount <= 0)
            {
                var netObj = GetComponent<NetworkObject>();
                netObj.Despawn();
            }
        }
    }
    
    public void ComeBack()
    {
        if (tpPlace != null)
        {
            _correct = false;
            _startPosition = tpPlace.transform.position;
            transform.position = _startPosition;
        }
    }
}
