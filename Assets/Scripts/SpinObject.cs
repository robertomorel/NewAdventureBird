using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinObject : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
    	// -- Lógica para os objetos rodarem em y sequencialmente com o tempo
        transform.Rotate(new Vector3(0.0f, 90.0f, 0.0f) * Time.deltaTime);
    }
}
