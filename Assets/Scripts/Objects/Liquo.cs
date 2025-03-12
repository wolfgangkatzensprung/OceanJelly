using UnityEngine;


public class Liquo : MonoBehaviour
{
    //public float playerUpDrive = 30;
    public Vector2 liquoForce = new Vector2();

    [Tooltip("Je niedriger Platsch ist, desto staerker wird Player beim Eintritt ins Wasser gebremst.")]
    public float platsch = .88f;

    [Tooltip("Wenn die Velocity niedriger ist als dieser Wert, findet Platsch statt")]
    public float minPlatschVelocity = -50f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement.Instance.inLiquo = true;
            PlayerLiquo.Instance.SetLiquoForce(liquoForce);
        }
        else if (collision.CompareTag("Entity"))
        {
            collision.gameObject.AddComponent<ConstantForce2D>();
            collision.gameObject.GetComponent<ConstantForce2D>().force = liquoForce;
        }
        else if (collision.CompareTag("Item"))
        {
            collision.gameObject.AddComponent<ConstantForce2D>();
            collision.gameObject.GetComponent<ConstantForce2D>().force = liquoForce * 2f;
        }
        else if (collision.CompareTag("Interactable"))
        {
            collision.gameObject.AddComponent<ConstantForce2D>();
            collision.gameObject.GetComponent<ConstantForce2D>().force = liquoForce * 2f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement.Instance.inLiquo = false;
        }
        else if (collision.CompareTag("Entity"))
        {
            Destroy(collision.gameObject.GetComponent<ConstantForce2D>());
        }
        else if (collision.CompareTag("Item"))
        {
            Destroy(collision.gameObject.GetComponent<ConstantForce2D>());
        }
        else if (collision.CompareTag("Interactable"))
        {
            Destroy(collision.gameObject.GetComponent<ConstantForce2D>());
        }
    }
}
