using UnityEngine;
using System;

public abstract class LivingEntity : MonoBehaviour, IDamageable
{
    [Header("Statystyki Życia")]
    public int maxHealth = 100;
    public int currentHealth;
    public bool isDead;

    // Event informujący inne skrypty (np. UI albo GameManager), że ta encja umarła
    public event Action OnDeath;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        isDead = false;
    }

    // Implementacja interfejsu IDamageable - polimorficzne zadawanie obrażeń
    public virtual void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} otrzymał {amount} obrażeń. HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    // Metoda wirtualna - pozwala na dopisanie własnej logiki zgonu w klasach pochodnych
    public virtual void Die()
    {
        isDead = true;
        Debug.Log($"{gameObject.name} umarł.");

        // Wywołujemy zdarzenie o śmierci, jeśli ktoś je subskrybuje
        OnDeath?.Invoke();

        // Domyślne zniszczenie obiektu w Unity
        Destroy(gameObject);
    }
}