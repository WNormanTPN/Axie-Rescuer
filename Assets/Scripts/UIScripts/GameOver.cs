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
            SceneManager.LoadScene("Demo_Scene");
            Time.timeScale = 1;
        }
        public void BackToTheMenu()
        {
            SceneManager.LoadScene("MainMenu");
            Time.timeScale = 1;
        }
    }
    
}