using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUIManager : MonoBehaviour
{

    #region privates_fields_serializebles
    [SerializeField] private Text _text;               // Guarda o local onde os textos serão impressos
    [SerializeField] private string[] _strText;        // Array com todos os textos que serão mostrados em tela
    [SerializeField] private GameObject _helpPanel;    // Referência ao GameObject do painel de help
    #endregion

    #region privates_fields
    private bool _jumpText;                            // Controla de a animação de texto será pulada
    private bool _textAnimFinished;                    // Controla de alguma animação de texto está rodando 
    private bool _finishAllTexts;                      // Para verificar se todos os textos já foram mostrados
    private bool _tutorialStarted;                     // Para verificar se o tutorial já começou
    #endregion

    public AudioSource audioSource;

    int countText;

    // Start is called before the first frame update
    void Start()
    {
        countText = 0;
        _finishAllTexts = false;

        _jumpText = false;
        _textAnimFinished = true;

        Invoke("WriteTutorialTexts", 4.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_tutorialStarted)
        {
            return;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            // -- Se não há mais textos para serem passados...
            if (_finishAllTexts)
            {
                // -- Permite que o jogo continue
                GameManager.gameIsOn = true;
                // -- Desabilita o painel do tutorial
                _helpPanel.SetActive(false);
                // -- Destrói este game Object
                Destroy(this.gameObject);
            }
            else
            {
                // -- Se tiver alguma animação de texto sendo executada...
                if (!_textAnimFinished)
                {
                    // -- Marca para pular animação
                    _jumpText = true;
                }
                else
                {
                    StartCoroutine(WriteOnTime());
                }
            }

        }
    }

    void WriteTutorialTexts()
    {
        GameManager.gameIsOn = false;
        _helpPanel.SetActive(true);
        _tutorialStarted = true;
        StartCoroutine(WriteOnTime());
    }

    IEnumerator WriteOnTime()
    {
        _textAnimFinished = false;
        _text.text = "";

        char[] c = _strText[countText].ToCharArray();
        for (int i = 0; i < c.Length; i++)
        {
            if (GameConfig.useAudio)
            {
                audioSource.Play();
            }
            if (_jumpText)
            {
                break;
            }
            _text.text += c[i];
            yield return new WaitForSeconds(Random.Range(.08f, .11f));
        }
        if (GameConfig.useAudio)
        {
            audioSource.Stop();
        }
        _text.text = _strText[countText];
        yield return new WaitForSeconds(.5f);
        _textAnimFinished = true;
        _jumpText = false;
        countText++;
        if (countText == (_strText.Length))
        {
            _finishAllTexts = true;
        }
    }
}
