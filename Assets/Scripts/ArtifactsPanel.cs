using UnityEngine;
using UnityEngine.UI;

public class ArtifactsPanel : MonoBehaviour
{
    public Animator anim;
    [Tooltip("Rot = 0, Blau = 1; Gelb = 2")]
    public Sprite[] artifacts;
    public Image artifactImg;

    internal void PlayGemAnim(Mermaid.Variation variation)
    {
        artifactImg.sprite = artifacts[(int)variation];
        UIManager.Instance.AddArtifact(variation);
        anim.Play("ArtifactPopup");
        Invoke("FadeOut", 3f);
    }

    private void FadeOut()  // called by invoke
    {
        anim.Play("GemFadeOut");
    }
    public void DisableGameObject() // called by anim
    {
        gameObject.SetActive(false);
    }
}
