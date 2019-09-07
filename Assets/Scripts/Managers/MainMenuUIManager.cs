using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{

    [SerializeField] private GameObject _loadSceneGameObject;
    [SerializeField] private GameObject _background;
    [SerializeField] private GameObject _configPanel;
    [SerializeField] private GameObject _creditsPanel;

    void Awake()
    {
        Time.timeScale = 1.0f;
    }

    void Start()
    {

    }

    void Update()
    {
        GetComponent<AudioSource>().mute = !GameConfig.useAudio;
    }

    public void Start_Onclick()
    {

    }

    public void Continue_Onclick()
    {

    }

    public void Tutorial_Onclick()
    {
        //SceneManager.LoadScene("Tutorial_" + GameConfig.tutorial.ToString());
        LoadScene("Tutorial_1");
    }

    public void Credits_Onclick()
    {
        _background.SetActive(true);
        _creditsPanel.SetActive(true);
    }

    public void Config_Onclick()
    {
        _background.SetActive(true);
        _configPanel.SetActive(true);
    }

    void LoadScene(string sceneName)
    {
        _loadSceneGameObject.SetActive(true);
        LoadScene loadScene = _loadSceneGameObject.GetComponent<LoadScene>();
        StartCoroutine(loadScene.LoadSceneCroutine(sceneName));
    }
}
