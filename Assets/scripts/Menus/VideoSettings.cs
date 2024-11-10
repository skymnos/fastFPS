using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoSettings : MonoBehaviour
{
    [SerializeField] private Toggle fullscreenTog;
    [SerializeField] private Toggle VsyncTog;


    [SerializeField] private TMP_Dropdown resolutionDropDown;
    [SerializeField] private Vector2[] resolutions;

    private void Start()
    {
        fullscreenTog.isOn = Screen.fullScreen;

        if (QualitySettings.vSyncCount == 0)
        {
            VsyncTog.isOn = false;
        }
        else
        {
            VsyncTog.isOn= true;
        }

        bool foundRes = false;
        for (int i = 0; i < resolutions.Length; i++) 
        {
            if (Screen.width == (int) resolutions[i].x && Screen.height == (int) resolutions[i].y) 
            { 
                foundRes = true; 
                resolutionDropDown.value = i;

                break; 
            }
        }

        if (!foundRes)
        {
            List<string> newRes = new List<string> {Screen.width + "X" + Screen.height};
            resolutionDropDown.AddOptions(newRes);
            resolutionDropDown.value = resolutionDropDown.options.Count - 1;
        }
    }

    public void Apply()
    {

        if(VsyncTog.isOn)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }

        Screen.SetResolution( (int) resolutions[resolutionDropDown.value].x, (int) resolutions[resolutionDropDown.value].y, fullscreenTog.isOn);
    }
}
