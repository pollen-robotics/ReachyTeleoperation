using UnityEngine;
using Unity.WebRTC;
using UnityEngine.UI;
using UnityEngine.Events;

public class WebRTCAVReceiver : WebRTCBase
{
    public Renderer screen;
    public AudioSource outputAudioSource;
    private MediaStream _receiveStream;

    private string left_track_id_name = "";

    public UnityEvent<bool> event_OnVideoRoomStatusHasChanged;

    protected override void WebRTCCall()
    {
        base.WebRTCCall();
        left_track_id_name = "";
        _receiveStream = new MediaStream();
        _receiveStream.OnAddTrack += OnAddTrack;
        if (_pc != null)
        {
            _pc.OnTrack = (evt) =>
            {
                Debug.Log($"[WebRTC] OnTrack {evt.Track} id: {evt.Track.Id}");

                if (evt.Track.Kind == TrackKind.Video)
                {
                    //left track is received first
                    if (left_track_id_name == "")
                        left_track_id_name = evt.Track.Id;
                    _receiveStream.AddTrack(evt.Track);
                    if (screen == null)
                        Debug.LogError("Screen is not assigned. Image won't be rendered");
                    event_OnVideoRoomStatusHasChanged.Invoke(true);
                }
                else if (evt.Track.Kind == TrackKind.Audio)
                {
                    _receiveStream.AddTrack(evt.Track);
                    if (outputAudioSource == null)
                        Debug.LogWarning("Output audio is not assigned. Sound won't be rendered");
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
                if (e.Track.Id == left_track_id_name)
                {
                    screen.material.SetTexture("_LeftTex", tex);

                }
                else
                {
                    screen.material.SetTexture("_RightTex", tex);
                }
            };
        }
        else if (e.Track is AudioStreamTrack audio)
        {
            Debug.Log("playing audio track");
            outputAudioSource.Stop();
            outputAudioSource.SetTrack(audio);
            outputAudioSource.loop = true;
            outputAudioSource.Play();
        }
    }
}

