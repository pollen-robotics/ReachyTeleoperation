using UnityEngine;
using Unity.WebRTC;
using UnityEngine.UI;
using UnityEngine.Events;

public class WebRTCAVReceiver : WebRTCBase
{
    public Renderer screen;
    public AudioSource outputAudioSource;
    private MediaStream _receiveStream;

    private bool isRobotInRoom = true;
    public Material image;
        
    public UnityEvent<bool> event_OnVideoRoomStatusHasChanged;


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
            var transceiver = _pc.AddTransceiver(TrackKind.Video);
            transceiver.Direction = RTCRtpTransceiverDirection.RecvOnly;
        }
    }

    void OnAddTrack(MediaStreamTrackEvent e)
    {
        if (e.Track is VideoStreamTrack video)
        {
            isRobotInRoom = true;
            event_OnVideoRoomStatusHasChanged.Invoke(isRobotInRoom);
            video.OnVideoReceived += tex =>
            {
                if (e.Track.Id == "right"){
                    Debug.LogError("right");
                    image.SetTexture("_RightTex", tex);
                }
                else{
                    Debug.LogError("left");
                    image.SetTexture("_LeftTex", tex);
                }
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
