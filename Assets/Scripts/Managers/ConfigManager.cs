using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigManager : MonoBehaviour
{

    private Toggle _toggleAudio;
    private Toggle _toggleAnim;
    private Toggle _togglePt;
    private Toggle _toggleEn;

    [SerializeField] private GameObject _background;

    public GameObject audioConfig;             // Referência ao GO de configuração do audio 
    public GameObject animConfig;              // Referência ao GO de configuração da anim~ção 
    public GameObject languagePtConfig;        // Referência ao GO de configuração da linguagem em português 
    public GameObject languageEnConfig;        // Referência ao GO de configuração da linguagem em inglês 

    // Start is called before the first frame update
    void Start()
    {

        _toggleAudio = audioConfig.GetComponent<Toggle>();
        _toggleAnim = animConfig.GetComponent<Toggle>();
        _togglePt = languagePtConfig.GetComponent<Toggle>();
        _toggleEn = languageEnConfig.GetComponent<Toggle>();

        ChangeValuesScreenFromConfig();

        //Add listener for when the state of the Toggle changes, to take action
        _togglePt.onValueChanged.AddListener(delegate {
            TogglePtValueChanged(_togglePt, _toggleEn);
        });

        //Add listener for when the state of the Toggle changes, to take action
        _toggleEn.onValueChanged.AddListener(delegate {
            TogglePtValueChanged(_toggleEn, _togglePt);
        });
    }

    void Update()
    {
       
    }

    void TogglePtValueChanged(Toggle toggle, Toggle toggle_p)
    {
        if (toggle.isOn)
        {
            toggle_p.isOn = false;
        }
    }

    void ChangeValuesScreenFromConfig()
    {
        _toggleAudio.isOn = GameConfig.useAudio;
        _toggleAnim.isOn = GameConfig.showStarAnim;
        _togglePt.isOn = GameConfig.language == 1 ? true : false;
        _toggleEn.isOn = !_togglePt.isOn;
    }

    public void CloseAndSaveConfigScreen()
    {
        ChangeValuesConfigFromScreen();
        GameObject.Find("SaveManager").GetComponent<SaveManager>().Save();
        _background.SetActive(false);
        this.gameObject.SetActive(false);
    }

    void ChangeValuesConfigFromScreen()
    {
        GameConfig.useAudio = _toggleAudio.isOn;
        GameConfig.showStarAnim = _toggleAnim.isOn;
        GameConfig.language = _togglePt.isOn ? 1 : 0;
    }
}
