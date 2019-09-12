using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour
{

    #region privates_fields
    private CharacterController _characterController;               // Guardará a referência do CharacterController
    private PlayerManager _playerManager;                           // Guardará a referência do script PlayerManager

    private Vector3 _direction = new Vector3(0, 0, 0);              // Direção inicial de movimentação 

    private Vector3 _moveCameraForward;                             // Guardará a referência do Vector3 para a movimentação frontal da câmera 
    private Vector3 _moveObjetc;                                    // Guardará a referência do Vector3 para a movimentação do personagem após calculo
    private Vector3 _normalZeroGround = new Vector3(0, 0, 0);       // Guardará a referência do Vector3 para a posição inicial normalizada 
    private Vector3 _vectorDirection = new Vector3(0, 0, 0);        // Guardará a referência do Vector3 para o vetor de direção principal

    private int _count = 0;                                         // Guardará o contador de "pisca" quando o player se ferir

    private float _pushPower = 2.0f;                                // Guarda a força que o player terá para empurrar objetos com rigidBody
    private float _weight = 6.0f;                                   // Guarda o peso dos objetos com rigidBody
    #endregion

    #region privates_fields_serializebles
    [SerializeField] private float _speed = 3.0f;                   // Valor fechado para a velocidade do player
    [SerializeField] private float _spin = 60.0f;                   // Valor para o giro do player
    [SerializeField] private float _gravity = 3.5f;                 // Valor fechado para a gravidade aplicada
    [SerializeField] private float _jump = 5.0f;                    // Valor fechado para o pulo do player
    [SerializeField] private float _forward = 3.0f;                 // Valor fechado para a velocidade de impulso do player

    [SerializeField]
    private ParticleSystem _particles;                              // Guarda referência da partícula aplicada sempre que o player pular
    #endregion

    #region public_fields
    public GameObject player;                                       // Guardará a referência ao prefab do player
    public Animation playerAnimation;                               // Guardará a referência à animação do player
    #endregion

    void Awake()
    {
        // -- Preenche referências
        _characterController = GetComponent<CharacterController>();
        _playerManager = GetComponent<PlayerManager>();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.gameIsOn || GameManager.gameOver || GameManager.gameWin)
        {
            // -- Se player está no chão...
            //if (_characterController.isGrounded)
            //{
            // -- Ativa a animação "IDLE"
            playerAnimation.Play("IDLE");
            //}
            return;
        }

        float inputVertical, inputHorizontal;
        bool buttonJump;

#if UNITY_ANDROID
        inputVertical = MSJoystickController.joystickInput.y;
        inputHorizontal = MSJoystickController.joystickInput.x;
        buttonJump = CrossPlatformInputManager.GetButtonDown("Jump");
#elif UNITY_IOS
        Debug.log("It´s running in IOS platform");
#else
        inputVertical = Input.GetAxis("Vertical");
        inputHorizontal = Input.GetAxis("Horizontal");
        buttonJump = Input.GetButton("Jump");
#endif

        //inputVertical = Input.GetAxis("Vertical");
        //inputHorizontal = Input.GetAxis("Horizontal");
        //buttonJump = CrossPlatformInputManager.GetButtonDown("Jump");//Input.GetButtonDown("Fire1");

        /*
        if (Input.mousePosition.x <= Screen.width / 2)
        {
            buttonJump = false;
        }
        */

        // -- Armazena o transform da câmera
        Transform _transformCamera = Camera.main.transform;

        // -- Multiplica o vetor de posição da câmera com o Vector3(1, 0, 1) e normaliza
        // -- Isso condiciona o movimento apenas nos vetores X e Z
        _moveCameraForward = Vector3.Scale(_transformCamera.forward, new Vector3(1, 0, 1)).normalized;
        // -- Calcula o vetor de movimentação do personagem de acordo com os inputs dados
        _moveObjetc = inputVertical * _moveCameraForward + inputHorizontal * _transformCamera.right;

        // -- Ajusta direção Y para aplicar gravidade
        _vectorDirection.y -= _gravity * Time.deltaTime;

        // -- Move de fato o personagem de acordo com o vetor calculado
        _characterController.Move(_vectorDirection * Time.deltaTime);
        // -- Move de fato o personagem para baixo, aplicando a gravidade
        _characterController.Move(_moveObjetc * _speed * Time.deltaTime);

        // -- Enquanto houve movimento, ou seja, o vetor tiver valores...
        if (_moveObjetc.magnitude > 1.0f)
        {
            // -- Vetor é normalizado
            _moveObjetc.Normalize();
        }

        // -- Transforma a direção (vetor) de World Space para Local Space
        _moveObjetc = transform.InverseTransformDirection(_moveObjetc);
        // -- Projeta vetor em um plano definido pelo _normalZeroGround
        _moveObjetc = Vector3.ProjectOnPlane(_moveObjetc, _normalZeroGround);

        // -- Calcula angulo para giro do personagem
        _spin = Mathf.Atan2(_moveObjetc.x, _moveObjetc.z);

        // -- Marca a posição frontal
        _forward = _moveObjetc.z;

        // -- Move personagem para baixo, segundo gravidade
        _characterController.SimpleMove(Physics.gravity);

        // -- Aplica o giro
        ApplyRotation();

        // -- Se pressionado o botão de pular... 
        if (buttonJump)
        {
            // -- Se player está no chão...
            if (_characterController.isGrounded)
            {
                // -- Ajusta direção Y para aplicar o pulo
                _vectorDirection.y = _jump;
                // -- Ativa a animação "JUMP"
                playerAnimation.Play("JUMP");
                _playerManager.PlayJumpSound();
                // -- Ativa a partícula das penas
                _particles.Play();
            }
        }

        // -- Se movimento horizontal ou vertical está sendo aplicado...
        else if (inputVertical != 0 || inputHorizontal != 0)
        {
            // Se a animação "JUMP" não estiver mais rodando...
            if (!playerAnimation.IsPlaying("JUMP"))
            {
                // -- Ativa a animação "WALK"
                playerAnimation.Play("WALK");
            }
        }
        else // -- Se não existir movimento horizontal ou vertical...
        {
            // -- Se player está no chão...
            if (_characterController.isGrounded)
            {
                // -- Ativa a animação "IDLE"
                playerAnimation.Play("IDLE");
            }
        }
    }

    private void ApplyRotation()
    {
        // -- Calcula velocidade de rotação
        float turnSpeed = Mathf.Lerp(180, 360, _forward);
        // -- Rotaciona player
        transform.Rotate(0, _spin * turnSpeed * Time.deltaTime, 0);
    }

    public void PlayerHit()
    {
        _playerManager.PlayHitSound();
        // -- Chama o método "PlayerChangeState" após 0s e repete após cada 0.1s
        InvokeRepeating("PlayerChangeState", 0, 0.1f);
        // -- Volta o player 3.0f paa trás
        _characterController.Move(transform.TransformDirection(Vector3.back) * 3);
        PlayerManager.life--;
        Debug.Log("Life: " + PlayerManager.life);
    }

    void PlayerChangeState()
    {
        _count++;
        /*
         Lógica para fazer o player piscar após ser atingido.
         Vai apagando e mostrando repetidamente até 7 vezes
         */
        player.SetActive(!player.activeInHierarchy);
        if (_count > 7)
        {
            _count = 0;
            player.SetActive(true);
            CancelInvoke();
        }

    }
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // -- Caso o objeto colidido não tenha a tag "PuzzleBox", retorna
        if (hit.collider.tag != "PuzzleBox")
            return;

        // -- Toma o componente rigidbody vinculado 
        Rigidbody body = hit.collider.attachedRigidbody;

        // Caso não tenha rigidbody, retorna
        if (body == null || body.isKinematic)
        {
            return;
        }

        // Lógica para não enpurrar objetos abaixo
        if (hit.moveDirection.y < -0.3)
        {
            return;
        }

        // Calcula a direção do empurrão a partir  a direção acertada
        // Apenas iremos empurrar os objetos nos lados. Nunca em cima ou em baixo...
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // Se sabemos a velocidade que o player irá se mover, podemos multiplicar o empurrão por esta velocidade

        // Aplica o empurrão
        body.velocity = pushDir * _pushPower;
    }
}