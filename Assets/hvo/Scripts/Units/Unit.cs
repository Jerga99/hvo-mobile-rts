

using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    [SerializeField] private ActionSO[] m_Actions;

    public bool IsMoving;
    public bool IsTargeted;

    protected Animator m_Animator;
    protected AIPawn m_AIPawn;
    protected SpriteRenderer m_SpriteRenderer;
    protected Material m_OriginalMaterial;
    protected Material m_HighlightMaterial;

    public ActionSO[] Actions => m_Actions;
    public SpriteRenderer Renderer => m_SpriteRenderer;

    protected void Awake()
    {
        if (TryGetComponent<Animator>(out var animator))
        {
            m_Animator = animator;
        }

        if (TryGetComponent<AIPawn>(out var aiPawn))
        {
            m_AIPawn = aiPawn;
        }

        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_OriginalMaterial = m_SpriteRenderer.material;
        m_HighlightMaterial = Resources.Load<Material>("Materials/Outline");
    }

    public void MoveTo(Vector3 destination)
    {
        var direction = (destination - transform.position).normalized;
        m_SpriteRenderer.flipX = direction.x < 0;

        m_AIPawn.SetDestination(destination);
    }

    public void Select()
    {
        Highlight();
        IsTargeted = true;
    }

    public void Deselect()
    {
        UnHighlight();
        IsTargeted = false;
    }

    void Highlight()
    {
        m_SpriteRenderer.material = m_HighlightMaterial;
    }

    void UnHighlight()
    {
        m_SpriteRenderer.material = m_OriginalMaterial;
    }
}
