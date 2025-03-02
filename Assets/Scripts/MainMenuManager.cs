using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private string _endlessMode = "EndlessMode";
    private string _levelsMode = "LevelsMode";
    
    public void PlayEndlessMode()
    {
        SceneLoader.Instance.LoadSceneAsync(_endlessMode);
    }

    public void PlayLevelsMode()
    {
        SceneLoader.Instance.LoadSceneAsync(_levelsMode);
    }
}