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

        soundClips[(int)SoundName.bash] = Resources.Load<AudioClip>("Audio/SFX/bash");
        soundClips[(int)SoundName.bow] = Resources.Load<AudioClip>("Audio/SFX/bow");
        soundClips[(int)SoundName.spear] = Resources.Load<AudioClip>("Audio/SFX/spear");
        soundClips[(int)SoundName.sword] = Resources.Load<AudioClip>("Audio/SFX/sound");
    }

    public void PlaySound(SoundName soundId)
    {
        sound.clip = soundClips[(int)soundId];
        sound.Play();
        Debug.Log("Playing sound: " + soundId.ToString());
    }

    public void PlayMusic(MusicName musicId)
    {
        if (currentMusic == musicId && music.isPlaying)
        {
            return;
        }

        currentMusic = musicId;
        music.clip = musicClips[(int)musicId];
        music.loop = true;
        music.Play();
        Debug.Log("Playing music: " + musicId.ToString());
    }
}