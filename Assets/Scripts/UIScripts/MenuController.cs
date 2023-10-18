using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
namespace AxieRescuer
{
    public class MenuController : MonoBehaviour
    {
        public string SceneName;
        [SerializeField] private Slider _audioSlider = null;
        [SerializeField] private TMP_Text _audioTextValue = null;
        [SerializeField] private GameObject _confirmbox = null;
        [SerializeField] private Toggle _toggleFPS = null;
        public void NewGameDialogYes()
        {
            SceneManager.LoadScene(SceneName);
        }
        public void AudioChange()
        {
            AudioListener.volume = _audioSlider.value / 100;
            _audioTextValue.text = ((int)(_audioSlider.value)).ToString();
        }
        public void AudioApply()
        {
            PlayerPrefs.SetFloat("mastervolume", AudioListener.volume);
            StartCoroutine(Confirmationbox());
        }
        public void ShowPerformanceApply()
        {
            if (_toggleFPS.isOn)
            {
                PlayerPrefs.SetInt("istoggle", 1);
            }
            else
            {
                PlayerPrefs.SetInt("istoggle", 0);
            }
            StartCoroutine(Confirmationbox());
        }
        public void ResetToDefault()
        {
            AudioListener.volume = 50;
            _audioSlider.value = 50f;
            _audioTextValue.text = "50";
            _toggleFPS.isOn = true;
            AudioApply();
        }
        public void ExitGame()
        {
            Application.Quit();
        }
        IEnumerator Confirmationbox()
        {
            _confirmbox.SetActive(true);
            yield return new WaitForSeconds(2);
            _confirmbox.SetActive(false);
        }

    }
}
