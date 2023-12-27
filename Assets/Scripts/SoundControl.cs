using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundControl : MonoBehaviour
{
    [SerializeField]
    AudioSource CrashSound;

    bool crashSoundReady;

    float latestRecordedMagnitude = 0;

    Rigidbody rb;

    int frameCount = 0;

    AudioSource EngineSound;
    // Start is called before the first frame update
    void Start()
    {
        EngineSound = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rb.velocity.magnitude > 40)
        {
            crashSoundReady = true;
            latestRecordedMagnitude = rb.velocity.magnitude;
            frameCount = 0;
        }
        else 
        {
            frameCount++;
            if (frameCount > 2)
            {
                crashSoundReady = false;
            }
        }

        if (rb.velocity.magnitude > 70)
        {
            EngineSound.volume = 1;
        }
        else
        {
            EngineSound.volume = rb.velocity.magnitude / 70.0f;
        }
        //Debug.Log(GetComponent<Rigidbody>().velocity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.Equals("Player") && crashSoundReady && !collision.gameObject.CompareTag("Ball"))
        {
            if (latestRecordedMagnitude > 70)
            {
                CrashSound.volume = 1;
            }
            else
            {
                CrashSound.volume = (latestRecordedMagnitude-40) / 30;
            }
            CrashSound.Play();
        }
    }

}
