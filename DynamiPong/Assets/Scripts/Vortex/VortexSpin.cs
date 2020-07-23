using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VortexSpin : MonoBehaviour
{
    PointEffector2D pointEffector;
    ParticleSystem particleSystem;


    // Start called at the start
    protected void Start()
    {
        pointEffector = GetComponent<PointEffector2D>();
        particleSystem = GetComponent<ParticleSystem>();
    }
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, 50 * Time.deltaTime); //rotates 50 degrees per second around z axis


        // Chance based logic
        float rand = UnityEngine.Random.value;
        if (rand < 0.001)
        {
            // Small chance to enable boost
            pointEffector.forceMagnitude *= -1;
            particleSystem.startSpeed *= -1;
        }
        
        


    }
}
