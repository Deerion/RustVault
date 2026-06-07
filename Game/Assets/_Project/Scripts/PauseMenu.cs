using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private CanvasGroup pauseCanvasGroup;

    [SerializeField] private float fadeDuration = 0.2f;

    [Header("Interfejs")]
    public GameObject crosshairUI;

    private bool isPaused = false;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        // na starcie ukrywamy pauze
        pausePanel.SetActive(false);
        pauseCanvasGroup.alpha = 0f;
    }

    private void Update()
    {
        // sprawdzanie czy wcisnieto escape
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // zatrzymanie czasu w grze

        // odblokowanie i pokazanie kursora, zeby dalo sie klikac w menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        pausePanel.SetActive(true);

        // plynne pojawianie
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeUI(pauseCanvasGroup, 0f, 1f, fadeDuration));

        if (crosshairUI != null)
        {
            crosshairUI.SetActive(false); // Chowa celownik
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // wznowienie czasu

        // zablokowanie i ukrycie kursora po powrocie do gry
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // plynne znikanie
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeUI(pauseCanvasGroup, 1f, 0f, fadeDuration, true));

        if (crosshairUI != null)
        {
            crosshairUI.SetActive(true); // Przywraca celownik
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // trzeba zresetowac czas przed zmiana sceny, bo inaczej menu bedzie zamrozone
        SceneManager.LoadScene("MainMenu");
    }

    // korutyna do fade'owania UI
    private IEnumerator FadeUI(CanvasGroup cg, float start, float end, float duration, bool disable = false)
    {
        float elapsed = 0f;
        cg.alpha = start;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // unscaled bo timeScale = 0 podczas pauzy
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }

        cg.alpha = end;
        if (disable) pausePanel.SetActive(false);
    }
}