using UnityEngine;

public class AudioManager : MonoBehaviour
{
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

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private AudioSource musicSource;
    private AudioSource sfxSource;
    public void StopSFX()
    {
        sfxSource.Stop();
    }

    void Awake()
    {
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

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = musicVolume;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.volume = sfxVolume;
    }

    void Start()
    {
        PlayBackgroundMusic();

        GameEvents.OnPlayerAttack += PlayPlayerAttack;
        GameEvents.OnPlayerDamaged += PlayPlayerHit;
        GameEvents.OnEnemyDamaged += PlayEnemyHit;
        GameEvents.OnPlayerDeath += PlayDefeat;
        GameEvents.OnEnemyDeath += PlayVictory;
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

    void PlayPlayerAttack() => PlaySFX(playerAttackSound);
    void PlayPlayerHit(int damage) => PlaySFX(playerHitSound);
    void PlayEnemyHit(int damage) => PlaySFX(skeletonHitSound);
    void PlayVictory()
    {
        sfxSource.Stop(); // stop any ongoing SFX
        PlaySFX(victorySound);
    }

    void PlayDefeat()
    {
        sfxSource.Stop();
        PlaySFX(defeatSound);
    }

    public void PlayWitchAttack() => PlaySFX(witchAttackSound);
    public void PlaySkeletonAttack() => PlaySFX(skeletonAttackSound);
    public void PlayProjectileHit() => PlaySFX(projectileHitSound);
    public void PlayBlock() => PlaySFX(blockSound);
    public void PlayDodge() => PlaySFX(dodgeSound);

   void PlaySFX(AudioClip clip)
   {
       if (clip != null)
       {
           Debug.Log("Playing SFX: " + clip.name);
           sfxSource.PlayOneShot(clip, 1f);
       }
       else
       {
           Debug.Log("PlaySFX called but clip is NULL!");
       }
   }
}