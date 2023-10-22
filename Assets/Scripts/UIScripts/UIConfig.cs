using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AxieRescuer
{
    public class UIConfig : MonoBehaviour
    {
        public GameObject MoveTutorial;
        public GameObject FireTutorial;
        public float TimeUpTutorial;
        void Start()
        {
          
        }

        // Update is called once per frame
        void Update()
        {
            TimeUpTutorial -= Time.deltaTime;
            if (TimeUpTutorial <= 0 )
            {
                MoveTutorial.SetActive(false);
                FireTutorial.SetActive(false);
            }
        }
}
}
