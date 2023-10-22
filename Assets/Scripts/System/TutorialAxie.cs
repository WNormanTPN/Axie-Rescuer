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
        }
        protected override void OnUpdate() 
        {
            if (Application.loadedLevelName != "TutorialScene") return;
            _gameObject = GameObject.FindGameObjectWithTag("AxieRescuer");
           // _canvas = _gameObject.transform.GetChild(0).gameObject;
            var axie = SystemAPI.GetSingletonEntity<AxieTag>();
            var axieTransform = SystemAPI.GetComponent<LocalTransform>(axie).Position;
            var axieScale = SystemAPI.GetComponent<LocalTransform>(axie).Scale;
            var temp = axieTransform;
            var scale = axieScale / 10  ;
            temp.y = 10f ;
            _gameObject.transform.localPosition = temp;
            _gameObject.transform.localScale = new Vector3(scale,scale,scale);
        }
    }
}