using UnityEngine;
using Unity.WebRTC;
using UnityEngine.UI;

public class WebRTCAVReceiver : WebRTCBase
{
    public Renderer screen;
    public AudioSource outputAudioSource;
    private MediaStream _receiveStream;

    /*protected override void Start()
    {
        base.Start();
    }*/

    protected override void WebRTCCall()
    {
        base.WebRTCCall();
        _receiveStream = new MediaStream();
        _receiveStream.OnAddTrack += OnAddTrack;
        if (_pc != null)
        {
            _pc.OnTrack = (evt) =>
            {
                Debug.Log($"[WebRTC] OnTrack {evt.Track} id: {evt.Track.Id}");

                if (evt.Track.Kind == TrackKind.Video)
                {
                    _receiveStream.AddTrack(evt.Track);
                    if (screen == null)
                        Debug.LogError("Screen is not assigned. Image won't be rendered");
                }
                else if (evt.Track.Kind == TrackKind.Audio)
                {
                    _receiveStream.AddTrack(evt.Track);
                    if (outputAudioSource == null)
                        Debug.LogWarning("Output audio is not assigned. Sound won't be rendered");
                }
            };
            //var transceiver = _pc.AddTransceiver(TrackKind.Video);
            //transceiver.Direction = RTCRtpTransceiverDirection.RecvOnly;
        }
    }

    void OnAddTrack(MediaStreamTrackEvent e)
    {
        if (e.Track is VideoStreamTrack video)
        {
            video.OnVideoReceived += tex =>
            {
                //if (e.Track.Id == "left")
                screen.material.mainTexture = tex;
                //else
                //screen.material.SetTexture("_MainTex_right", tex);
            };
        }
        else if (e.Track is AudioStreamTrack audio)
        {
            outputAudioSource.SetTrack(audio);
            outputAudioSource.loop = true;
            outputAudioSource.Play();
        }
    }
}

