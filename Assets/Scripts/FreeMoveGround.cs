using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeMoveGround : MonoBehaviour
{
    public float speedPerTime = 10.0f;
    public float speed = 5.0f;
    public float amplitude = 1.0f;
    public float maxAmplitude = 1.0f;

    public bool moveXPosition = false;
    public bool moveYPosition = false;
    public bool moveZPosition = false;

    public bool moveXYCircle = false;
    public bool moveXZCircle = false;
    public bool moveYZCircle = false;

    public bool amplitudeIncrease = false;

    // Start is called before the first frame update
    void Start()
    {
        EraseAllPhases();
    }

    // Update is called once per frame
    void Update()
    {
        speedPerTime += Time.deltaTime * speed;

        if (moveXPosition)
        {
            EraseAllPhases();
            moveXPosition = true;
            transform.position = new Vector3(Mathf.Sin(speedPerTime) * amplitude, transform.position.y, transform.position.z);
            //transform.position = new Vector3(Mathf.Cos(speedPerTime) * amplitude, transform.position.y, transform.position.z);
        }
        else if (moveYPosition)
        {
            EraseAllPhases();
            moveYPosition = true;
            transform.position = new Vector3(transform.position.x, Mathf.Sin(speedPerTime) * amplitude, transform.position.z);
            //transform.position = new Vector3(transform.position.x, Mathf.Cos(speedPerTime) * amplitude, transform.position.z);
        }
        else if (moveZPosition)
        {
            EraseAllPhases();
            moveZPosition = true;
            transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Sin(speedPerTime) * amplitude);
            //transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Cos(speedPerTime) * amplitude);
        }
        else if (moveXYCircle)
        {
            EraseAllPhases();
            moveXYCircle = true;
            transform.position = new Vector3(Mathf.Cos(speedPerTime) * amplitude, Mathf.Sin(speedPerTime) * amplitude, transform.position.z);
        }
        else if (moveXZCircle)
        {
            EraseAllPhases();
            moveXZCircle = true;
            transform.position = new Vector3(Mathf.Cos(speedPerTime) * amplitude, transform.position.y, Mathf.Sin(speedPerTime) * amplitude);
        }
        else if (moveYZCircle)
        {
            EraseAllPhases();
            moveYZCircle = true;
            transform.position = new Vector3(transform.position.x, Mathf.Cos(speedPerTime) * amplitude, Mathf.Sin(speedPerTime) * amplitude);
        }

        if (amplitudeIncrease)
        {
            amplitude += Time.deltaTime;
        }
        else
        {
            if (amplitude > maxAmplitude)
            {
                amplitude -= Time.deltaTime;
            }
        }

    }

    void EraseAllPhases()
    {
        moveXPosition = false;
        moveYPosition = false;
        moveZPosition = false;
        moveXYCircle = false;
        moveXZCircle = false;
        moveYZCircle = false;
    }
}
