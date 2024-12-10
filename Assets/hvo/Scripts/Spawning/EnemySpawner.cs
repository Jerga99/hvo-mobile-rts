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
    private float m_WaveDurationTimer;
    private float m_SpawnFrequencyTimer;
    private SpawnState m_SpawnState = SpawnState.Idle;

    public SpawnState SpawnState => m_SpawnState;

    public void Startup()
    {
        m_SpawnState = SpawnState.Waiting;
        m_CurrentWaveIndex = 0;
        InitializeTimers();
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
            m_WaveDurationTimer -= Time.deltaTime;
            m_SpawnFrequencyTimer -= Time.deltaTime;

            if (m_WaveDurationTimer <= 0)
            {
                m_CurrentWaveIndex++;

                if (m_CurrentWaveIndex >= m_SpawnWaves.Length)
                {
                    m_SpawnState = SpawnState.Finished;
                }
                else
                {
                    m_SpawnState = SpawnState.Waiting;
                    InitializeTimers();
                }
            }
            else if (m_SpawnFrequencyTimer <= 0)
            {
                Spawn();
                m_SpawnFrequencyTimer = m_SpawnWaves[m_CurrentWaveIndex].Frequency;
            }
        }
    }

    void InitializeTimers()
    {
        m_DelayBetweenWavesTimer = m_DelayBetweenWaves;
        m_WaveDurationTimer = m_SpawnWaves[m_CurrentWaveIndex].Duration;
        m_SpawnFrequencyTimer = m_SpawnWaves[m_CurrentWaveIndex].Frequency;
    }

    void Spawn()
    {
        Debug.Log("Spawning new enemey!");
    }
}
