using UnityEngine;
using Unity.WebRTC;
using UnityEngine.UI;

public class WebRTCVideoReceiver : WebRTCBase
{
    public Renderer screen;
    private MediaStream _receiveStream;

    protected override void Start()
    {
        base.Start();
        //var codecs = RTCRtpSender.GetCapabilities(TrackKind.Video).codecs;
        //Debug.Log(codecs);
        if (screen == null)
            Debug.LogError("texture is not assigned!");
    }

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
                }
            };
            var transceiver = _pc.AddTransceiver(TrackKind.Video);
            transceiver.Direction = RTCRtpTransceiverDirection.RecvOnly;
        }
    }

    void OnAddTrack(MediaStreamTrackEvent e)
    {
        if (e.Track is VideoStreamTrack video)
        {
            video.OnVideoReceived += tex =>
            {
                if (e.Track.Id == "right")
                    screen.material.SetTexture("_MainTex_right", tex);
                else
                    screen.material.mainTexture = tex;
            };
        }
    }
}

