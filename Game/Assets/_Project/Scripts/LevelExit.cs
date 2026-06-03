using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [Header("Referencje UI")]
    public GameObject winPanel; // Tu podepniemy nasz panel wygranej

    private void Start()
    {
        // Upewniamy się, że panel jest wyłączony na starcie gry
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
    }

    // Ta funkcja odpala się automatycznie, gdy coś wejdzie w nasz Box Collider (Trigger)
    private void OnTriggerEnter(Collider other)
    {
        // Sprawdzamy, czy obiekt, który wszedł w strefę, ma tag "Player"
        if (other.CompareTag("Player"))
        {
            Debug.Log("Gracz dotarł do wyjścia!");
            LevelComplete();
        }
    }

    private void LevelComplete()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true); // Pokazujemy napis o wygranej
        }

        Time.timeScale = 0f; // Zatrzymujemy czas w grze (potwory stają)

        // Uwalniamy kursor, żeby gracz mógł kliknąć przycisk powrotu do menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Metoda do podpięcia pod przycisk na ekranie wygranej
    public void ReturnToMenu()
    {
        Time.timeScale = 1f; // Odmrażamy czas przed zmianą sceny
        SceneManager.LoadScene("MainMenu");
    }
}