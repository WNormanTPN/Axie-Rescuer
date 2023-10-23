using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AxieRescuer
{
    public class LoadScene : MonoBehaviour
    {
        [SerializeField] private GameObject _loadingScene;
        [SerializeField] private Slider loadingSlider;
        public void LoadLevelBtn()
        {
            _loadingScene.SetActive(true);
            StartCoroutine(LoadLevelAsync());    
        }
        public void Awake()
        {
            LoadLevelBtn();
        }
        IEnumerator LoadLevelAsync()
        {
            while (loadingSlider.value < 1)
            {
                    loadingSlider.value += 0.01f;
                    yield return new WaitForSeconds(0.1f);  
            }
            _loadingScene.SetActive(false);
        }
    }
}