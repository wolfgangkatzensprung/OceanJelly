using UnityEngine;

public class PlayerMoveParticles : MonoBehaviour
{
    ParticleSystem particles;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
    }

    private void OnEnable() => PlayerInputManager.Instance.onTap += PlayParticles;

    private void OnDisable() => PlayerInputManager.Instance.onTap -= PlayParticles;

    void PlayParticles(Vector2 touchPos)
    {
        particles.Play();
    }
}