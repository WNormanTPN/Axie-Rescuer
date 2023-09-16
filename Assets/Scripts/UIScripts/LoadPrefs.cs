using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadPrefs : MonoBehaviour
{
    [Header("Volume Setting")]
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private TMP_Text _volumeValue;
    [SerializeField] private Toggle _toggleFPS;
    private float _defaultVolume = 50f;
    private void Awake()
    {
        if (PlayerPrefs.HasKey("mastervolume"))
        {
            float _localVolume = PlayerPrefs.GetFloat("mastervolume");
            _volumeSlider.value = _localVolume;
            _volumeValue.text = ((int)_localVolume).ToString();
        }
        if(PlayerPrefs.HasKey("istoggle"))
        {
            int checkToggle = PlayerPrefs.GetInt("istoggle");
            if (checkToggle == 1) 
            {
                _toggleFPS.isOn = true;
            }
            else
            {
                _toggleFPS.isOn= false;
            }
        }
    }

}
