using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Muryotaisu;

// Manages the fight setup and game flow in the Map scene.
// Responsible for spawning the correct enemy, positioning the player,
// triggering the fight sequence, and handling win/lose outcomes.
public class ArenaManager : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject enemy1Prefab; // Witch prefab
    public GameObject enemy2Prefab; // Skeleton prefab

    [Header("Spawn Points")]
    public Transform enemySpawnPoint; // Where the enemy appears
    public Transform playerSpawnPoint; // Where the player starts

    [Header("References")]
    public CombatUIManager combatUI; // Handles all UI updates
    public GameObject player;        // The player GameObject

    [Header("Game Flow")]
    public float fightBannerDelay = 1f; // Seconds before FIGHT banner appears
    public bool autoStartFight = true;  // Whether to auto-start the fight sequence

    [Header("Scene Names")]
    public string mainMenuScene = "MainMenu";
    public string enemySelectionScene = "EnemySelection";

    private GameObject currentEnemy;
    private bool fightStarted = false;

    void Start()
    {
        // Read which enemy was selected in the EnemySelection scene
        int selectedEnemyIndex = PlayerPrefs.GetInt("SelectedEnemyIndex", 0);

        // Move player to spawn point, disabling CharacterController to avoid conflicts
        if (player != null && playerSpawnPoint != null)
        {
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
                player.transform.position = playerSpawnPoint.position;
                player.transform.rotation = playerSpawnPoint.rotation;
                controller.enabled = true;
            }
            else
            {
                player.transform.position = playerSpawnPoint.position;
                player.transform.rotation = playerSpawnPoint.rotation;
            }
        }

        // Make sure player can't move before fight starts
        SetPlayerMovement(false);

        // Spawn the chosen enemy and link it to the UI
        SpawnEnemy(selectedEnemyIndex);

        // Subscribe to death events to detect win/lose
        GameEvents.OnEnemyDeath += OnEnemyDefeated;
        GameEvents.OnPlayerDeath += OnPlayerDefeated;

        // Begin the READY/FIGHT countdown
        if (autoStartFight)
            StartCoroutine(StartFightSequence());
    }

    // Unsubscribe from events when destroyed to prevent memory leaks
    void OnDestroy()
    {
        GameEvents.OnEnemyDeath -= OnEnemyDefeated;
        GameEvents.OnPlayerDeath -= OnPlayerDefeated;
    }

    // Enables or disables player movement and combat
    void SetPlayerMovement(bool enabled)
    {
        if (player == null) return;

        MuryotaisuController movement = player.GetComponent<MuryotaisuController>();
        PlayerCombat combat = player.GetComponent<PlayerCombat>();

        if (movement != null) movement.canMove = enabled;
        if (combat != null) combat.enabled = enabled;
        
        if (currentEnemy != null)
        { 
            WitchAI witch = currentEnemy.GetComponent<WitchAI>();
            SkeletonAI skeleton = currentEnemy.GetComponent<SkeletonAI>();
            if (witch != null) witch.enabled = enabled;
            if (skeleton != null) skeleton.enabled = enabled;
        }
    }

    // Instantiates the correct enemy prefab based on player's selection
    void SpawnEnemy(int index)
    {
        GameObject enemyToSpawn = null;

        if (index == 0 && enemy1Prefab != null)
            enemyToSpawn = enemy1Prefab;
        else if (index == 1 && enemy2Prefab != null)
            enemyToSpawn = enemy2Prefab;
        else
        {
            Debug.LogError("Invalid enemy index or prefab not assigned: " + index);
            return;
        }

        if (currentEnemy != null)
            Destroy(currentEnemy);

        if (enemySpawnPoint != null)
        {
            currentEnemy = Instantiate(enemyToSpawn, enemySpawnPoint.position, enemySpawnPoint.rotation);

            // Link enemy health to the combat UI so HP bar updates correctly
            EnemyHealth enemyHealth = currentEnemy.GetComponent<EnemyHealth>();
            if (combatUI != null && enemyHealth != null)
                combatUI.SetCurrentEnemy(enemyHealth);

            Debug.Log("Spawned: " + enemyToSpawn.name);
        }
    }

    // Waits for banner delay, shows FIGHT banner, then unlocks player controls
    IEnumerator StartFightSequence()
    {
        yield return new WaitForSeconds(fightBannerDelay);

        if (combatUI != null)
            combatUI.ShowFightBanner();
        yield return new WaitUntil(() => combatUI.fightSequenceDone); //Wait until banner signals it's done

        // Unlock player controls after fight banner finishes
        SetPlayerMovement(true);
        fightStarted = true;
        Debug.Log("FIGHT STARTED");
    }

    void OnEnemyDefeated() => fightStarted = false;
    void OnPlayerDefeated() => fightStarted = false;

    public void RestartFight() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void BackToEnemySelection() => SceneManager.LoadScene(enemySelectionScene);
    public void BackToMainMenu() => SceneManager.LoadScene(mainMenuScene);
    public void QuitGame() { Application.Quit(); Debug.Log("Quit Game"); }
}