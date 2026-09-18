using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string gameplaySceneName = "SampleScene";
    public GameObject tutorialPanel;

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
        Debug.Log("[Menu] 멀티플레이 - 준비 중입니다.");
    }
}
