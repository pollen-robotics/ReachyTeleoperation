using UnityEngine;
using Unity.WebRTC;
using UnityEngine.UI;
using UnityEngine.Events;
namespace TeleopReachy
{
    public class WebRTCVideoReceiver : WebRTCBase
    {
        private MediaStream _receiveStream;
        private bool isRobotInRoom = true;
        
        public Renderer screen;
        public Material image;
        public UnityEvent<bool> event_OnVideoRoomStatusHasChanged;

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
        }
    }

}