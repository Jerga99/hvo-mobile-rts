


using System.Collections.Generic;
using UnityEngine;

public class GoldMine: MonoBehaviour
{
    [SerializeField] private CapsuleCollider2D m_Collider;
    [SerializeField] private float m_EnterMineFreq = 2f;
    private int m_MaxAllowedMiners = 2;
    private Queue<WorkerUnit> m_ActiveMinersQueue = new();
    private float m_NextPossibleEnterTime;

    public bool TryToEnterMine(WorkerUnit worker)
    {
        if (
            m_ActiveMinersQueue.Count < m_MaxAllowedMiners
            && Time.time >= m_NextPossibleEnterTime
        )
        {
            worker.Hide();
            m_ActiveMinersQueue.Enqueue(worker);
            m_NextPossibleEnterTime = Time.time + m_EnterMineFreq;
            return true;
        }

        Debug.Log("Cannot enter yet!");
        return false;
    }

    public Vector3 GetBottomPosition()
    {
        return m_Collider.bounds.min;
    }
}
