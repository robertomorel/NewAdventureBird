using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    #region privates_fields
    private GameObject[] _sceneFeathers;                            // Guarda todas as penas que existem na fase
    private GameObject[] _sceneEggs;                                // Guarda todas os ovos que existem na fase
    private GameObject[] _sceneCoins;                               // Guarda todas as moedas que existem na fase

    private bool thereWasFeathersOnGame;                            // Variável que indica se a fase começou com alguma pena     
    private bool thereWasEggsOnGame;                                // Variável que indica se a fase começou com algum ovo 
    private bool thereWasCoinsOnGame;                               // Variável que indica se a fase começou com alguma moeda 

    private float _startDelay = 3.0f;                               // Delay usado antes de começar o jogo
    private float _endDelay = .5f;                                  // Delay usado antes de finalizar o jogo

    private WaitForSeconds _startWait;                              // Aplicação do delay para inicio do jogo
    private WaitForSeconds _endWait;                                // Aplicação do delay para fim do jogo
    #endregion

    #region privates_fields_serializebles
    [SerializeField] private GameObject _starFire;
    [SerializeField] private GameObject _starShield;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private GameObject _starCamera;

    [SerializeField] private GameObject _loadSceneGameObject;

    [SerializeField] private AudioClip _audioWin, _audioLose;
    [SerializeField] private GameObject _joystick;
    #endregion

    #region publics_fields
    public int sceneFeathersCount;
    public int sceneEggCount;
    public int sceneCoinCount;

    public int levelNumber;

    public bool puzzleDone;

    public GameObject _puzzleAnim;
    #endregion

    #region static_fields
    public static bool gameIsOn;
    public static bool gameOver;
    public static bool gameWin;
    #endregion

    void Awake()
    {
        Time.timeScale = 1.0f;

        _sceneFeathers = GameObject.FindGameObjectsWithTag("Feather");
        _sceneEggs = GameObject.FindGameObjectsWithTag("Egg");
        _sceneCoins = GameObject.FindGameObjectsWithTag("Coin");

        GetComponent<AudioSource>().mute = !GameConfig.useAudio;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameIsOn = false;
        gameOver = false;
        gameWin = false;

        puzzleDone = false;

        sceneFeathersCount = _sceneFeathers.Length;
        sceneEggCount = _sceneEggs.Length;
        sceneCoinCount = _sceneCoins.Length;

        thereWasFeathersOnGame = (sceneFeathersCount > 0);
        thereWasEggsOnGame = (sceneEggCount > 0);
        thereWasCoinsOnGame = (sceneCoinCount > 0);

        // -- Cria os delays padrão para início e fim de jogo
        _startWait = new WaitForSeconds(_startDelay);
        _endWait = new WaitForSeconds(_endDelay);

        SetLevelNumber();

        StartCoroutine(RoundStarting());
    }

    // Update is called once per frame
    void Update()
    {
        if (thereWasFeathersOnGame && sceneFeathersCount == 0)
        {
            if (GameConfig.showStarAnim)
            {
                StartCoroutine(FreeStarFromFire());
            }
            else
            {
                _starFire.SetActive(false);
            }
            
            thereWasFeathersOnGame = false;
        }

        if (thereWasEggsOnGame && sceneEggCount == 0)
        {
            if (GameConfig.showStarAnim)
            {
                StartCoroutine(FreeStarFromShield());
            }
            else
            {
                _starShield.SetActive(false);
            }
            
            thereWasEggsOnGame = false;
        }

        if (puzzleDone)
        {
            StartCoroutine(ActivatePuzzleAnimations());
        }

        CheckIfGameIsOver();
    }

    IEnumerator FreeStarFromFire()
    {
        gameIsOn = false;
        ActivateStarCamera();
        yield return new WaitForSeconds(1.5f);
        _starFire.SetActive(false);
    }

    IEnumerator FreeStarFromShield()
    {
        gameIsOn = false;
        ActivateStarCamera();
        yield return new WaitForSeconds(1.5f);
        _starShield.SetActive(false);
    }

    private IEnumerator RoundStarting()
    {
        // -- Aguarda pelo período de tempo específico antes de iniciar o jogo
        yield return _startWait;
        gameIsOn = true;
    }

    private IEnumerator RoundEnding()
    {
        // -- Aguarda pelo período de tempo específico antes de finalizar o jogo
        yield return _endWait;
        gameIsOn = false;
        gameOver = true;
    }

    void CheckIfGameIsOver()
    {
        if (PlayerManager.life == 0)
        {
            _joystick.SetActive(false);
            if (GameConfig.useAudio)
            {
                GetComponent<AudioSource>().PlayOneShot(_audioLose, .5f);
            }
            StartCoroutine(RoundEnding());
        }
    }

    void SetLevelNumber()
    {
        GameConfig.level = levelNumber;
    }

    public void LoadScene(string sceneName)
    {
        _loadSceneGameObject.SetActive(true);
        LoadScene loadScene = _loadSceneGameObject.GetComponent<LoadScene>();
        StartCoroutine(loadScene.LoadSceneCroutine(sceneName));
    }

    void ActivateStarCamera()
    {
        if (GameConfig.showStarAnim)
        {
            gameIsOn = false;
            _starCamera.SetActive(true);
        }
    }

    public void RoundWon()
    {
        ConcludLevelBySaving();
        if (GameConfig.useAudio)
        {
            GetComponent<AudioSource>().PlayOneShot(_audioWin, .5f);
        }
        _joystick.SetActive(false);
        Time.timeScale = .2f;
        GameManager.gameIsOn = false;
        GameManager.gameWin = true;
    }

    void ConcludLevelBySaving()
    {
        GameConfig.scoreGame += PlayerManager.score;
        GameObject.Find("SaveManager").GetComponent<SaveManager>().Save();
    }

    IEnumerator ActivatePuzzleAnimations()
    {
        puzzleDone = false;
        gameIsOn = false;
        _puzzleAnim.SetActive(true);
        yield return new WaitForSeconds(1.5f);
    }

}
