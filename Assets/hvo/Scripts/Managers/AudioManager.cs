

using UnityEngine;

public enum AudioPriority
{
    High = 0,
    Medium = 128,
    Low = 256
}

[System.Serializable]
public class AudioSettings
{
    public AudioClip[] Clips;
    public float Volume = 1.0f;
    public float Pitch = 1.0f;
    public bool Loop = false;
    public float SpatialBlend = 1.0f;
    public float MinDistance = 5.0f;
    public float MaxDistance = 50.0f;
    public AudioPriority Priority = AudioPriority.Medium;
}


public class AudioManager : SingletonManager<AudioManager>
{
    protected override void Awake()
    {
        DontDestroyOnLoad(gameObject);
        base.Awake();
    }
}
