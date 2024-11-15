

public class StructureUnit: Unit
{
    private BuildingProcess m_BuildingProcess;

    public bool IsUnderConstuction => m_BuildingProcess != null;

    void Update()
    {
        if (IsUnderConstuction)
        {
            m_BuildingProcess.Update();
        }
    }

    public void RegisterProcess(BuildingProcess process)
    {
        m_BuildingProcess = process;
    }
}
