using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActualButton : MonoBehaviour
{
    public ButtonType type;
    public Animator animator;

    public string sceneToChangeTo;

    bool buttonPressed;

    private void Update()
    {
        if (buttonPressed)
        {
            if(animator.GetCurrentAnimatorStateInfo(0).IsName("ButtonPush") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                if (type == ButtonType.SceneChanger)
                    SceneManager.LoadScene(sceneToChangeTo);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "projectile")
        {
            animator.SetTrigger("PushButton");
            buttonPressed = true;
        }
    }
}