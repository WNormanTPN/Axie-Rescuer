using TMPro;
using UnityEngine;
namespace AxieRescuer
{
    public class UICheckLoad : MonoBehaviour
    {
        public TMP_Text FPSValue;
        private void Awake()
        {
            if (PlayerPrefs.HasKey("istoggle"))
            {
                int checkToggle = PlayerPrefs.GetInt("istoggle");
                if (checkToggle == 1)
                {
                    FPSValue.enabled = true;
                }
                else
                {
                    FPSValue.enabled = false;
                }
            }
        }
    }
}
