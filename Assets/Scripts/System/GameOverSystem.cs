using System;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AxieRescuer
{
    [BurstCompile]
    public partial class GameOverSystem : SystemBase
    {
        private GameObject _canvasGameOver;
        private GameObject _gameOver;
        private bool _flag = true;
        protected override void OnCreate()
        {
            RequireForUpdate<PlayerTag>();
        }
        protected override void OnUpdate()
        {
            _canvasGameOver = GameObject.FindGameObjectWithTag("GameOver");
            if (_canvasGameOver == null) return;
            _gameOver = _canvasGameOver.transform.GetChild(0).gameObject;
            var player = SystemAPI.GetSingletonEntity<PlayerTag>();
            var health = SystemAPI.GetComponentRO<Health>(player);
            if (_flag )
            {
                if (health.ValueRO.Current <= 0)
                {
                    _flag = false;
                    _gameOver.SetActive(true);
                    UnityEngine.Time.timeScale = 0;
                }
            }
            //if (health.ValueRO.Current > 0)
            //{
            //    UnityEngine.Time.timeScale = 1;
            //}
        }
    }
}