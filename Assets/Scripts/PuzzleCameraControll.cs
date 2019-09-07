using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCameraControll : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;    // Referência ao script controlador do player 
    [SerializeField] private GameObject _animGameObject;            // Referência ao objeto que será ativo após puzze completo
    private bool _isCoroutineExecuting;                             // Variável para rerificar se corrotina ainda está em execução

    public void StopAndChangeCamera()
    {
        // -- Expressão lambda que executa uma tarefa dentro de uma task (roda dentro do tempo de 1,5s)
        StartCoroutine(ExecuteAfterTime(1.5f, () =>
        {
            // -- Início da Task -------------------------------------------------
            // -- Ativa câmera principal
            Camera.main.enabled = true;
            // -- Ativa script controlador do player
            _playerController.enabled = true;
            // -- Retoma início do jogo
            GameManager.gameIsOn = true;
            // -- Desativa o game object cujo script está anexado
            this.gameObject.SetActive(false);
            // -- Fim da Task ----------------------------------------------------
        }));
    }

    IEnumerator ExecuteAfterTime(float time, Action task)
    {
        // -- Se a corrotina estiver sendo executada, desconsidera qualquer espera...
        if (_isCoroutineExecuting)
            yield break;
        // -- Setta execução da corrotina    
        _isCoroutineExecuting = true;
        // -- Aguarda 1,5s
        yield return new WaitForSeconds(time);
        // -- Executa a tarefa passada na expressão lambda
        task();
        // -- Setta fim da corrotina
        _isCoroutineExecuting = false;
    }

    public void ExecuteAnimGameObject()
    {
        // -- Ativa a animação do objeto que será liberado após puzzle completo
        _animGameObject.SetActive(true);
    }
}
