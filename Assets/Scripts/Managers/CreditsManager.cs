using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsManager : MonoBehaviour
{
    [SerializeField] private GameObject _background;

    public void CloseCreditsScreen()
    {
        _background.SetActive(false);
        this.gameObject.SetActive(false);
    }
}
