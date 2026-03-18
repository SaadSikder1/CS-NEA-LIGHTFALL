using UnityEngine;

// Implementing 'IDamageable' acts as an interface
// The 'partial' keyword allows the class definition to be split across multiple files if needed.
public partial class Target : MonoBehaviour, IDamageable
{
    // Encapsulated state variable. [SerializeField] exposes it to the Unity Inspector without making it public.
    [SerializeField] private float health = 100f;

    // Public method fulfilling the IDamageable interface contract.
    // This allows other scripts (like your Gun script) to interact with it in a standardised way.
    public void TakeDamage(float amount)
    {
        health -= amount;
        
        // Checks the condition to trigger the destruction sequence.
        if (health <= 0f) Die();
    }

    // Private method encapsulating the logic for when the target is eliminated.
    private void Die()
    {
        // Null check to prevent a runtime error if the GameManager hasn't been instantiated yet.
        // This interacts with the Singleton design pattern.
        if (GameManager.Instance != null)
        {
            // Accesses the single, globally available instance of the GameManager to update the score.
            GameManager.Instance.AddScore(10);
        }

        // Removes the object from the active scene, allowing the garbage collector to free up memory.
        Destroy(gameObject);
    }
}