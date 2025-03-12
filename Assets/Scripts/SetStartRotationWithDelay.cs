using System.Collections;
using UnityEngine;

public class SetStartRotationWithDelay : MonoBehaviour
{
    public Vector3 startRotation = Vector3.zero;

    private void Start()
    {
        StartCoroutine(SetRotation());
    }

    IEnumerator SetRotation()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(.3f);
        transform.rotation = Quaternion.Euler(startRotation);
    }
}
