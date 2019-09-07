using UnityEngine;
using System;

//
//A camera "OlharParaOPlayer" fica olhando para o objeto que contém o script. Não necessita estar filiada ao jogador
//A camera "primeiraPessoa" funciona como um MouseLook, e necessita estar filiada ao jogador
//A camera "SeguirPlayer" segue o objeto que contém o script.
//A camera "orbital" orbita o objeto que contém o script
//A camera "parada" não possui função alguma além de ser ativada ou desativada. Pode estar em qualquer lugar
//A camera "paradaRetilinea" fica sempre na mesma posição e rotação, mas mantém o horizonte sempre reto. Ideal para visão de veículos.
//A camera "OrbitalQueSeque" é uma camera orbital que assume a função de seguir o player caso não exista nenhum input do Mouse
//A camera "CameraEstiloETS" é uma câmera em primeira pessoa que funciona como um MouseLook, mas se desloca para fora do veículo quando olhamos para a esquerda.
//

[Serializable]
public class TipoDeCam
{
    public Camera _camera;
    public enum TipoRotac { OlharParaOPlayer, primeiraPessoa, SeguirPlayer, orbital, parada, paradaRetilinea, OrbitalQueSegue, CameraEstiloETS }
    public TipoRotac TipoDeRotacao = TipoRotac.OlharParaOPlayer;
    [Range(0.0f, 1.0f)] public float volume = 1;
}
[Serializable]
public class ConfigsCamera
{
    public KeyCode TeclaTrocarDeCamera = KeyCode.C;
    [Range(1, 20)] public float sensibilidade = 10.0f;
    [Range(0, 360)] public float anguloHorizntal = 65.0f;
    [Range(0, 85)] public float anguloVertical = 20.0f;
    [Range(1, 20)] public float velocidadeCameraSeguir = 5.0f;
    [Range(1, 30)] public float velocidadeCameraGirar = 15.0f;
    [Range(0.5f, 3.0f)] public float deslocamentoCameraETS = 2.0f;
    public ConfigsCameraOrbital cameraOrbital;
}
[Serializable]
public class ConfigsCameraOrbital
{
    [Range(0.01f, 2.0f)] public float sensibilidade = 0.8f, velocidadeScrool = 1.0f, velocidadeY = 0.5f;
    [Range(3.0f, 20.0f)] public float distanciaMinima = 5.0f;
    [Range(20.0f, 1000.0f)] public float distanciaMaxima = 50.0f;
}

public class Cameras : MonoBehaviour
{

    public TipoDeCam[] cameras;
    public ConfigsCamera ConfiguracoesCameras;

