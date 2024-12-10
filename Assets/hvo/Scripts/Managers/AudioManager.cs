


public class AudioManager : SingletonManager<AudioManager>
{

    protected override void Awake()
    {
        DontDestroyOnLoad(gameObject);
        base.Awake();
    }
}
