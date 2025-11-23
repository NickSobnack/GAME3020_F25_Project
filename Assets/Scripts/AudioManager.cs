using UnityEngine;

public enum MusicName
{
    title,
    overworld,
    battle,
    victory,
    COUNT
}
public enum SoundName
{
    bash,
    bow,
    spear,
    sword,
    cure,
    COUNT
}

// Audio Singleton that loads sound and music clips and persists throughout all scenes.
// Can be called from other events to play either sound or music.

public class AudioManager : MonoBehaviour
{
    AudioClip[] musicClips = new AudioClip[(int)MusicName.COUNT];
    AudioClip[] soundClips = new AudioClip[(int)SoundName.COUNT];

    AudioSource music;
    AudioSource sound;
    private MusicName currentMusic;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        music = gameObject.AddComponent<AudioSource>();
        sound = gameObject.AddComponent<AudioSource>();

        LoadAudio();
    }

    private void LoadAudio()
    {
        musicClips[(int)MusicName.title] = Resources.Load<AudioClip>("Audio/BGM/title");
        musicClips[(int)MusicName.overworld] = Resources.Load<AudioClip>("Audio/BGM/overworld");
        musicClips[(int)MusicName.battle] = Resources.Load<AudioClip>("Audio/BGM/battle");
        musicClips[(int)MusicName.victory] = Resources.Load<AudioClip>("Audio/BGM/victory");

        soundClips[(int)SoundName.bash] = Resources.Load<AudioClip>("Audio/SFX/Bash");
        soundClips[(int)SoundName.bow] = Resources.Load<AudioClip>("Audio/SFX/Bow");
        soundClips[(int)SoundName.cure] = Resources.Load<AudioClip>("Audio/SFX/Cure");
        soundClips[(int)SoundName.spear] = Resources.Load<AudioClip>("Audio/SFX/Spear");
        soundClips[(int)SoundName.sword] = Resources.Load<AudioClip>("Audio/SFX/Sound");
    }

    public void PlaySound(SoundName soundId)
    {
        sound.clip = soundClips[(int)soundId];
        sound.Play();
    }

    public void PlayMusic(MusicName musicId, bool loopMusic)
    {
        if (currentMusic == musicId && music.isPlaying)
        {
            return;
        }

        currentMusic = musicId;
        music.clip = musicClips[(int)musicId];
        music.loop = loopMusic;
        music.Play();
    }
}