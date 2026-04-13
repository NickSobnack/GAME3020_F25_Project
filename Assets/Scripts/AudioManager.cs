using UnityEngine;
using UnityEngine.SceneManagement;

public enum MusicName
{
    title,
    field,
    swamp,
    battle,
    victory,
    gameover,
    shop,
    COUNT
}
public enum SoundName
{
    bash,
    bow,
    spear,
    sword,
    cure,
    impact,
    clash,
    coin,
    doorOpen,
    spell,
    COUNT
}

public class AudioManager : MonoBehaviour
{
    AudioClip[] musicClips = new AudioClip[(int)MusicName.COUNT];
    AudioClip[] soundClips = new AudioClip[(int)SoundName.COUNT];

    AudioSource music;
    AudioSource sound;
    private MusicName currentMusic;
    public MusicName PreviousMusic { get; private set; }

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
        musicClips[(int)MusicName.field] = Resources.Load<AudioClip>("Audio/BGM/overworldField");
        musicClips[(int)MusicName.swamp] = Resources.Load<AudioClip>("Audio/BGM/overworldSwamp");
        musicClips[(int)MusicName.battle] = Resources.Load<AudioClip>("Audio/BGM/battle");
        musicClips[(int)MusicName.victory] = Resources.Load<AudioClip>("Audio/BGM/victory");
        musicClips[(int)MusicName.gameover] = Resources.Load<AudioClip>("Audio/BGM/gameover");
        musicClips[(int)MusicName.shop] = Resources.Load<AudioClip>("Audio/BGM/BuySomething!");

        soundClips[(int)SoundName.bash] = Resources.Load<AudioClip>("Audio/SFX/Bash");
        soundClips[(int)SoundName.bow] = Resources.Load<AudioClip>("Audio/SFX/Bow");
        soundClips[(int)SoundName.cure] = Resources.Load<AudioClip>("Audio/SFX/Cure");
        soundClips[(int)SoundName.spear] = Resources.Load<AudioClip>("Audio/SFX/Spear");
        soundClips[(int)SoundName.sword] = Resources.Load<AudioClip>("Audio/SFX/Sword");
        soundClips[(int)SoundName.impact] = Resources.Load<AudioClip>("Audio/SFX/Impact");
        soundClips[(int)SoundName.clash] = Resources.Load<AudioClip>("Audio/SFX/Clash");
        soundClips[(int)SoundName.coin] = Resources.Load<AudioClip>("Audio/SFX/Coin");
        soundClips[(int)SoundName.doorOpen] = Resources.Load<AudioClip>("Audio/SFX/DoorOpen");
        soundClips[(int)SoundName.spell] = Resources.Load<AudioClip>("Audio/SFX/Spell");
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
            return;

        AudioClip clip = musicClips[(int)musicId];
        if (clip == null)
            return;

        PreviousMusic = currentMusic;

        currentMusic = musicId;
        music.clip = clip;
        music.loop = loopMusic;
        music.Play();
    }

    public void SetMusicVolume(float value)
    {
        music.volume = value;
    }

    public void SetSFXVolume(float value)
    {
        sound.volume = value;
    }

    public void MuteAudio(bool enable)
    {
        music.mute = enable;
        sound.mute = enable;
    }

    public float GetMusicVolume()
    {
        return music.volume;
    }

    public float GetSFXVolume()
    {
        return sound.volume;
    }

    public bool IsMuted()
    {
        return music.mute;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.buildIndex)
        {
            case 0: // Title Scene
                PlayMusic(MusicName.title, true);
                break;
            case 1: // Field Map Scene
                PlayMusic(MusicName.field, true);
                break;
            case 2: // Swamp Map Scene
                PlayMusic(MusicName.swamp, true);
                break;
            case 3: // Battle Scene
                PlayMusic(MusicName.battle, true);
                break;
        }
    }
}
