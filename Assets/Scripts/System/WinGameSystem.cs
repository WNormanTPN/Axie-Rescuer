using System.Globalization;
using TMPro;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace AxieRescuer
{
    [BurstCompile]
    public partial class WinGameSystem : SystemBase
    {
        private GameObject _winGameCanvas;
        private GameObject _gameCanvas;
        private int _axieCount = 0;
        private int _axieCurrent = 0;
        private GameObject _axieCountText;
        private TMP_Text _text;
        protected override void OnCreate()
        {
            RequireForUpdate<PlayerTag>();
            RequireForUpdate<AxieTag>();
        }
        protected override void OnStartRunning()
        {
            _axieCount = 0;
            foreach (var Axie in SystemAPI.Query<AxieTag>())
            {
                _axieCount++;
            }
        }
        protected override void OnUpdate()
        {
            if (Application.loadedLevelName == "TutorialScene") return;
            _winGameCanvas = GameObject.FindGameObjectWithTag("WinGame");
            _gameCanvas = _winGameCanvas.transform.GetChild(0).gameObject;
            _axieCountText = GameObject.FindGameObjectWithTag("AxieCount");
            _text = _axieCountText.GetComponent<TMP_Text>();

            foreach (var Axie in SystemAPI.Query<AxieTag>())
            {
                _axieCurrent++;
            }
                _text.text = "Axie rescued: " + (_axieCount - _axieCurrent).ToString() + "/" + "3";
                if (_axieCount - _axieCurrent == 3)
                {
                    _gameCanvas.SetActive(true);
                    _axieCountText.SetActive(true);
                    UnityEngine.Time.timeScale = 0;

                }
            _axieCurrent = 0;
        }
    }
}