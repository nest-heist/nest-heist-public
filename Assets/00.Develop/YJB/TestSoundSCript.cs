using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestSoundSCript : MonoBehaviour
{
    bool IsPlaying = false;
    void Start()
    {
    }
    private void Update()
    {
        if (!IsPlaying)
        {
            AudioManager.Instance.PlayBGM("track_shortadventure_loop");
            IsPlaying = true;
        }

        /*if(Input.GetMouseButton(0))
        {
            AudioManager.Instance.PlaySFX("sfx_button_select2");
        }*/
    }


}
