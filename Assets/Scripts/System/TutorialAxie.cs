using TMPro;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace AxieRescuer
{
    public partial class TutorialAxie : SystemBase
    {
        private GameObject _gameObject;
        private GameObject _canvas;
        protected override void OnCreate()
        {
            RequireForUpdate<AxieTag>(); 
            RequireForUpdate<PlayerTag>();
        }
        protected override void OnUpdate() 
        {
            if (Application.loadedLevelName != "TutorialScene") return;
            var player = SystemAPI.GetSingletonEntity<PlayerTag>();
            _gameObject = GameObject.FindGameObjectWithTag("AxieRescuer");
            var text = _gameObject.GetComponentInChildren<TMP_Text>();
           // _canvas = _gameObject.transform.GetChild(0).gameObject;
            var axie = SystemAPI.GetSingletonEntity<AxieTag>();
            var axieTransform = SystemAPI.GetComponent<LocalTransform>(axie).Position;
            var axieScale = SystemAPI.GetComponent<LocalTransform>(axie).Scale;
            var temp = axieTransform;
            var scale = axieScale / 10  ;
            temp.y = 10f ;
            _gameObject.transform.localPosition = temp;
            _gameObject.transform.localScale = new Vector3(scale,scale,scale);
            if (EntityManager.IsComponentEnabled<FollowingAxie>(player))
            {
                if (!EntityManager.IsComponentEnabled<FollowingAxie>(player)) return;
                text.text = "Move To SafeZone";
            }
        }
    }
}