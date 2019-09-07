using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    #region privates_fields
    private GameManager _gameManager;                              // Referência do script GameManager
    private PlayerController _playerController;                    // Referência do script PlayerController
    #endregion

    #region privates_fields_serializebles
    [SerializeField] private AudioClip _audioHit, _audioJump;      // Variáveis para armazenamento dos clipes de audio 
    #endregion

    #region publics_fields

    #endregion

    #region static_fields
    public static int life;                                        // Referência ao life do personagem       
    public static int score;                                       // Referência ao score do personagem
    #endregion

    void Awake()
    {
        // -- Setta referências
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _playerController = GetComponent<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // -- Setta life inicial do personagem para 8
        life = 8;
        // -- Setta score único da fase para o personagem em 0
        score = 0;
    }

    // -- Método chamado quando o colisor do personagem tocar em outro colisor com trigger ativo
    private void OnTriggerEnter(Collider other)
    {
        // -- Busca a tag do objeto tocado 
        string tag = other.tag;
        // -- Chama método ControllCollectiblesOnScreen, passando a tag do objeto por parâmetro 
        ControllCollectiblesOnScreen(tag);
    }

    void ControllCollectiblesOnScreen(string tag)
    {
        if (tag == "Feather")
        {
            // -- Caso seja uma pena, retirar da quantidade total existente no jogo
            _gameManager.sceneFeathersCount--;
        }
        else if (tag == "Egg")
        {
            // -- Caso seja um ovo, retirar da quantidade total existente no jogo
            _gameManager.sceneEggCount--;
        }
        else if (tag == "Coin")
        {
            // -- Caso seja uma moeda, retirar da quantidade total existente no jogo
            _gameManager.sceneCoinCount--;
            // -- Aumenta o score em +1
            score++;
        }
        else if (tag == "Fire")
        {
            // -- Caso seja o fogo, chamar método que gera dano no player
            _playerController.PlayerHit();
        }
        else if (tag == "Star")
        {
            // -- Caso seja uma estrela, aumentar o score em +10
            score += 10;
            // -- Chama método para finalização do round com vitória 
            _gameManager.RoundWon();
        }
        else if (tag == "Hole")
        {
            // -- Caso seja o buraco, settar life para 0 para, consequêntemente, finalizar o round com derrota
            life = 0;
        }
    }

    /*
     Método para tocar o clip de hit caso a configuração inicial do jogo permita
         */
    public void PlayHitSound()
    {
        if (GameConfig.useAudio)
        {
            GetComponent<AudioSource>().PlayOneShot(_audioHit, .8f);
        }
    }

    /*
     Método para tocar o clip de jump caso a configuração inicial do jogo permita
         */
    public void PlayJumpSound()
    {
        if (GameConfig.useAudio)
        {
            GetComponent<AudioSource>().PlayOneShot(_audioJump, .8f);
        }
    }
}
