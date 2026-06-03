using UnityEngine;
using UnityEngine.SceneManagement; // WAŻNE: Biblioteka do obsługi scen

public class MainMenu : MonoBehaviour
{
    // Metoda podpinana pod przycisk GRAJ
    public void PlayGame()
    {
        // Podmień "NazwaTwojejScenyZGra" na dokładną nazwę pliku z Twoim poziomem (np. "SampleScene")
        SceneManager.LoadScene("SampleScene");
    }

    // Metoda podpinana pod przycisk WYJŚCIE
    public void QuitGame()
    {
        Debug.Log("Zamykanie gry..."); // Ten log pojawi się w Unity (bo w edytorze gra się nie wyłączy)
        Application.Quit(); // To zadziała po zbudowaniu gotowej gry do pliku .exe
    }
}