using UnityEngine;
using UnityEngine.SceneManagement;

public enum MusicName
{
    title,
    overworld,
    battle,
    victory,
    gameover,
    COUNT
}
public enum SoundName
{
    bash,
    bow,
    spear,
    sword,
    cure,
    hurt1,
    hurt2,
    hurt3,
    hit1,
    hit2,
    impact,
    clash,
    COUNT
}

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

        music = gameObject.AddComponent<AudioSource>();
        sound = gameObject.AddComponent<AudioSource>();
        music.volume = 0.2f; 
        sound.volume = 0.5f; 
        LoadAudio();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void LoadAudio()
    {
        musicClips[(int)MusicName.title] = Resources.Load<AudioClip>("Audio/BGM/title");
        musicClips[(int)MusicName.overworld] = Resources.Load<AudioClip>("Audio/BGM/overworld");
        musicClips[(int)MusicName.battle] = Resources.Load<AudioClip>("Audio/BGM/battle");
        musicClips[(int)MusicName.victory] = Resources.Load<AudioClip>("Audio/BGM/victory");
        musicClips[(int)MusicName.gameover] = Resources.Load<AudioClip>("Audio/BGM/gameover");

        soundClips[(int)SoundName.bash] = Resources.Load<AudioClip>("Audio/SFX/Bash");
        soundClips[(int)SoundName.bow] = Resources.Load<AudioClip>("Audio/SFX/Bow");
        soundClips[(int)SoundName.cure] = Resources.Load<AudioClip>("Audio/SFX/Cure");
        soundClips[(int)SoundName.spear] = Resources.Load<AudioClip>("Audio/SFX/Spear");
        soundClips[(int)SoundName.sword] = Resources.Load<AudioClip>("Audio/SFX/Sword");
        soundClips[(int)SoundName.hurt1] = Resources.Load<AudioClip>("Audio/SFX/Hurt1");
        soundClips[(int)SoundName.hurt2] = Resources.Load<AudioClip>("Audio/SFX/Hurt2");
        soundClips[(int)SoundName.hurt3] = Resources.Load<AudioClip>("Audio/SFX/Hurt3");
        soundClips[(int)SoundName.hit1] = Resources.Load<AudioClip>("Audio/SFX/Hit1");
        soundClips[(int)SoundName.hit2] = Resources.Load<AudioClip>("Audio/SFX/Hit2");
        soundClips[(int)SoundName.impact] = Resources.Load<AudioClip>("Audio/SFX/Impact");
        soundClips[(int)SoundName.clash] = Resources.Load<AudioClip>("Audio/SFX/Clash");
    }

    public void PlaySound(SoundName soundId)
    {
        sound.clip = soundClips[(int)soundId];
        sound.Play();
        Debug.Log ($"Playing sound: {soundId}");
    }

    public void PlayMusic(MusicName musicId, bool loopMusic)
    {
        if (music == null)
        {
            Debug.LogError("AudioManager: Music AudioSource is missing!");
            return;
        }

        AudioClip clip = musicClips[(int)musicId];
        if (clip == null)
        {
            Debug.LogError($"AudioManager: Missing clip for {musicId}");
            return;
        }

        if (currentMusic == musicId && music.isPlaying)
            return;

        currentMusic = musicId;
        music.clip = clip;
        music.loop = loopMusic;
        music.Play();
        Debug.Log($"Playing music: {musicId}");
    }

    public void ToggleMusic(bool enable)
    {
        if (music != null)
        {
            music.mute = !enable;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.buildIndex)
        {
            case 0: // Title Scene
                PlayMusic(MusicName.title, true);
                break;
            case 1: // Overworld Scene
                PlayMusic(MusicName.overworld, true);
                break;
            case 2: // Battle Scene
                PlayMusic(MusicName.battle, true);
                break;
        }
    }
}
