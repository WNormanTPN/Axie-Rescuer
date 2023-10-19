using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AxieRescuer
{
    public class UIConfig : MonoBehaviour
    {
        #region Move
        public float TimeMoveTutorial;
        public GameObject MoveTutorial;
        public GameObject FireTutorial;
        public GameObject JoyStick;
        #endregion
        #region MoveToTarget
        public GameObject MTT;
        [SerializeField] private float timeOffMTT;
        #endregion
        void Start()
        {
            JoyStick.SetActive(false);
            MTT.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            #region Move
            TimeMoveTutorial -= Time.deltaTime;
            if (TimeMoveTutorial <= 0)
            {
                MoveTutorial.SetActive(false);
                FireTutorial.SetActive(false);
                JoyStick.SetActive(true);
                MTT.SetActive(true);
                timeOffMTT -= Time.deltaTime;
                if (timeOffMTT <= 0)
                {
                    MTT.SetActive (false);
                }
            }
            #endregion
            #region MoveToTarget
            
            #endregion
    }
}
}
