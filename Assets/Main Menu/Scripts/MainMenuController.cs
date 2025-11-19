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
        // Handle background music persistence
        SetupBackgroundMusic();
        
        // Set up SFX audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = sfxVolume;
    }

    private void SetupBackgroundMusic()
    {
        // Check if music manager already exists
        if (musicManager != null)
        {
            // Music already exists, destroy this duplicate
            Destroy(gameObject);
            return;
        }

        // Create persistent music manager
        musicManager = new GameObject("BackgroundMusicManager");
        DontDestroyOnLoad(musicManager);
        
        // Add audio source for background music
        musicSource = musicManager.AddComponent<AudioSource>();
        musicSource.volume = musicVolume;
        musicSource.loop = true;
        
        // Play background music if assigned
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    private void Start()
    {
        // Set up button click events
        startButton.onClick.AddListener(OnStartGame);
        funButton.onClick.AddListener(OnFunButton);
        exitButton.onClick.AddListener(OnExitGame);

        // Set up button hover events
        SetupButtonHoverSounds(startButton);
        SetupButtonHoverSounds(funButton);
        SetupButtonHoverSounds(exitButton);
    }

    private void SetupButtonHoverSounds(Button button)
    {
        // Add hover sound trigger
        var hoverTrigger = button.gameObject.AddComponent<ButtonHoverSound>();
        hoverTrigger.Initialize(buttonHoverSound, audioSource);
    }

    private void OnStartGame()
    {
        PlayClickSound();
        Debug.Log("Loading game scene: " + gameSceneName);
        
        // Load the game scene
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
        
        // You can add additional fun effects here like:
        // - Screen shake
        // - Particle effects
        // - Random color changes
        // - Temporary UI animations
    }

    private void OnExitGame()
    {
        PlayClickSound();
        Debug.Log("Exiting game...");
        
        // Destroy music manager when exiting to prevent duplicates on restart
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

    // Public methods to control background music from other scripts
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

// Separate component for handling button hover sounds
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
        // This ensures the component works even if added manually
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