using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AxieRescuer
{
    public class GameOver : MonoBehaviour
    {
        public void PlayAgain()
        {
            StartCoroutine(WaitForLoading());
            Time.timeScale = 1;
        }
        public void BackToTheMenu()
        {
            SceneManager.LoadScene("MainMenu");
            Time.timeScale = 1;
        }
        IEnumerator WaitForLoading()
        {
            SceneManager.LoadScene("Demo_Scene");
            yield return new WaitForSeconds(5);
        }
    }
    
}