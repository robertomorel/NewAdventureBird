using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    public Text m_Text;   // Referência ao texto que será carregado

    public IEnumerator LoadSceneCroutine(string sceneName)
    {
        // -- Cria texto inicial do load
        m_Text.text = "Loading: 0%";

        yield return null;

        // -- Inicia o carregamento da cena pré definida
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        m_Text.text = "Loading: " + (asyncOperation.progress * 100) + "%";
        
        //When the load is still in progress, output the Text and progress bar
        while (!asyncOperation.isDone)
        {
            //Output the current progress
            m_Text.text = "Loading: " + (asyncOperation.progress * 100) + "%";
            yield return null;
        }
    }

}
