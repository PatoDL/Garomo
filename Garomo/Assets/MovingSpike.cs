using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSpike : MonoBehaviour
{
    public void SpikeOn()
    {
        if (GameManager.Instance.soundOn)
            AkSoundEngine.PostEvent("Spike_On", gameObject);
    }

    public void SpikeOff()
    {
        if (GameManager.Instance.soundOn)
            AkSoundEngine.PostEvent("Spike_Off", gameObject);
    }
}
