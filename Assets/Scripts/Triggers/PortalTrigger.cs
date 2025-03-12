using UnityEngine;
public class PortalTrigger : MonoBehaviour
{
    [Tooltip("Target Scene Name")]
    [SerializeField]
    string targetLocation;

    //[Tooltip("Where Player spawns in the target scene")]
    //public Transform spawnPosition;
    //[Tooltip("Leave spawnPosition empty to use Vector spawnPos")]
    //public Vector2 spawnPos = Vector2.zero;     // alternative spawnPosition

    public SceneManagerScript.SceneType sceneType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (sceneType.Equals(SceneManagerScript.SceneType.Ocean))
            {
                SceneManagerScript.Instance.LoadScene("Ocean");
                return;
            }

            if (sceneType.Equals(SceneManagerScript.SceneType.IceCave))
            {
                UIManager.Instance.ShowDungeonPopup(SceneManagerScript.SceneType.IceCave);
            }
            else
            {
                UIManager.Instance.ShowDungeonPopup();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            UIManager.Instance.HideDungeonPopup();
        }
    }

    private void OnDisable()
    {
        UIManager.Instance.HideDungeonPopup();
    }
    //private void OnDrawGizmos()
    //{
    //    if (spawnPosition != null)
    //    {
    //            Gizmos.color = Color.yellow;
    //            Gizmos.DrawSphere(spawnPosition.position, 7f);
    //            Gizmos.color = Color.blue;
    //            Gizmos.DrawSphere(spawnPosition.position, 3f);
    //    }
    //    else if (spawnPos != null)
    //    {
    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawSphere(spawnPos, 7f);
    //        Gizmos.color = Color.blue;
    //        Gizmos.DrawSphere(spawnPos, 3f);
    //    }
    //}
}
