using System;
using UnityEngine;

// Event hub for the game using observer pattern so scripts communicate through events instead of direct references.
public class GameEvents : MonoBehaviour
{
    // Events for combat interaction
    public static event Action OnPlayerAttack;         // Fires when the player attacks
    public static event Action<int> OnPlayerDamaged;   // Fires when player takes damage
    public static event Action<int> OnEnemyDamaged;    // Fires when enemy takes damage

    // Health Events
    public static event Action<int> OnPlayerHealthChanged;  // Fires when player HP updates
    public static event Action<int> OnEnemyHealthChanged;   // Fires when enemy HP updates

    // XP Events
    public static event Action<int, int, int> OnPlayerXPChanged; // currentXP, neededXP, and level
    public static event Action<int> OnPlayerLevelUp;             // Fires when level changes

    // Game State Events
    public static event Action OnPlayerDeath;  // Fires when player HP reaches 0
    public static event Action OnEnemyDeath;   // Fires when enemy HP reaches 0
    public static event Action OnGamePause;    // Fires when game is paused
    public static event Action OnGameResume;   // Fires when game is resumed

    // Helper methods to trigger events safely
    public static void PlayerAttack() => OnPlayerAttack?.Invoke();
    public static void PlayerDamaged(int damage) => OnPlayerDamaged?.Invoke(damage);
    public static void EnemyDamaged(int damage) => OnEnemyDamaged?.Invoke(damage);
    public static void PlayerHealthChanged(int health) => OnPlayerHealthChanged?.Invoke(health);
    public static void EnemyHealthChanged(int health) => OnEnemyHealthChanged?.Invoke(health);
    public static void PlayerDeath() => OnPlayerDeath?.Invoke();
    public static void EnemyDeath() => OnEnemyDeath?.Invoke();
    public static void GamePause() => OnGamePause?.Invoke();
    public static void GameResume() => OnGameResume?.Invoke();
    public static void PlayerXPChanged(int current, int needed, int level) => OnPlayerXPChanged?.Invoke(current, needed, level);
    public static void PlayerLevelUp(int level) => OnPlayerLevelUp?.Invoke(level);
}