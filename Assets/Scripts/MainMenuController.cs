using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string gameplaySceneName = "SampleScene";
    public GameObject tutorialPanel;
    public GameObject multiplayerPanel;

    public void OnSinglePlayClicked()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnTutorialClicked()
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(true);
    }

    public void OnTutorialBackClicked()
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
    }

    public void OnMultiplayerClicked()
    {
        if (multiplayerPanel != null) multiplayerPanel.SetActive(true);
    }

    public void HideMultiplayerPanel()
    {
        if (multiplayerPanel != null) multiplayerPanel.SetActive(false);
    }
}
