using Unity.WebRTC;
using UnityEngine;
public class WebRTCService : GenericSingletonClass<WebRTCService>
{
    void Start()
    {
        StartCoroutine(WebRTC.Update());
    }

}