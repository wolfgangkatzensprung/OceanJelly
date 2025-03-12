using UnityEngine;

public class MenuButtons : MonoBehaviour
{
    public RectTransform[] buttons = new RectTransform[4];
    Animator[] anims;

    int buttonIndex = -1;        // -1 = kein button selektiert
    bool swappingBetweenButtons;

    private void Start()
    {
        anims = new Animator[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            anims[i] = buttons[i].GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (GameController.Instance == null)
            return;

        if (GameController.Instance.GetIsInMenu() && Input.GetButton("Escape"))
            ResetButtonSelection();

        //if (Input.GetButtonDown("Submit") && buttonIndex != -1)
        //{
        //    Debug.Log("Submit");
        //    buttons[buttonIndex].GetComponent<Button>().onClick.Invoke();
        //}

        //if (Input.GetAxisRaw("Vertical") != 0)
        //{
        //    if (!swappingBetweenButtons)
        //    {
        //        if (Input.GetButtonDown("Up") || Input.GetAxisRaw("Vertical") > .1f)
        //        {
        //            SwapButton(moveUp: true);
        //        }
        //        else if (Input.GetButtonDown("Down") || Input.GetAxisRaw("Vertical") < -.1f)
        //        {
        //            SwapButton(moveUp: false);
        //        }
        //    }
        //}
        else swappingBetweenButtons = false;
    }

    void SwapButton(bool moveUp)
    {
        Debug.Log("SwapButton with moveUp = " + moveUp);
        swappingBetweenButtons = true;

        if (moveUp)
        {
            buttonIndex -= 1;
            if (buttonIndex < 0)
            {
                buttonIndex = buttons.Length - 1;
            }
        }
        else
        {
            buttonIndex += 1;
            if (buttonIndex > buttons.Length - 1)
                buttonIndex = 0;
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttonIndex == i)
                anims[i].SetBool("isSelected", true);
            else
                anims[i].SetBool("isSelected", false);
        }
    }

    public void ResetButtonSelection()
    {
        Debug.Log("ResetButtonSelection()");

        foreach (Animator anim in anims)
        {
            anim.SetBool("isSelected", false);
            anim.Play("Idle");
        }
        buttonIndex = -1;
        swappingBetweenButtons = false;
    }

    private void OnDisable()
    {
        ResetButtonSelection();
    }
}