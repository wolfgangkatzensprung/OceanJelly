using UnityEngine;

public class ArenaTrigger : MonoBehaviour
{
    int arenaNr;

    void Start()
    {
        // getScene
        // je nach Scene dann arenaNr
        arenaNr = 1; // solange es nur eine Scene gibt -> danach DEL
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (arenaNr)
        {
            case 1: // Banana Farm Arena

                // greife auf Arena 1 Script zu, wo die waves mit koordinaten etc durch Instantiate() instanziert werden
                // jede arena kriegt n eigenes script wo jede wave ne eigene Funktion ist die mit SpawnWave1() etc getriggert wird
                break;

        }
    }
}
