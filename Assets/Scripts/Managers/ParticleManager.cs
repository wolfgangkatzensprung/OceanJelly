using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager _instance;
    public static ParticleManager Instance { get { return _instance; } }

    public GameObject particleHolder;

    // Particle Prefabs

    public ParticleSystem defaultParticles;

    [Header("Player Particles")]
    public ParticleSystem touchGroundParticles;
    public ParticleSystem jumpParticles;
    public ParticleSystem jumpHitParticles;
    public ParticleSystem extraJumpParticles;
    public ParticleSystem dashParticles;
    public ParticleSystem damagedParticles;
    public ParticleSystem sleepParticles;
    public ParticleSystem walkBlubberParticles;
    public ParticleSystem walkBlubberBurstParticles;

    [Header("Environment Particles")]
    public ParticleSystem bubbleBurstParticles;

    [Header("Entity Particles")]
    public ParticleSystem hitEffectFleckenParticles;
    public ParticleSystem deathFleckenParticles;
    public ParticleSystem deathParticles;
    public ParticleSystem circularExplosionParticles;

    [Header("Universal Particles")]
    public ParticleSystem triangleParticles;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }
    public void ClearParticles(string particlesToClearName, bool isOnPlayer)
    {
        if (isOnPlayer)
        {
            switch (particlesToClearName)
            {
                case "WalkBlubber":
                    foreach (Transform child in particleHolder.transform)
                    {
                        if (child.name.Contains("WalkBlubber"))
                        {
                            Destroy(child.gameObject);
                        }
                    }
                    break;
            }
        }
        else
        {
            // sonstige in der scene rumfliegende particles
        }
    }

    #region SpawnParticles

    public void SpawnParticles(string name, Vector2 position, Quaternion identity)
    {
        //Debug.Log("Spawning ParticleSystem " + name + ".");
        ParticleSystem particles = ChoseParticlesToSpawn(name);
        Instantiate(particles, position, identity);
    }

    public void SpawnParticles(string name, Vector2 position, Quaternion identity, bool stickToPlayer)
    {
        //Debug.Log("Spawning ParticleSystem " + name + ".");
        ParticleSystem particles = ChoseParticlesToSpawn(name);
        if (stickToPlayer)
        {
            ParticleSystem particlesOnPlayer = Instantiate(particles, position, identity);
            particlesOnPlayer.transform.parent = particleHolder.transform;
        }
        else
        {
            Instantiate(particles, position, identity);
        }
    }
    public void SpawnParticles(string name, Vector2 position, Quaternion identity, Color startColor)
    {
        //Debug.Log("Spawning ParticleSystem " + name + ".");
        ParticleSystem particles = ChoseParticlesToSpawn(name);
        ParticleSystem ps = Instantiate(particles, position, identity);
        var main = ps.main;
        main.startColor = startColor;
    }

    public void SpawnCustomParticles(GameObject particlePrefab, Vector2 position, Quaternion identity)
    {
        Debug.Log("Spawning ParticleSystem");
        Instantiate(particlePrefab, position, identity);
    }
    public GameObject SpawnCustomParticlesGet(GameObject particlePrefab, Vector2 position, Quaternion identity)
    {
        Debug.Log("Spawning ParticleSystem");
        GameObject customParticles = Instantiate(particlePrefab, position, identity);
        return customParticles;
    }

    private ParticleSystem ChoseParticlesToSpawn(string name)
    {
        ParticleSystem particles;
        switch (name)
        {
            default:
                Debug.Log("No " + name + " Particles found. Spawning default particles.");
                particles = defaultParticles;
                break;
            case "Jump":
                particles = jumpParticles;
                break;
            case "JumpHit":
            case "HitJump":
                particles = jumpHitParticles;
                break;
            case "TouchGround":
                particles = touchGroundParticles;
                break;
            case "Death":
                particles = deathParticles;
                break;
            case "WalkBlubber":
                particles = walkBlubberParticles;
                break;
            case "WalkBlubberBurst":
                particles = walkBlubberBurstParticles;
                break;
            case "BubbleBurst":
                particles = bubbleBurstParticles;
                break;
            case "DeathFlecken":
                particles = deathFleckenParticles;
                break;
            case "HitEffect":
                particles = hitEffectFleckenParticles;
                break;
            case "Triangles":
                particles = triangleParticles;
                break;
        }

        return particles;
    }

    #endregion

    public void SetSleepParticles(bool sleeps)
    {
        if (sleeps)
        {
            sleepParticles.Play();
        }
        else
        {
            sleepParticles.Stop();
        }
    }
}
