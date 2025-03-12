using UnityEngine;

public class RayCaster : MonoBehaviour
{
    public ParticleSystem splatParticles;
    public GameObject splatPrefab;
    public Transform splatHolder;

    // das ist sozusaen die main methode für splat particle burst
    // todo: eingabewerte fuer custom splash sprites. muss man vllt mit mehreren particle system prefabs machen

    public void CastSplatRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

        if (hit.collider != null)
        {
            GameObject splat = Instantiate(splatPrefab, hit.point, Quaternion.identity) as GameObject;
            splat.transform.SetParent(splatHolder, true);
            Splat splatScript = splat.GetComponent<Splat>();

            splatParticles.transform.position = hit.point;
            splatParticles.Play();

            if (hit.collider.CompareTag("Background"))
            {
                splatScript.Initialize(Splat.SplatLocation.Background);
            }
            else
            {
                splatScript.Initialize(Splat.SplatLocation.Foreground);
            }
        }
    }
}
