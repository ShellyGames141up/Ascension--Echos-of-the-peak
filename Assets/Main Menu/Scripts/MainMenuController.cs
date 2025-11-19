using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button funButton;
    [SerializeField] private Button exitButton;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip buttonHoverSound;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip funButtonSound;
    [SerializeField] private AudioClip backgroundMusic;

    [Header("Audio Settings")]
    [SerializeField] private float musicVolume = 0.5f;
    [SerializeField] private float sfxVolume = 1.0f;

    [Header("Scene Management")]
    [SerializeField] private string gameSceneName = "GameScene";

    private AudioSource audioSource;
    private AudioSource musicSource;
    private static GameObject musicManager;

    private void Awake()
    {
        SetupBackgroundMusic();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = sfxVolume;
    }

    private void SetupBackgroundMusic()
    {
        if (musicManager != null)
        {
            Destroy(gameObject);
            return;
        }
        
        musicManager = new GameObject("BackgroundMusicManager");
        DontDestroyOnLoad(musicManager);
        
        
        musicSource = musicManager.AddComponent<AudioSource>();
        musicSource.volume = musicVolume;
        musicSource.loop = true;
        
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    private void Start()
    {
       
        startButton.onClick.AddListener(OnStartGame);
        funButton.onClick.AddListener(OnFunButton);
        exitButton.onClick.AddListener(OnExitGame);

        
        SetupButtonHoverSounds(startButton);
        SetupButtonHoverSounds(funButton);
        SetupButtonHoverSounds(exitButton);
    }

    private void SetupButtonHoverSounds(Button button)
    {
      
        var hoverTrigger = button.gameObject.AddComponent<ButtonHoverSound>();
        hoverTrigger.Initialize(buttonHoverSound, audioSource);
    }

    private void OnStartGame()
    {
        PlayClickSound();
        Debug.Log("Loading game scene: " + gameSceneName);
        
        
        if (!string.IsNullOrEmpty(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.LogWarning("Game scene name not set!");
        }
    }

    private void OnFunButton()
    {
        PlayFunSound();
        Debug.Log("Fun button pressed! Playing fun sound effect.");
        
    }

    private void OnExitGame()
    {
        PlayClickSound();
        Debug.Log("Exiting game...");
        
        if (musicManager != null)
        {
            Destroy(musicManager);
        }
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void PlayClickSound()
    {
        if (buttonClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    private void PlayFunSound()
    {
        if (funButtonSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(funButtonSound);
        }
    }
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }

    public void StopBackgroundMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void PlayBackgroundMusic()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }
}

public class ButtonHoverSound : MonoBehaviour
{
    private AudioClip hoverSound;
    private AudioSource audioSource;
    private Button button;

    public void Initialize(AudioClip sound, AudioSource source)
    {
        hoverSound = sound;
        audioSource = source;
        button = GetComponent<Button>();
    }

    private void Start()
    {
        if (button == null)
            button = GetComponent<Button>();
    }

    public void OnPointerEnter()
    {
        if (hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }
}