    private int indiceCameras = 0;
    private float rotacaoX = 0.0f, rotacaoY = 0.0f, tempoOrbit = 0.0f, rotacaoXETS = 0.0f, rotacaoYETS = 0.0f;
    private GameObject[] objetosPosicCamerasParadasRetilineas;
    private Quaternion[] rotacaoOriginalCameras;
    private GameObject[] posicaoOriginalCameras;
    private float[] xOrbit, yOrbit, distanciaCameraOrbit;
    private bool orbitalAtiv;
    Vector3[] posicOriginalCameraETS;
    void Awake()
    {
        objetosPosicCamerasParadasRetilineas = new GameObject[cameras.Length];
        rotacaoOriginalCameras = new Quaternion[cameras.Length];
        posicaoOriginalCameras = new GameObject[cameras.Length];
        posicOriginalCameraETS = new Vector3[cameras.Length];
        xOrbit = new float[cameras.Length];
        yOrbit = new float[cameras.Length];
        distanciaCameraOrbit = new float[cameras.Length];
        for (int x = 0; x < cameras.Length; x++)
        {
            distanciaCameraOrbit[x] = ConfiguracoesCameras.cameraOrbital.distanciaMinima;
            if (cameras[x]._camera == null)
            {
                Debug.LogError("Não existe nenhuma camera linkada no indice  " + x);
            }
            if (cameras[x].TipoDeRotacao == TipoDeCam.TipoRotac.paradaRetilinea)
            {
                objetosPosicCamerasParadasRetilineas[x] = new GameObject("posicaoDaCameraParadaRetilinea" + x);
                objetosPosicCamerasParadasRetilineas[x].transform.parent = cameras[x]._camera.transform;
                objetosPosicCamerasParadasRetilineas[x].transform.localPosition = new Vector3(0, 0, 1.0f);
                objetosPosicCamerasParadasRetilineas[x].transform.parent = transform;
            }
            if (cameras[x].TipoDeRotacao == TipoDeCam.TipoRotac.primeiraPessoa)
            {
                rotacaoOriginalCameras[x] = cameras[x]._camera.transform.localRotation;
            }
            if (cameras[x].TipoDeRotacao == TipoDeCam.TipoRotac.SeguirPlayer)
            {
                posicaoOriginalCameras[x] = new GameObject("posicaoDaCameraSeguir" + x);
                posicaoOriginalCameras[x].transform.parent = transform;
                posicaoOriginalCameras[x].transform.localPosition = cameras[x]._camera.transform.localPosition;
                transform.gameObject.layer = 2;
                foreach (Transform trans in this.gameObject.GetComponentsInChildren<Transform>(true))
                {
                    trans.gameObject.layer = 2;
                }
            }
            if (cameras[x].TipoDeRotacao == TipoDeCam.TipoRotac.orbital)
            {
                xOrbit[x] = cameras[x]._camera.transform.eulerAngles.x;
                yOrbit[x] = cameras[x]._camera.transform.eulerAngles.y;
                transform.gameObject.layer = 2;
                foreach (Transform trans in this.gameObject.GetComponentsInChildren<Transform>(true))
                {
                    trans.gameObject.layer = 2;
                }
            }
            if (cameras[x].TipoDeRotacao == TipoDeCam.TipoRotac.OrbitalQueSegue)
            {
                xOrbit[x] = cameras[x]._camera.transform.eulerAngles.x;
                yOrbit[x] = cameras[x]._camera.transform.eulerAngles.y;
                //
                posicaoOriginalCameras[x] = new GameObject("posicaoDaCameraSeguir" + x);
                posicaoOriginalCameras[x].transform.parent = transform;
                posicaoOriginalCameras[x].transform.localPosition = cameras[x]._camera.transform.localPosition;
                //
                transform.gameObject.layer = 2;
                foreach (Transform trans in this.gameObject.GetComponentsInChildren<Transform>(true))
                {
                    trans.gameObject.layer = 2;
                }
            }
            if (cameras[x].TipoDeRotacao == TipoDeCam.TipoRotac.CameraEstiloETS)
            {
                rotacaoOriginalCameras[x] = cameras[x]._camera.transform.localRotation;
                posicOriginalCameraETS[x] = cameras[x]._camera.transform.localPosition;
            }
            AudioListener captadorDeSom = cameras[x]._camera.GetComponent<AudioListener>();
            if (captadorDeSom == null)
            {
                cameras[x]._camera.transform.gameObject.AddComponent(typeof(AudioListener));
            }
        }
    }

    void Start()
    {
        AtivarCameras(indiceCameras);
    }

    void AtivarCameras(int indicePedido)
    {
        if (cameras.Length > 0)
        {
            for (int x = 0; x < cameras.Length; x++)
            {
                if (x == indicePedido)
                {
                    cameras[x]._camera.gameObject.SetActive(true);
                }
                else
                {
                    cameras[x]._camera.gameObject.SetActive(false);
                }
            }
        }
    }

