using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryVisualization : MonoBehaviour
{
    private new ParticleSystem particleSystem;
    private ParticleSystem.EmissionModule m_emission;
    private ParticleSystem.MainModule main;
    private bool animationFinished;
    private bool gateReleased;

    private void Start()
    {
        particleSystem = gameObject.GetComponent<ParticleSystem>();

        main = particleSystem.main;
        main.startRotation3D = true;
        // Unity doesn't offer what we need, so we need to change
        // the start rotation constantly while keeping world alignment.
        gameObject.GetComponent<ParticleSystemRenderer>().alignment = ParticleSystemRenderSpace.World;

        m_emission = particleSystem.emission;
        m_emission.enabled = false;
    }

    /// <summary>
    /// Set the rotation of the main particle system, which is what emits the particles.
    /// </summary>
    private void FixedUpdate()
    {
        main.startRotationX = transform.eulerAngles.x * Mathf.Deg2Rad;
        main.startRotationY = transform.eulerAngles.y * Mathf.Deg2Rad;
        main.startRotationZ = transform.eulerAngles.z * Mathf.Deg2Rad;
    }
    
    /// <summary>
    /// Enable the particle system to start emitting particles.
    /// </summary>
    public void EnableEmissions()
    {
        m_emission.enabled = true;
        animationFinished = false;
        gateReleased = false;
    }

    /// <summary>
    /// Disable the particle system from emitting particles.
    /// </summary>
    public void DisableEmissions()
    {
        m_emission.enabled = false;
        AttemptToDespawnParticles("AnimationFinished");
    }

    /// <summary>
    /// Disable the particle system from emitting particles and clear all current particles instantly.
    /// </summary>
    public void DisableAndClearEmissions()
    {
        m_emission.enabled = false;
        particleSystem.Clear();
    }

    /// <summary>
    /// Attempt to despawn particles based on two flag conditions. Only despawns if both are met.
    /// </summary>
    /// <param name="flagName"></param>
    public void AttemptToDespawnParticles(string flagName)
    { 
        if (flagName == "AnimationFinished")
            animationFinished = true;
        else if (flagName == "GateReleased")
            gateReleased = true;

        if (animationFinished && gateReleased)
        {
            animationFinished = false;
            gateReleased = false;
            DespawnParticles();
        }    
    }

    /// <summary>
    /// Despawn particles in the same fashion they spawned.
    /// </summary>
    private void DespawnParticles()
    {
        // Create container for particles and retrieve from particle system
        int numParticles = this.particleSystem.particleCount;
        ParticleSystem.Particle[] emittedParticles = new ParticleSystem.Particle[numParticles];
        particleSystem.GetParticles(emittedParticles);

        // Ensure that the particles actually exist
        if (numParticles < 1)
        {
            Debug.LogError("Attempted to despawn particles that dont exist");
            return;
        }

        // Edit the remainingLifetime of each particle so that all particles despawn in a FIFO manner
        float lowestLifetime = 0;
        lowestLifetime = emittedParticles[0].remainingLifetime;
        for (int n = 0; n < numParticles; n++)
            emittedParticles[n].remainingLifetime -= lowestLifetime;

        // Assign particles back to particle system
        particleSystem.SetParticles(emittedParticles, emittedParticles.Length);
    }
}