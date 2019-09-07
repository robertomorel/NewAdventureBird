using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    
    public static SaveManager Instance { set; get; }    // Refarência a própria instancia
    public GameConfig state;                            //  

    private void Awake()
    {
        // -- Faz com que o gameobject anexado ao script não seja destruído entre as cenas
        DontDestroyOnLoad(this.gameObject);
        // -- Setta a instância
        Instance = this;
        // -- Chama método Load
        Load();

        Debug.Log("-- After Inicial Load -- " + Helper.Serialize<GameConfig>(state));
    }

    // -- Salva todo o estado do GameConfig para o player pref 
    public void Save()
    {
        PlayerPrefs.SetString("save", Helper.Serialize<GameConfig>(state));
        Debug.Log("-- After Save -- " + Helper.Serialize<GameConfig>(state));
    }

    // -- Carrega o estado prévio do GameConfig a partir do player pref
    public void Load()
    {
        // -- Se já existe algo salvo...
        if (PlayerPrefs.HasKey("save"))
        {
            state = Helper.Deserialize<GameConfig>(PlayerPrefs.GetString("save"));
        } else
        {
            state = new GameConfig();
            Save();
            Debug.Log("Nenhum arquivo encontrado, criando um novo.");
        }
    }
}
