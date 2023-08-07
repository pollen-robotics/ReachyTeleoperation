using System.Collections;
using System.Collections.Concurrent;
using Unity.WebRTC;
using WebSocketSharp;
using UnityEngine;

[RequireComponent(typeof(WebRTCService))]
public abstract class WebRTCBase : MonoBehaviour
{
    private string _signalingServerURL;
    public string room = "teleop_stereo_video_robot_python_server";
    protected string uid = "Unity";
    private Signaling _signaling;

    protected RTCPeerConnection _pc;

    private static RTCConfiguration _conf = new RTCConfiguration
    {
        iceServers = new[] { new RTCIceServer { urls = new[] { "stun:stun.l.google.com:19302" } } }
    };

    private bool _gotOffer = false;
    private RTCSessionDescription _offer;
    private bool _gotAnswer = false;
    private RTCSessionDescription _answer;
    private ConcurrentQueue<RTCIceCandidate> _candidates = new ConcurrentQueue<RTCIceCandidate>();

    protected virtual void Awake()
    {
        //WebRTC.Initialize();
        // string ip_address = "localhost"; //PlayerPrefs.GetString("ip_address");
        // string ip_address="192.168.1.126";
        // string ip_address="0.0.0.0";
        string ip_address="192.168.1.174";
        _signalingServerURL = "ws://" + ip_address + ":8080/ws";
        Debug.Log("Signaling server URL: " + _signalingServerURL);
    }

    protected virtual void Start()
    {
        StartCoroutine(WebRTCUpdate());
        SignalingCall();
    }

    void OnDestroy()
    {
        WebRTCHangUp();
        _signaling?.Close();
    }

    void SignalingCall()
    {
        if (_signaling != null)
        {
            throw new System.Exception();
        }
        _signaling = new Signaling(_signalingServerURL, room, uid, "operator");// PlayerPrefs.GetString("robot_uid"));

        _signaling.OnRoomStatus += (sender, e) =>
        {
            switch (e.RoomStatus)
            {
                case RoomStatus.Waiting:
                    WebRTCHangUp();
                    break;

                case RoomStatus.Ready:
                    WebRTCCall();
                    break;

                case RoomStatus.Kicked:
                    break;
            }
        };
        _signaling.OnOffer += (sender, e) =>
        {
            Debug.Log($"[WebRTC] Got offer {e.Offer}");

            _offer = e.Offer;
            _gotOffer = true;
        };
        _signaling.OnAnswer += (sender, e) =>
        {
            Debug.Log($"[WebRTC] Got answer {e.Answer}");

            _answer = e.Answer;
            _gotAnswer = true;
        };
        _signaling.OnICECandidate += (sender, e) =>
        {
            Debug.Log($"[WebRTC] Got remote ICECandidate {e.Candidate}");

            _candidates.Enqueue(e.Candidate);
        };
        _signaling.Connect();
    }
    protected virtual void WebRTCCall()
    {
        if (_pc == null)
        {
            Debug.Log("[WebRTC] Calling for _pc...");

            _pc = new RTCPeerConnection(ref _conf);

            _pc.OnConnectionStateChange = (state) =>
            {
                Debug.Log($"[WebRTC] OnConnectionStateChange {state}");
            };
            _pc.OnIceCandidate = (candidate) =>
            {
                Debug.Log($"[WebRTC] Got local IceCandidate {candidate}");

                _signaling.SendICECandidate(candidate);
            };
            _pc.OnIceConnectionChange = (status) =>
            {
                Debug.Log($"[WebRTC] OnIceConnectionChange {status}");
            };
            _pc.OnIceGatheringStateChange = (status) =>
            {
                Debug.Log($"[WebRTC] OnIceGatheringStateChange {status}");
            };
            _pc.OnNegotiationNeeded = () =>
            {
                Debug.Log($"[WebRTC] OnNegotiationNeeded");
            };
            /*_pc.OnNegotiationNeeded = () =>
            {
                Debug.Log($"[WebRTC] OnNegotiationNeeded");
                StartCoroutine(PeerNegotiationNeeded(_pc));
            };*/
        }
    }

    protected virtual void WebRTCHangUp()
    {
        Debug.Log("[WebRTC] Closing...");

        _pc?.Dispose();
        _pc = null;
    }

    IEnumerator WebRTCUpdate()
    {
        while (true)
        {
            // Wait until all frame rendering is done
            yield return new WaitForEndOfFrame();

            if (_gotOffer)
            {
                _gotOffer = false;
                yield return WebRTCGotOffer(_offer);
            }
            if (_gotAnswer)
            {
                _gotAnswer = false;
                yield return WebRTCGotAnswer(_answer);
            }
            if (!_candidates.IsEmpty)
            {
                RTCIceCandidate candidate;
                if (_candidates.TryDequeue(out candidate))
                {
                    var newCandidate_pc = _pc.AddIceCandidate(candidate);
                    yield return newCandidate_pc;

                    Debug.Log($"[WebRTC] Adding remote candidate {candidate}");
                }
                else
                {
                    Debug.LogError("Deque fails!");
                }
            }
        }
    }
    IEnumerator WebRTCGotOffer(RTCSessionDescription offer)
    {
        var remoteDesc = _pc.SetRemoteDescription(ref offer);
        yield return remoteDesc;
        Debug.Log($"[WebRTC] Set offer as remote desc {remoteDesc}");

        var answer = _pc.CreateAnswer();
        yield return answer;
        var answerDesc = answer.Desc;
        Debug.Log($"[WebRTC] Create answer {answerDesc}");

        var localDesc = _pc.SetLocalDescription(ref answerDesc);
        yield return localDesc;
        Debug.Log($"[WebRTC] Set anwer as local desc {localDesc}");

        _signaling.SendLocalDescription(_pc.LocalDescription);
    }
    IEnumerator WebRTCGotAnswer(RTCSessionDescription answer)
    {
        var remoteDesc = _pc.SetRemoteDescription(ref answer);
        yield return remoteDesc;

        Debug.Log($"[WebRTC] Set answer as remote desc {remoteDesc}");
    }

    IEnumerator PeerNegotiationNeeded(RTCPeerConnection pc)
    {
        var op = pc.CreateOffer();
        yield return op;

        if (!op.IsError)
        {
            if (pc.SignalingState != RTCSignalingState.Stable)
            {
                yield break;
            }

            yield return StartCoroutine(OnCreateOfferSuccess(pc, op.Desc));
        }
        else
        {
            var error = op.Error;
            OnSetSessionDescriptionError(ref error);
        }
    }

    private IEnumerator OnCreateOfferSuccess(RTCPeerConnection pc, RTCSessionDescription desc)
    {
        var op = pc.SetLocalDescription(ref desc);
        yield return op;

        if (!op.IsError)
        {
            OnSetLocalSuccess(pc);
        }
        else
        {
            var error = op.Error;
            OnSetSessionDescriptionError(ref error);
        }

        _signaling.SendLocalDescription(pc.LocalDescription);
    }

    private void OnSetLocalSuccess(RTCPeerConnection pc)
    {
        Debug.Log("SetLocalDescription complete");
    }

    static void OnSetSessionDescriptionError(ref RTCError error)
    {
        Debug.LogError($"Error Detail Type: {error.message}");
    }

}