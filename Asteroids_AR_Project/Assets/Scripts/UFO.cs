using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class UFO : MonoBehaviour
{
    public enum UFOStates
    {
        Idle,
        Attacking
    }


    [SerializeField] private UFOStates _currentState;
    [SerializeField] private List<Vector3> _trajectoryVectors = new List<Vector3>();
    [SerializeField] private int _trajectoriesPerSpawn = 2;
    [SerializeField] private float _spawnDistanceFromPlayer = 20;
    [SerializeField] private float _xyOffset = 10;
    public float _minDistance = 50.0f; // Minimum distance from the camera.
    public float _maxDistance = 100.0f; // Maximum distance from the camera.

    [SerializeField] private float _movementSpeed = 5;
    [SerializeField] private int _cooldownMinTime = 5;
    [SerializeField] private int _cooldownMaxTime = 15;
    [SerializeField] private GameState _gameState;



    private Transform _player;

    [SerializeField] private UnityEvent OnStartAttacking;
    [SerializeField] private UnityEvent OnStopAttacking;
    [SerializeField] private UnityEvent OnDie;
    Vector3 GetNewPositionVector()
    {
        Vector3 cameraPosition = Camera.main.transform.position;

        // Generate random X and Y coordinates within the specified range.
        float randomX = Random.Range(-_xyOffset, _xyOffset);
        float randomY = Random.Range(-_xyOffset, _xyOffset);

        // Calculate the Z coordinate using a random value within the specified range.
        float randomZ = Random.Range(_minDistance, _maxDistance);

        // Create a new Vector3 using the random coordinates.
        Vector3 newPosition = new Vector3(cameraPosition.x + randomX, cameraPosition.y + randomY, cameraPosition.z + randomZ);

        return newPosition;
    }
    private IEnumerator IdleRoutine()
    {
        transform.position = new Vector3(1000, 1000, 1000);

        _trajectoryVectors.Clear();

        yield return new WaitForSeconds(Random.Range(_cooldownMinTime, _cooldownMaxTime));

        CurrentState = UFOStates.Attacking;
    }

    public void StartCooldown()
    {
        StartCoroutine(IdleRoutine());
    }
    public UFOStates CurrentState
    {
        get => _currentState;
        set
        {
            _currentState = value;

            if (_currentState == UFOStates.Attacking)
            {
                OnStartAttacking?.Invoke();
            }
            else
            {
                OnStopAttacking?.Invoke();
            }
        }



    }
    private void Start()
    {

        // find the player in the scene.
        GameObject playerObject = GameObject.Find("Player");

        // If the Player is found,then grab its transform object.
        if (playerObject)
        {
            _player = playerObject.transform;
        }

        // Players should start in this state.
        CurrentState = UFOStates.Idle;
    }

    public void StartAttacking()
    {
        // Check if the player is available
        if (_player == null) return;

        // Define the new spawn position
        Vector3 spawnPosition = GetNewPositionVector();
        transform.position = spawnPosition;

        // Define new random trajectory vectors
        for (int i = 0; i < _trajectoriesPerSpawn; i++)
        {
            _trajectoryVectors.Add(GetNewPositionVector());
        }

        StartCoroutine(AttackMovement());
    }

    IEnumerator AttackMovement()
    {
        for (int i = 0; i < _trajectoryVectors.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, _trajectoryVectors[i]);

            while (distance > 0.5f && !_gameState.GameOver)
            {
                // wait a frame
                yield return null;

                transform.position = Vector3.MoveTowards(transform.position, _trajectoryVectors[i], Time.deltaTime * _movementSpeed);

                distance = Vector3.Distance(transform.position, _trajectoryVectors[i]);
            }
        }

        CurrentState = UFOStates.Idle;
    }

    public void Die()
    {
        OnDie?.Invoke();
        OnStopAttacking?.Invoke();

        StopAllCoroutines();
        StartCooldown();
    }
}
