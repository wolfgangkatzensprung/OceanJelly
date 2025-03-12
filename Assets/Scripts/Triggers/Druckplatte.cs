using System.Collections.Generic;
using UnityEngine;

public class Druckplatte : MonoBehaviour
{
    public DoorOpener doorOpener;

    // Settings Bools

    public bool allowPlayer;
    public bool allowFlame;
    public bool allowNautic;
    public bool allowAether;

    public bool mustTouch;
    List<GameObject> collidingObjects = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (allowPlayer)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                OpenIt(collision);
            }
        }
        if (allowFlame)
        {
            if (collision.gameObject.CompareTag("Flame"))
            {
                OpenIt(collision);
            }
        }
        if (allowNautic)
        {
            if (collision.gameObject.CompareTag("Nautic"))
            {
                OpenIt(collision);
            }
        }
        if (allowAether)
        {
            if (collision.gameObject.CompareTag("Aether"))
            {
                OpenIt(collision);
            }
        }
    }

    void OpenIt(Collider2D col)
    {
        doorOpener.GetComponent<DoorOpener>().SetOpenGate(true);
        collidingObjects.Add(col.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collidingObjects.Remove(collision.gameObject);
        if (collidingObjects.Count == 0)
            doorOpener.GetComponent<DoorOpener>().SetOpenGate(false);
    }
}
