using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    // Combat Events
    public static event Action OnPlayerAttack;
    public static event Action<int> OnPlayerDamaged;
    public static event Action<int> OnEnemyDamaged;
    
    // Health Events
    public static event Action<int> OnPlayerHealthChanged;
    public static event Action<int> OnEnemyHealthChanged;
    
    // Game State Events
    public static event Action OnPlayerDeath;
    public static event Action OnEnemyDeath;
    public static event Action OnGamePause;
    public static event Action OnGameResume;
    
    // Helper methods to trigger events
    public static void PlayerAttack() => OnPlayerAttack?.Invoke();
    public static void PlayerDamaged(int damage) => OnPlayerDamaged?.Invoke(damage);
    public static void EnemyDamaged(int damage) => OnEnemyDamaged?.Invoke(damage);
    public static void PlayerHealthChanged(int health) => OnPlayerHealthChanged?.Invoke(health);
    public static void EnemyHealthChanged(int health) => OnEnemyHealthChanged?.Invoke(health);
    public static void PlayerDeath() => OnPlayerDeath?.Invoke();
    public static void EnemyDeath() => OnEnemyDeath?.Invoke();
    public static void GamePause() => OnGamePause?.Invoke();
    public static void GameResume() => OnGameResume?.Invoke();
}