    void GerenciarCameras()
    {
        for (int x = 0; x < cameras.Length; x++)
        {
            if (cameras[x].TipoDeRotacao == TipoDeCam.TipoRotac.SeguirPlayer)
            {
                if (cameras[x]._camera.isActiveAndEnabled)
                {
                    cameras[x]._camera.transform.parent = null;
                }
                else
                {
                    cameras[x]._camera.transform.parent = transform;
                }
            }
        }
        //
        AudioListener.volume = cameras[indiceCameras].volume;
        //
        float velocidadeTimeScale = 1.0f / Time.timeScale;
        switch (cameras[indiceCameras].TipoDeRotacao)
        {
            case TipoDeCam.TipoRotac.parada:
                //camera parada, nao faz nada
                break;
            case TipoDeCam.TipoRotac.paradaRetilinea:
                var newRotationDest = Quaternion.LookRotation(objetosPosicCamerasParadasRetilineas[indiceCameras].transform.position - cameras[indiceCameras]._camera.transform.position, Vector3.up);
                cameras[indiceCameras]._camera.transform.rotation = Quaternion.Slerp(cameras[indiceCameras]._camera.transform.rotation, newRotationDest, Time.deltaTime * 15.0f);
                break;
            case TipoDeCam.TipoRotac.OlharParaOPlayer:
                cameras[indiceCameras]._camera.transform.LookAt(transform.position);
                break;
            case TipoDeCam.TipoRotac.primeiraPessoa:
                rotacaoX += Input.GetAxis("Mouse X") * ConfiguracoesCameras.sensibilidade;
                rotacaoY += Input.GetAxis("Mouse Y") * ConfiguracoesCameras.sensibilidade;
                rotacaoX = ClampAngle(rotacaoX, -ConfiguracoesCameras.anguloHorizntal, ConfiguracoesCameras.anguloHorizntal);
                rotacaoY = ClampAngle(rotacaoY, -ConfiguracoesCameras.anguloVertical, ConfiguracoesCameras.anguloVertical);
                Quaternion xQuaternion = Quaternion.AngleAxis(rotacaoX, Vector3.up);
                Quaternion yQuaternion = Quaternion.AngleAxis(rotacaoY, -Vector3.right);
                Quaternion rotacFinal = rotacaoOriginalCameras[indiceCameras] * xQuaternion * yQuaternion;
                cameras[indiceCameras]._camera.transform.localRotation = Quaternion.Lerp(cameras[indiceCameras]._camera.transform.localRotation, rotacFinal, Time.deltaTime * 10.0f * velocidadeTimeScale);
                break;
            case TipoDeCam.TipoRotac.SeguirPlayer:
                //camera seguir
                RaycastHit hit;
                float velocidade = ConfiguracoesCameras.velocidadeCameraSeguir;
                if (!Physics.Linecast(transform.position, posicaoOriginalCameras[indiceCameras].transform.position))
                {
                    cameras[indiceCameras]._camera.transform.position = Vector3.Lerp(cameras[indiceCameras]._camera.transform.position, posicaoOriginalCameras[indiceCameras].transform.position, Time.deltaTime * velocidade);
                }
                else if (Physics.Linecast(transform.position, posicaoOriginalCameras[indiceCameras].transform.position, out hit))
                {
                    cameras[indiceCameras]._camera.transform.position = Vector3.Lerp(cameras[indiceCameras]._camera.transform.position, hit.point, Time.deltaTime * velocidade);
                }
                //camera rodar 
                float velocidadeGir = ConfiguracoesCameras.velocidadeCameraGirar;
                var newRotation = Quaternion.LookRotation(transform.position - cameras[indiceCameras]._camera.transform.position, Vector3.up);
                cameras[indiceCameras]._camera.transform.rotation = Quaternion.Slerp(cameras[indiceCameras]._camera.transform.rotation, newRotation, Time.deltaTime * velocidadeGir);
                break;
            case TipoDeCam.TipoRotac.orbital:
                float sensibilidade = ConfiguracoesCameras.cameraOrbital.sensibilidade;
                float distMin = ConfiguracoesCameras.cameraOrbital.distanciaMinima;
                float distMax = ConfiguracoesCameras.cameraOrbital.distanciaMaxima;
                float velocidadeScrool = ConfiguracoesCameras.cameraOrbital.velocidadeScrool * 50.0f;
                float sensYMouse = ConfiguracoesCameras.cameraOrbital.velocidadeY * 10.0f;
                //
                RaycastHit hit2;
                if (!Physics.Linecast(transform.position, cameras[indiceCameras]._camera.transform.position))
                {

                }
                else if (Physics.Linecast(transform.position, cameras[indiceCameras]._camera.transform.position, out hit2))
                {
                    distanciaCameraOrbit[indiceCameras] = Vector3.Distance(transform.position, hit2.point);
                    distMin = Mathf.Clamp((Vector3.Distance(transform.position, hit2.point)), distMin * 0.5f, distMax);
                }
                //
                xOrbit[indiceCameras] += Input.GetAxis("Mouse X") * (sensibilidade * distanciaCameraOrbit[indiceCameras]) / (distanciaCameraOrbit[indiceCameras] * 0.5f);
                yOrbit[indiceCameras] -= Input.GetAxis("Mouse Y") * sensibilidade * sensYMouse;
                yOrbit[indiceCameras] = ClampAngle(yOrbit[indiceCameras], 0.0f, 85.0f);
                Quaternion rotation = Quaternion.Euler(yOrbit[indiceCameras], xOrbit[indiceCameras], 0);
                distanciaCameraOrbit[indiceCameras] = Mathf.Clamp(distanciaCameraOrbit[indiceCameras] - Input.GetAxis("Mouse ScrollWheel") * velocidadeScrool, distMin, distMax);
                Vector3 negDistance = new Vector3(0.0f, 0.0f, -distanciaCameraOrbit[indiceCameras]);
                Vector3 position = rotation * negDistance + transform.position;
                Vector3 posicAtual = cameras[indiceCameras]._camera.transform.position;
                Quaternion rotacAtual = cameras[indiceCameras]._camera.transform.rotation;
                cameras[indiceCameras]._camera.transform.rotation = Quaternion.Lerp(rotacAtual, rotation, Time.deltaTime * 5.0f * velocidadeTimeScale);
                cameras[indiceCameras]._camera.transform.position = Vector3.Lerp(posicAtual, position, Time.deltaTime * 5.0f * velocidadeTimeScale);
                break;
            case TipoDeCam.TipoRotac.OrbitalQueSegue:
                float movX = Input.GetAxis("Mouse X");
                float movY = Input.GetAxis("Mouse Y");
                float movZ = Input.GetAxis("Mouse ScrollWheel");

                if (movX > 0.0f || movY > 0.0f || movZ > 0.0f)
                {
                    orbitalAtiv = true;
                    tempoOrbit = 0.0f;
                }
                else
                {
                    tempoOrbit += Time.deltaTime;
                }
                if (tempoOrbit > 3.0f)
                {
                    tempoOrbit = 3.1f;
                    orbitalAtiv = false;
                }
                if (orbitalAtiv == true)
                {
                    float _sensibilidade = ConfiguracoesCameras.cameraOrbital.sensibilidade;
                    float _distMin = ConfiguracoesCameras.cameraOrbital.distanciaMinima;
                    float _distMax = ConfiguracoesCameras.cameraOrbital.distanciaMaxima;
                    float _velocidadeScrool = ConfiguracoesCameras.cameraOrbital.velocidadeScrool * 50.0f;
                    float _sensYMouse = ConfiguracoesCameras.cameraOrbital.velocidadeY * 10.0f;
                    //
                    RaycastHit _hit;
                    if (!Physics.Linecast(transform.position, cameras[indiceCameras]._camera.transform.position))
                    {

                    }
                    else if (Physics.Linecast(transform.position, cameras[indiceCameras]._camera.transform.position, out _hit))
                    {
                        distanciaCameraOrbit[indiceCameras] = Vector3.Distance(transform.position, _hit.point);
                        _distMin = Mathf.Clamp((Vector3.Distance(transform.position, _hit.point)), _distMin * 0.5f, _distMax);
                    }
                    //
                    xOrbit[indiceCameras] += movX * (_sensibilidade * distanciaCameraOrbit[indiceCameras]) / (distanciaCameraOrbit[indiceCameras] * 0.5f);
                    yOrbit[indiceCameras] -= movY * _sensibilidade * _sensYMouse;
                    yOrbit[indiceCameras] = ClampAngle(yOrbit[indiceCameras], 0.0f, 85.0f);
                    Quaternion _rotation = Quaternion.Euler(yOrbit[indiceCameras], xOrbit[indiceCameras], 0);
                    distanciaCameraOrbit[indiceCameras] = Mathf.Clamp(distanciaCameraOrbit[indiceCameras] - movZ * _velocidadeScrool, _distMin, _distMax);
                    Vector3 _negDistance = new Vector3(0.0f, 0.0f, -distanciaCameraOrbit[indiceCameras]);
                    Vector3 _position = _rotation * _negDistance + transform.position;
                    Vector3 _posicAtual = cameras[indiceCameras]._camera.transform.position;
                    Quaternion _rotacAtual = cameras[indiceCameras]._camera.transform.rotation;
                    cameras[indiceCameras]._camera.transform.rotation = Quaternion.Lerp(_rotacAtual, _rotation, Time.deltaTime * 5.0f * velocidadeTimeScale);
                    cameras[indiceCameras]._camera.transform.position = Vector3.Lerp(_posicAtual, _position, Time.deltaTime * 5.0f * velocidadeTimeScale);
                }
                else
                {
                    RaycastHit __hit;
                    float __velocidade = ConfiguracoesCameras.velocidadeCameraSeguir;
                    if (!Physics.Linecast(transform.position, posicaoOriginalCameras[indiceCameras].transform.position))
                    {
                        cameras[indiceCameras]._camera.transform.position = Vector3.Lerp(cameras[indiceCameras]._camera.transform.position, posicaoOriginalCameras[indiceCameras].transform.position, Time.deltaTime * __velocidade);
                    }
                    else if (Physics.Linecast(transform.position, posicaoOriginalCameras[indiceCameras].transform.position, out __hit))
                    {
                        cameras[indiceCameras]._camera.transform.position = Vector3.Lerp(cameras[indiceCameras]._camera.transform.position, __hit.point, Time.deltaTime * __velocidade);
                    }
                    //camera rodar 
                    float __velocidadeGir = ConfiguracoesCameras.velocidadeCameraGirar;
                    var __newRotation = Quaternion.LookRotation(transform.position - cameras[indiceCameras]._camera.transform.position, Vector3.up);
                    cameras[indiceCameras]._camera.transform.rotation = Quaternion.Slerp(cameras[indiceCameras]._camera.transform.rotation, __newRotation, Time.deltaTime * __velocidadeGir);
                }
                break;
            case TipoDeCam.TipoRotac.CameraEstiloETS:
                rotacaoXETS += Input.GetAxis("Mouse X") * ConfiguracoesCameras.sensibilidade;
                rotacaoYETS += Input.GetAxis("Mouse Y") * ConfiguracoesCameras.sensibilidade;
                Vector3 novaPosicao = new Vector3(posicOriginalCameraETS[indiceCameras].x + Mathf.Clamp(rotacaoXETS / 50 + (ConfiguracoesCameras.deslocamentoCameraETS / 3.0f), -ConfiguracoesCameras.deslocamentoCameraETS, 0), posicOriginalCameraETS[indiceCameras].y, posicOriginalCameraETS[indiceCameras].z);
                cameras[indiceCameras]._camera.transform.localPosition = Vector3.Lerp(cameras[indiceCameras]._camera.transform.localPosition, novaPosicao, Time.deltaTime * 10.0f);
                rotacaoXETS = ClampAngle(rotacaoXETS, -180, 80);
                rotacaoYETS = ClampAngle(rotacaoYETS, -60, 60);
                Quaternion _xQuaternionETS = Quaternion.AngleAxis(rotacaoXETS, Vector3.up);
                Quaternion _yQuaternionETS = Quaternion.AngleAxis(rotacaoYETS, -Vector3.right);
                Quaternion _rotacFinalETS = rotacaoOriginalCameras[indiceCameras] * _xQuaternionETS * _yQuaternionETS;
                cameras[indiceCameras]._camera.transform.localRotation = Quaternion.Lerp(cameras[indiceCameras]._camera.transform.localRotation, _rotacFinalETS, Time.deltaTime * 10.0f * velocidadeTimeScale);
                break;
        }
    }

    public static float ClampAngle(float angulo, float min, float max)
    {
        if (angulo < -360F) { angulo += 360F; }
        if (angulo > 360F) { angulo -= 360F; }
        return Mathf.Clamp(angulo, min, max);
    }

    void Update()
    {
        if (Input.GetKeyDown(ConfiguracoesCameras.TeclaTrocarDeCamera) && indiceCameras < (cameras.Length - 1))
        {
            indiceCameras++;
            AtivarCameras(indiceCameras);
        }
        else if (Input.GetKeyDown(ConfiguracoesCameras.TeclaTrocarDeCamera) && indiceCameras >= (cameras.Length - 1))
        {
            indiceCameras = 0;
            AtivarCameras(indiceCameras);
        }
    }

    void LateUpdate()
    {
        if (cameras.Length > 0)
        {
            GerenciarCameras();
        }
    }
}