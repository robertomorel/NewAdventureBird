using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{

    #region privates_fields
    private CharacterController _characterController;        // Referência ao componente CharacterController 
    private float _speed = 3.0f;                             // Velocidade de caminhada do player 
    private float _jump = 4.0f;                              // Valor aplicado ao pulo
    private float _gravity = 9.8f;                           // Valor aplicado à gravidade
    private Vector3 _vectorDirection = new Vector3(0, 0, 0); // Vetor utilizado para aplicação da gravidade. Puxo para baixo. 
    #endregion

    #region privates_fields_serializebles
    [SerializeField]
    private Transform finalPosition;                         // Guarda referência da posição final onde o player ficará 
    [SerializeField]
    private Transform lookAttPosition;                       // Guarda referência da posição final onde o player olhará
    [SerializeField]
    private ParticleSystem _particles;                       // Guarda referência da partícula aplicada sempre que o player pular
    #endregion

    #region public_fields
    public Animation playerAnimation;                        // Referência ao componente Animation do player
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // -- Olha para a posição onde deverá atingir
        transform.LookAt(finalPosition);
        // -- Chama o método Jump a cada 10s
        InvokeRepeating("Jump", 8.0f, 10.0f);
        // -- Guarda componente CharacterController
        _characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        // -- Se player não está executando a animação do JUMP e está no chão...
        if (!playerAnimation.IsPlaying("JUMP") && _characterController.isGrounded)
        {
            // -- Calcula a distância entre ele e o alvo 
            Vector3 offset = finalPosition.position - transform.position;

            // -- Se a distância for aior que 0.15f...
            if (offset.magnitude > .15f)
            {
                // -- Chama método para mover o player utilizando o _characterController
                MoveTowardsTarget(offset);
                // -- Ativa a animação WALK
                playerAnimation.Play("WALK");
            }
            // -- Se já chegou à posição marcada
            else
            {
                // -- Ativa a animação IDLE
                playerAnimation.Play("IDLE");
                // -- Olha para o destino final
                transform.LookAt(lookAttPosition);
            }
        }

        // -- Aplica valor negativo em y x tempo (baixo)
        _vectorDirection.y -= _gravity * Time.deltaTime;
        // -- Força movimento do personagem para baixo na constância do tempo
        _characterController.Move(_vectorDirection * Time.deltaTime);
    }

    void MoveTowardsTarget(Vector3 offset)
    {
        // -- Normaliza o valor da distância entre personagem e pivot e multiplica pela velocidade aplicada
        offset = offset.normalized * _speed;
        // -- Move o personagem por fator de tempo
        _characterController.Move(offset * Time.deltaTime);
    }

    void Jump()
    {
        // -- Aplica valor positivo em y (cima)
        _vectorDirection.y = _jump;
        // -- Ativa a animação JUMP
        playerAnimation.Play("JUMP");
        // -- Ativa a partícula das penas
        _particles.Play();
    }

}
