using UnityEngine;

public class DamageOnCollision : MonoBehaviour
{
    public Material burnGraph;

    public int damageAmount = 1;

    float timer = 0f;
    bool timerRunning;
    public float warmupTime = 3f;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            timerRunning = true;
        }
    }

    private void Start()
    {
        burnGraph = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        if (timerRunning)
        {
            timer += Time.deltaTime;
            burnGraph.SetFloat("_Timer", timer);

            if (timer > warmupTime)
            {
                CombatManager.Instance.DealDamageToPlayer(damageAmount);
                timer = 0f;
                burnGraph.SetFloat("_Timer", 0);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            timerRunning = false;
            timer = 0f;
            burnGraph.SetFloat("_Timer", 0);
        }
    }
}
