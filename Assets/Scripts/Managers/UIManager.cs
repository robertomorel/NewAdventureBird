using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    #region privates_fields
    private GameManager _gameManager;                      // Referência ao objeto GameManager
    private bool _gameWinAnimationIsOn = false;            // Variável para chamar apenas uma vez a corrotina de vitória do jogo
    #endregion

    #region privates_fields_serializebles
    [SerializeField] private Text _startText;              // Referência ao texto de início do jogo
    [SerializeField] private GameObject _startTextGO;      // Referência ao Game Object do texto de início do jogo
    [SerializeField] private Text _winText;                // Referência ao texto de vitória do jogo
    [SerializeField] private GameObject _winTextGO;        // Referência ao Game Object do texto de vitória do jogo
    [SerializeField] private GameObject _winButtonNext;    // Referência ao Game Object do botão Next
    [SerializeField] private GameObject _winButtonMenu;    // Referência ao Game Object do botão Menu
    [SerializeField] private Text _loseText;               // Referência ao texto de derrota do jogo
    [SerializeField] private GameObject _loseTextGO;       // Referência ao Game Object do texto de derrota do jogo
    [SerializeField] private Text _scorePointsText;        // Referência ao texto de pontos do score do jogo
    [SerializeField] private Image hud;                    // Imagem do life do personagem 
    [SerializeField] private Sprite[] hudArray;            // Array de sprites dos lifes do personagem
    #endregion

    #region publics_fields
    public string nextLevelName;                           // Nome da fase seguinte         
    public bool isLastLevel;                               // Variável para informar se é o último nível 
    #endregion

    #region static_fields

    #endregion

    void Awake()
    {
        // -- Esconde todos os textos da tela
        HideAllTexts();
    }

    // Start is called before the first frame update
    void Start()
    {
        // -- Mostra o texto de início do jogo
        _startTextGO.SetActive(true);
        // -- Criar texto inicial do jogo
        _startText.text = "Iniciar Jogo";
        // -- Cria instância do script GameManager
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // -- Se o jogo já iniciou, esconde todos os textos...
        if (GameManager.gameIsOn)
        {
            HideAllTexts();
        }

        // -- Se o jogador perdeu, esconde todos os textos e depois mostra só os de Game Over
        if (GameManager.gameOver)
        {
            ShowGameOverTexts();
        }

        // -- Se o jogador venceu, esconde todos os textos e depois mostra só os de Game Win
        if (GameManager.gameWin)
        {
            ShowGameWinTexts();
        }

        // -- A imagem do life do jogador vai depender de quantas vidas ele tiver
        // -- O array deve ser criado de forma lógica com as imagens referentes aos lifes
        hud.sprite = hudArray[PlayerManager.life];

    }

    void HideAllTexts()
    {
        // -- Setta para "" todos os textos
        _winText.text = "";
        _loseText.text = "";
        _startText.text = "";

        // -- Desativa todos os game objects de interface
        _startTextGO.SetActive(false);
        _winTextGO.SetActive(false);
        _loseTextGO.SetActive(false);
    }

    void ShowGameOverTexts()
    {
        // -- Esconde todos os textos
        HideAllTexts();
        // -- Ativa game object de interface referente ao fim de jogo
        _loseTextGO.SetActive(true);
        // -- Setta texto de fim de jogo
        _loseText.text = "Fim de Jogo!!";
    }

    void ShowGameWinTexts()
    {
        // -- Variável de controle para chamar a corrotina ScoreTimingCount apenas uma vez
        if (!_gameWinAnimationIsOn)
        {
            _gameWinAnimationIsOn = true;

            // -- Esconde todos os textos
            HideAllTexts();
            // -- Ativa game object de interface referente à vitória do jogo
            _winTextGO.SetActive(true);
            // -- Setta texto de vitória
            _winText.text = "Você venceu!!";

            // -- Desativa botões Next e Menu
            _winButtonNext.SetActive(false);
            _winButtonMenu.SetActive(false);

            // -- Chama corrotina ScoreTimingCount
            StartCoroutine(ScoreTimingCount());
        }
    }

    public void BackToMenu()
    {
        _gameManager.LoadScene("MainMenu");
    }

    public void NextLevel()
    {
        _gameManager.LoadScene(nextLevelName);
    }

    public void ReloadLevel()
    {
        // -- Reinicia level atual
        _gameManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /*
     Método para que os scores apareçam como em um cronômetro, de um por um.
         */
    IEnumerator ScoreTimingCount()
    {
        int score = PlayerManager.score;
        // -- Percorre um laço pela quantidade de scores obtidos
        for (int i = 0; i < score; i++)
        {
            // -- A cada interação, o texto soma 1
            _scorePointsText.text = (i + 1).ToString();
            // -- Espera por 0,005 segundos
            yield return new WaitForSeconds(.005f);
        }
        yield return new WaitForSeconds(.3f);
        // -- Após 0,3 segundos, mostra os botões Next e Menu
        _winButtonNext.SetActive(true);
        _winButtonMenu.SetActive(true);
        if (isLastLevel)
        {
            // -- Apaga o botão Next caso seja a última fase
            _winButtonNext.SetActive(false);
        }
    }

}