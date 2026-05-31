using UnityEngine;

public class DestructibleObject : MonoBehaviour, IDamageable
{
    [Header("Ustawienia Obiektu")]
    public int health = 30;
    public int scrapDropAmount = 5; // Ilość złomu upuszczana po zniszczeniu

    private bool isDestroyed = false;

    // Bezpośrednia implementacja interfejsu IDamageable
    public void TakeDamage(int amount)
    {
        if (isDestroyed) return;

        health -= amount;
        Debug.Log($"{gameObject.name} (Niszczalny) otrzymał {amount} obrażeń. Pozostałe HP: {health}");

        if (health <= 0)
        {
            DestroyObject();
        }
    }

    private void DestroyObject()
    {
        isDestroyed = true;
        Debug.Log($"{gameObject.name} został zniszczony! Upuszczono złom w ilości: {scrapDropAmount}");

        // TODO: W tym miejscu w Etapie 5 wywołamy GameManager, 
        // aby zaktualizować licznik zasobów gracza.

        // Usunięcie obiektu ze sceny Unity
        Destroy(gameObject);
    }
}