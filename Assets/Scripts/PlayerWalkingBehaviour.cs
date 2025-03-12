using UnityEngine;

public class PlayerWalkingBehaviour : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SpawnWalkBlubberParticles(animator);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ClearWalkBlubberParticles();
    }

    private static void SpawnWalkBlubberParticles(Animator animator)
    {
        Vector3 particleSpawnPos = animator.transform.position;
        ParticleManager.Instance.SpawnParticles("WalkBlubber", particleSpawnPos, Quaternion.identity, true);
        ParticleManager.Instance.SpawnParticles("WalkBlubberBurst", particleSpawnPos, Quaternion.identity);
    }

    private static void ClearWalkBlubberParticles()
    {
        ParticleManager.Instance.ClearParticles("WalkBlubber", true);
    }
}
