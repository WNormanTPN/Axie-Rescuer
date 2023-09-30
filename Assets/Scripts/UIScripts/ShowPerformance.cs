using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AxieRescuer
{
public class ShowPerformance : MonoBehaviour
{
    [SerializeField] private TMP_Text _FPSValue = null;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("GetFPS", 0, 1);
    }

        // Update is called once per frame
        void GetFPS()
        {
            float _fps = (int)(1f / Time.unscaledDeltaTime);
            _FPSValue.text = _fps.ToString();

        }
    }
}
