using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Do obsługi klawisza ESC w nowym systemie

public class PauseMenu : MonoBehaviour
{
    [Header("Referencja do interfejsu")]
    public GameObject pausePanel; // Tu podepniemy nasz Panel

    private bool isPaused = false;

    void Start()
    {
        // Upewniamy się, że menu pauzy jest wyłączone na starcie
        pausePanel.SetActive(false);
    }

    void Update()
    {
        // Sprawdzamy, czy wciśnięto przycisk ESC na klawiaturze
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true); // Pokazujemy menu
        Time.timeScale = 0f; // Zatrzymuje cały czas w grze (0 klatek na sekundę)
        isPaused = true;

        // Uwalniamy kursor myszy, żeby można było kliknąć przycisk
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false); // Chowamy menu
        Time.timeScale = 1f; // Przywracamy normalny czas gry
        isPaused = false;

        // Znowu blokujemy i chowamy kursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void LoadMainMenu()
    {
        // BARDZO WAŻNE: Przed zmianą sceny trzeba odblokować czas!
        // Inaczej Main Menu też wczyta się "zamrożone".
        Time.timeScale = 1f;

        // Tutaj upewnij się, że nazwa to dokładnie nazwa Twojej sceny menu
        SceneManager.LoadScene("MainMenu");
    }
}