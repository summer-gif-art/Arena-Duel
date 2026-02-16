using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ArenaManager : MonoBehaviour
{
    [Header("2 Enemy Prefabs")]
    public GameObject enemy1Prefab;
    public GameObject enemy2Prefab;
    
    [Header("Spawn Settings")]
    public Transform enemySpawnPoint;
    public Transform playerSpawnPoint;
    
    [Header("References")]
    public CombatUIManager combatUI;
    public GameObject player;
    
    [Header("Game Flow")]
    public float fightBannerDelay = 1f;
    public bool autoStartFight = true;
    
    private GameObject currentEnemy;
    private bool fightStarted = false;
    
    void Start()
    {
        // Get selected enemy from PlayerPrefs
        int selectedEnemyIndex = PlayerPrefs.GetInt("SelectedEnemyIndex", 0);
        
        // Position player at spawn
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
        
        // Spawn the selected enemy
        SpawnEnemy(selectedEnemyIndex);
        
        // Subscribe to events
        GameEvents.OnEnemyDeath += OnEnemyDefeated;
        GameEvents.OnPlayerDeath += OnPlayerDefeated;
        
        // Start fight after delay
        if (autoStartFight)
        {
            StartCoroutine(StartFightSequence());
        }
    }
    
    void OnDestroy()
    {
        GameEvents.OnEnemyDeath -= OnEnemyDefeated;
        GameEvents.OnPlayerDeath -= OnPlayerDefeated;
    }
    
    void SpawnEnemy(int index)
    {
        GameObject enemyToSpawn = null;
        string enemyName = "";
        
        // Select which enemy to spawn
        if (index == 0 && enemy1Prefab != null)
        {
            enemyToSpawn = enemy1Prefab;
            enemyName = "Enemy 1";
        }
        else if (index == 1 && enemy2Prefab != null)
        {
            enemyToSpawn = enemy2Prefab;
            enemyName = "Enemy 2";
        }
        else
        {
            Debug.LogError("Invalid enemy index or prefab not assigned: " + index);
            return;
        }
        
        // Destroy existing enemy if any
        if (currentEnemy != null)
        {
            Destroy(currentEnemy);
        }
        
        // Spawn enemy at spawn point
        if (enemyToSpawn != null && enemySpawnPoint != null)
        {
            currentEnemy = Instantiate(
                enemyToSpawn, 
                enemySpawnPoint.position, 
                enemySpawnPoint.rotation
            );
            
            // Get enemy health component
            EnemyHealth enemyHealth = currentEnemy.GetComponent<EnemyHealth>();
            
            // Link enemy to combat UI
            if (combatUI != null && enemyHealth != null)
            {
                combatUI.SetCurrentEnemy(enemyHealth);
            }
            
            Debug.Log("Spawned enemy: " + enemyName);
        }
        else
        {
            Debug.LogError("Enemy prefab or spawn point not assigned!");
        }
    }
    
    IEnumerator StartFightSequence()
    {
        yield return new WaitForSeconds(fightBannerDelay);
        
        if (combatUI != null)
        {
            combatUI.ShowFightBanner();
        }
        
        fightStarted = true;
        Debug.Log("FIGHT STARTED!");
    }
    
    void OnEnemyDefeated()
    {
        Debug.Log("Enemy defeated! Player wins!");
        fightStarted = false;
    }
    
    void OnPlayerDefeated()
    {
        Debug.Log("Player defeated! Game over!");
        fightStarted = false;
    }
    
    public void RestartFight()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void BackToEnemySelection()
    {
        SceneManager.LoadScene("EnemySelection");
    }
    
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
