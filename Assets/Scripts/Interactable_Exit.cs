using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interactable_Exit : MonoBehaviour
{

    [SerializeField] private float sceneTransitionDelay = 3f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(ExitTransition());
    }

    private IEnumerator ExitTransition()
    {
        yield return new WaitForSeconds(sceneTransitionDelay);

        int sceneToLoadIndex  = SceneManager.GetActiveScene().buildIndex + 1;

        if (sceneToLoadIndex <= SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(sceneToLoadIndex);
        }
    }


}
