using UnityEngine;

public class Epilepsie : MonoBehaviour
{

    SpriteRenderer m_SpriteRenderer;
    Color m_NewColor;

    float m_Red, m_Blue, m_Green;

    // Use this for initialization
    void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_SpriteRenderer.color = Color.blue;
        m_SpriteRenderer.color = Color.red;


    }

    // Update is called once per frame
    void Update()
    {
        if (m_SpriteRenderer.color == Color.blue)
        {
            m_SpriteRenderer.color = Color.red;
        }
        else if (m_SpriteRenderer.color == Color.red)
        {
            m_SpriteRenderer.color = Color.green;
        }
        else if (m_SpriteRenderer.color == Color.green)
        {
            m_SpriteRenderer.color = Color.white;
        }
        else if (m_SpriteRenderer.color == Color.white)
        {
            m_SpriteRenderer.color = Color.blue;
        }
    }
}
