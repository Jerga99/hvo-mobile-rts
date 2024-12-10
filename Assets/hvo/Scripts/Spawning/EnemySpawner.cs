using UnityEngine;

[System.Serializable]
public struct EnemyConfig
{
    public EnemyUnit EnemyPrefab;
    public float Propability;
}

[System.Serializable]
public struct SpawnWave
{
    public EnemyConfig[] Enemies;
    public float Frequency;
    public float Duration;
}

public enum SpawnState
{
    Idle, Spawning, Waiting, Finished
}


public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private SpawnWave[] m_SpawnWaves;
    [SerializeField] private Transform[] m_SpawnPoints;
    [SerializeField] private float m_DelayBetweenWaves;
}
