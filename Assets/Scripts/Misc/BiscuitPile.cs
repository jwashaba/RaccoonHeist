using UnityEngine;

public class BiscuitPile : MonoBehaviour
{
    public string playerTag = "Player";

    public ParticleSystem ps1;
    public ParticleSystem ps2;
    public ParticleSystem ps3;
    public ParticleSystem ps4;
    public ParticleSystem ps5;

    public Transform sp;
    
    private Rigidbody2D _playerBody;
    private bool _playerInArea;
    private bool _playerIsMoving;

    [Header("Particle Timing")]
    public float particleSpawnDuration = 0.1f; // seconds between spawns
    private float _particleCountTime;

    void Start()
    {
        ps1.Stop();
        ps2.Stop();
        ps3.Stop();
        ps4.Stop();
        ps5.Stop();

        _playerBody = null;
        _playerInArea = false;
        _playerIsMoving = false;

        _particleCountTime = 0f;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag(playerTag))
        {
            _playerBody = collider.GetComponent<Rigidbody2D>();
            _playerInArea = true;
            _playerIsMoving = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag(playerTag))
        {
            _playerBody = null;
            _playerInArea = false;
            _playerIsMoving = false;
        }
    }

    void Update()
    {
        if (_playerInArea && _playerBody != null)
        {
            if (_playerBody.linearVelocity.magnitude > 0.1f)
            {
                // sp.position = _playerBody.transform.position;
                
                if (_particleCountTime < particleSpawnDuration)
                {
                    _particleCountTime += Time.deltaTime;
                }
                else
                {
                    // reset timer
                    _particleCountTime = 0f;

                    // pick random ParticleSystem
                    int index = Random.Range(0, 5);
                    switch (index)
                    {
                        case 0: ps1.Emit(1); break;
                        case 1: ps2.Emit(1); break;
                        case 2: ps3.Emit(1); break;
                        case 3: ps4.Emit(1); break;
                        case 4: ps5.Emit(1); break;
                    }
                }
            }
        }
    }
}
