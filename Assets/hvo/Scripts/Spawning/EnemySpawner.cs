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

    private int m_CurrentWaveIndex;
    private float m_DelayBetweenWavesTimer;
    private SpawnState m_SpawnState = SpawnState.Idle;

    public SpawnState SpawnState => m_SpawnState;

    public void Startup()
    {
        m_SpawnState = SpawnState.Waiting;
        m_DelayBetweenWavesTimer = m_DelayBetweenWaves;
        m_CurrentWaveIndex = 0;
    }

    void Update()
    {
        if (m_SpawnState == SpawnState.Finished)
        {
            Debug.Log("Spawning Finished");
            return;
        }
        else if (m_SpawnState == SpawnState.Waiting)
        {
            m_DelayBetweenWavesTimer -= Time.deltaTime;

            if (m_DelayBetweenWavesTimer <= 0)
            {
                m_SpawnState = SpawnState.Spawning;
                Debug.Log("Fight wave " + (m_CurrentWaveIndex + 1) + "!");
            }
        }
        else if (m_SpawnState == SpawnState.Spawning)
        {
            Debug.Log("Spawning!");
        }
    }
}
