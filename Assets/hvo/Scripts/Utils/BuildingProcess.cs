

using UnityEngine;

public class BuildingProcess
{
    private BuildActionSO m_BuildAction;
    private WorkerUnit m_Worker;
    private StructureUnit m_Structure;
    private ParticleSystem m_ConstructionEffect;
    private float m_ProgressTimer;
    private bool m_IsFinished;

    private bool InProgress => HasActiveWorker && m_Worker.CurrentState == UnitState.Building;
    public bool HasActiveWorker => m_Worker != null;

    public BuildingProcess(
        BuildActionSO buildAction,
        Vector3 placementPosition,
        WorkerUnit worker,
        ParticleSystem constructionEffectPrefab
    )
    {
        m_BuildAction = buildAction;
        var effectOffset = new Vector3(0, -1f, 0);
        m_ConstructionEffect = Object.Instantiate(
            constructionEffectPrefab,
            placementPosition + effectOffset,
            Quaternion.identity
        );
        m_Structure = Object.Instantiate(buildAction.StructurePrefab);
        m_Structure.Renderer.sprite = m_BuildAction.FoundationSprite;
        m_Structure.transform.position = placementPosition;
        m_Structure.RegisterProcess(this);
        worker.SendToBuild(m_Structure);
    }

    public void Update()
    {
        if (m_IsFinished) return;

        if (InProgress)
        {
            m_ProgressTimer += Time.deltaTime;

            if (!m_ConstructionEffect.isPlaying)
            {
                m_ConstructionEffect.Play();
            }

            if (m_ProgressTimer >= m_BuildAction.ConstructionTime)
            {
                m_IsFinished = true;
                m_Structure.Renderer.sprite = m_BuildAction.CompletionSprite;
                m_Worker.OnBuildingFinished();
                m_Structure.OnConstructionFinished();
            }
        }
    }

    public void AddWorker(WorkerUnit worker)
    {
        if (HasActiveWorker) return;
        m_Worker = worker;
    }

    public void RemoveWorker()
    {
        if (!HasActiveWorker) return;
        m_Worker = null;
        m_ConstructionEffect.Stop();
    }
}
