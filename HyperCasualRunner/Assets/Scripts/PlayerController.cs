using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController CurrentPlayer;


    [Header("Gameobjects")]
    [SerializeField] private GameObject ridingCylinderPrefab;
    [SerializeField] private GameObject bridgePiecePrefab;
    [SerializeField] internal List<RidingCylinder> cylinders;
    [Header("Player Movement Settings")]
    public float limitX;
    public float runningSpeed;
    private float _currentRunningSpeed;
    public float xSpeed;
    private float newX = 0;
    private float touchDelta = 0;
    public Animator anim;

    [Header("Bridge Spawning Parameters")]
    private bool _spawnBridge;
    private BridgeSpawner _bridgeSpawner = null;
    private float _creatingBridgeTimer;

    private float _scoreTimer=0;
    private bool _finished;

    private void Awake()
    {
        CurrentPlayer = this;
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (LevelController.Current != null && LevelController.Current.gameActive)
        {
            MovePlayer();
            SpawnBridgeParts();
        }
    }

    public void ChangeSpeed(float value)
    {
        _currentRunningSpeed = value;
    }
    private void SpawnBridgeParts()
    {
        if (_spawnBridge)
        {
            _creatingBridgeTimer -= Time.deltaTime;
            if (_creatingBridgeTimer < 0)
            {
                _creatingBridgeTimer = 0.01f;
                IncrementCylinderVolume(-0.01f);
                GameObject createdBridgePiece = Instantiate(bridgePiecePrefab);
                Vector3 direction = _bridgeSpawner.endReference.transform.position - _bridgeSpawner.startReference.transform.position;
                float distance = direction.magnitude; // vector aðýrlýðý yani bileþkesi
                direction = direction.normalized; // normalize birim vectore dönüþtürüyoruz. yani yönü buluyoruz

                createdBridgePiece.transform.forward = direction;
                float characterDistance = transform.position.z - _bridgeSpawner.startReference.transform.position.z;
                characterDistance = Mathf.Clamp(characterDistance, 0, distance);
                Vector3 newPiecePosition = _bridgeSpawner.startReference.transform.position + direction * characterDistance;
                newPiecePosition.x = transform.position.x;
                createdBridgePiece.transform.position = newPiecePosition;

                if (_finished)
                {
                    _scoreTimer -= Time.deltaTime;
                    if (_scoreTimer<=0)
                    {
                        _scoreTimer = 0.3f;
                        LevelController.Current.ChangeScore(1);
                    }
                }
            }

        }
    }

    private void MovePlayer()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            touchDelta = Input.GetTouch(0).deltaPosition.x / Screen.width;
        }
        else if (Input.GetMouseButton(0))
        {
            touchDelta = Input.GetAxis("Mouse X");
        }

        newX = transform.position.x + xSpeed * touchDelta * Time.deltaTime;
        newX = Mathf.Clamp(newX, -limitX, limitX);
        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z + _currentRunningSpeed * Time.deltaTime);

        transform.position = newPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AddCylender"))
        {
            IncrementCylinderVolume(0.1f);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("SpawnBridge"))
        {
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }
        else if (other.CompareTag("StopSpawnBridge"))
        {
            StopSpawningBridge();
            if (_finished)
            {
                LevelController.Current.FinishGame();
            }
        }
        else if (other.tag=="Finish")
        {
            _finished = true;
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Trap"))
        {
            IncrementCylinderVolume(-Time.fixedDeltaTime);
        }
    }

    private void IncrementCylinderVolume(float value)
    {
        if (cylinders.Count == 0)
        {
            if (value > 0)
            {
                CreateCylinder(value);
            }
            else
            {
                if (_finished)
                {
                    LevelController.Current.FinishGame();
                }
                else
                {
                    Die();
                }
            }
        }
        else
        {
            cylinders[cylinders.Count - 1].IncrementCylinderVolume(value);
        }
    }

    public void CreateCylinder(float value)
    {
        RidingCylinder createdCylinder = Instantiate(ridingCylinderPrefab, transform).GetComponent<RidingCylinder>();
        cylinders.Add(createdCylinder);
        createdCylinder.IncrementCylinderVolume(value);
    }

    public void DestroCylinder(RidingCylinder cylinder)
    {
        cylinders.Remove(cylinder);
        Destroy(cylinder.gameObject);
    }

    public void StartSpawningBridge(BridgeSpawner spawner)
    {
        _bridgeSpawner = spawner;
        _spawnBridge = true;
    }

    public void StopSpawningBridge()
    {
        _spawnBridge = false;
    }

    public void Die()
    {
        anim.SetBool("Dead",true);
        gameObject.layer = 6;
        Camera.main.transform.SetParent(null);
        LevelController.Current.GameOver();
    }
}
