using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblesControll : MonoBehaviour
{
    public ParticleSystem explosionParticles;         // Reference to the particles that will play on explosion.
    public AudioSource audioSource;                   // Reference to the audio that will play on explosion.

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;

        // -- Se o player encostou no objeto...
        if (tag == "Player")
        {
            
            Debug.Log("Pegou uma " + this.gameObject.tag);

            // -- O objeto será destruído, portanto, para que a partícula e o audio sejam ativados, o parentesco deve ser quebrado.
            explosionParticles.transform.parent = null;

            // Play the particle system.
            explosionParticles.Play();

            if (GameConfig.useAudio)
            {
                audioSource.Play();
            }

            // Once the particles have finished, destroy the gameobject they are on.
            //Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.duration);
            ParticleSystem.MainModule mainModule = explosionParticles.main;
            Destroy(explosionParticles.gameObject, mainModule.duration);

            // Destroy the object.
            Destroy(this.gameObject);
        }

    }
}
