using UnityEngine;

// Manages all game audio: background music and sound effects.
// Singleton with DontDestroyOnLoad so it persists across all scenes.
// Listens to GameEvents to play sounds automatically without direct references.
// Uses two separate AudioSources: one for looping music, one for one-shot SFX.
public class AudioManager : MonoBehaviour
{
    // Singleton instance — accessible from anywhere via AudioManager.Instance
    public static AudioManager Instance;

    [Header("Background Music")]
    public AudioClip backgroundMusic;

    [Header("Player Sounds")]
    public AudioClip playerAttackSound;
    public AudioClip playerHitSound;
    public AudioClip blockSound;
    public AudioClip dodgeSound;

    [Header("Enemy Sounds")]
    public AudioClip witchAttackSound;
    public AudioClip witchHitSound;
    public AudioClip skeletonAttackSound;
    public AudioClip skeletonHitSound;
    public AudioClip projectileHitSound;

    [Header("Game Sounds")]
    public AudioClip victorySound;
    public AudioClip defeatSound;

    [Header("UI Sounds")]
    public AudioClip fightBannerSound;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("SFX Spam Prevention")]
    [SerializeField] private float attackSoundCooldown = 0.4f; // Minimum seconds between attack sounds

    private AudioSource musicSource; // Dedicated source for looping background music
    private AudioSource sfxSource;   // Dedicated source for one-shot sound effects
    private float lastAttackSoundTime = 0f;    // Tracks last time player attack sound played
    private float lastSkeletonSoundTime = 0f;  // Tracks last time skeleton attack sound played
    private float lastWitchSoundTime = 0f;     // Tracks last time witch attack sound played

    void Awake()
    {
        // Singleton pattern: only one AudioManager exists at a time
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Create two AudioSources on this GameObject at runtime
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = musicVolume;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.volume = sfxVolume;

        // Subscribe to events in Awake (not Start) so they only register once
        GameEvents.OnPlayerAttack += PlayPlayerAttack;
        GameEvents.OnPlayerDamaged += PlayPlayerHit;
        GameEvents.OnEnemyDamaged += PlayEnemyHit;
        GameEvents.OnPlayerDeath += PlayDefeat;
        GameEvents.OnEnemyDeath += PlayVictory;
    }

    void Start()
    {
        PlayBackgroundMusic();
    }

    void OnDestroy()
    {
        GameEvents.OnPlayerAttack -= PlayPlayerAttack;
        GameEvents.OnPlayerDamaged -= PlayPlayerHit;
        GameEvents.OnEnemyDamaged -= PlayEnemyHit;
        GameEvents.OnPlayerDeath -= PlayDefeat;
        GameEvents.OnEnemyDeath -= PlayVictory;
    }

    void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && !musicSource.isPlaying)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    // Attack sound has cooldown to prevent spam when player clicks rapidly
    void PlayPlayerAttack()
    {
        if (Time.time - lastAttackSoundTime < attackSoundCooldown) return;
        lastAttackSoundTime = Time.time;
        PlaySFX(playerAttackSound);
    }

    void PlayPlayerHit(int damage) => PlaySFX(playerHitSound);
    void PlayEnemyHit(int damage) => PlaySFX(skeletonHitSound);

    void PlayVictory()
    {
        sfxSource.Stop();
        PlaySFX(victorySound);
    }

    void PlayDefeat()
    {
        sfxSource.Stop();
        PlaySFX(defeatSound);
    }

    // Public methods called directly by enemy/player scripts
    public void PlayFightBanner() => PlaySFX(fightBannerSound);
    public void PlayWitchAttack()
    {
        if (Time.time - lastWitchSoundTime < attackSoundCooldown) return;
        lastWitchSoundTime = Time.time;
        PlaySFX(witchAttackSound);
    }

    public void PlaySkeletonAttack()
    {
        if (Time.time - lastSkeletonSoundTime < attackSoundCooldown) return;
        lastSkeletonSoundTime = Time.time;
        PlaySFX(skeletonAttackSound);
    }
    public void PlayProjectileHit() => PlaySFX(projectileHitSound);
    public void PlayBlock() => PlaySFX(blockSound);
    public void PlayDodge() => PlaySFX(dodgeSound);
    public void StopSFX() => sfxSource.Stop();

    // Core SFX method — now actually uses sfxVolume (was hard-coded to 1f before)
    void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip, sfxVolume);
        else
            Debug.LogWarning("PlaySFX called but clip is NULL!");
    }
}