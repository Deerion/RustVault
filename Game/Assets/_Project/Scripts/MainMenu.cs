using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject optionsPanel; // do podpiecia w inspektorze

    private void Start()
    {
        // na starcie upewniam sie, ze panel opcji jest ukryty
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    // podpiete pod przycisk Graj / New Game
    public void PlayNewGame()
    {
        // ladowanie glownej sceny gry
        SceneManager.LoadScene("SampleScene");
    }

    // podpiete pod przycisk Opcje
    public void ToggleOptions()
    {
        // odwraca stan panelu (jak wlaczony to wylacza i na odwrot)
        if (optionsPanel != null)
            optionsPanel.SetActive(!optionsPanel.activeSelf);
    }

    // podpiete pod przycisk Wyjscie
    public void QuitGame()
    {
        Debug.Log("Wychodze z gry...");
        Application.Quit();
    }
}