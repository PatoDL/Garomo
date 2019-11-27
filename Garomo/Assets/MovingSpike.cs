using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSpike : MonoBehaviour
{
    public void SpikeOn()
    {
        AkSoundEngine.PostEvent("Spike_On", gameObject);
    }

    public void SpikeOff()
    {
        AkSoundEngine.PostEvent("Spike_Off", gameObject);
    }
}
