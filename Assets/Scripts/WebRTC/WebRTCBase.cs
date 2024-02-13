using System.Collections;
using System.Collections.Concurrent;
using Unity.WebRTC;
using UnityEngine;

[RequireComponent(typeof(WebRTCService))]
public abstract class WebRTCBase : MonoBehaviour
{
    private string _signalingServerURL;
    private Signaling _signaling;

    protected RTCPeerConnection _pc;

    public bool _producer = false;

    private static RTCConfiguration _conf = new RTCConfiguration
    {
        iceServers = new[] { new RTCIceServer { urls = new[] { "stun:stun.l.google.com:19302" } } }
    };

    private bool _gotOffer = false;
    private RTCSessionDescription _offer;
    private bool _gotAnswer = false;
    private bool _offerSent = false;
    private RTCSessionDescription _answer;
    private ConcurrentQueue<RTCIceCandidate> _candidates = new ConcurrentQueue<RTCIceCandidate>();

    protected virtual void Awake()
    {
        //WebRTC.Initialize();
        string ip_address = PlayerPrefs.GetString("robot_ip");
        //string ip_address = "10.0.1.36";
        // string ip_address="0.0.0.0";
        //string ip_address = "192.168.1.108";
        _signalingServerURL = "ws://" + ip_address + ":8443";
        Debug.Log("Signaling server URL: " + _signalingServerURL);
    }

    protected virtual void Start()
    {
        StartCoroutine(WebRTCUpdate());
        SignalingCall();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        _signaling.UpdateMessages();
#endif
    }

    void OnDestroy()
    {
        WebRTCHangUp();
        _signaling?.Close();
    }

    void HandleConnectionStatus(ConnectionStatus status)
    {
        switch (status)
        {
            case ConnectionStatus.Waiting:
                WebRTCHangUp();
                break;

            case ConnectionStatus.Ready:
                WebRTCCall();
                break;

            case ConnectionStatus.Kicked:
                Debug.LogWarning("Status not handled");
                break;
        }
    }

    void HandleOffer(RTCSessionDescription offer)
    {
        Debug.Log($"[WebRTC] Got offer {offer}");
        _offer = offer;
        _gotOffer = true;
    }

    void HandleAnswer(RTCSessionDescription answer)
    {
        Debug.Log($"[WebRTC] Got offer {answer}");
        _answer = answer;
        _gotAnswer = true;
    }

    void HandleICECandidate(RTCIceCandidate candidate)
    {
        Debug.Log($"[WebRTC] Got remote ICECandidate {candidate}");
        _candidates.Enqueue(candidate);
    }

    void SignalingCall()
    {
        if (_signaling != null)
        {
            throw new System.Exception();
        }
        _signaling = new Signaling(_signalingServerURL, _producer);

        _signaling.event_OnConnectionStatus.AddListener(HandleConnectionStatus);

        _signaling.event_OnOffer.AddListener(HandleOffer);

        _signaling.event_OnAnswer.AddListener(HandleAnswer);

        _signaling.event_OnICECandidate.AddListener(HandleICECandidate);
        _offerSent = false;

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

                //some client needs to receive SDP before ice candidates (i.e. chrome)
                StartCoroutine(SendICECandidate(candidate));
            };
            _pc.OnIceConnectionChange = (status) =>
            {
                Debug.Log($"[WebRTC] OnIceConnectionChange {status}");
            };
            _pc.OnIceGatheringStateChange = (status) =>
            {
                Debug.Log($"[WebRTC] OnIceGatheringStateChange {status}");
            };
            /*_pc.OnNegotiationNeeded = () =>
            {
                Debug.Log($"[WebRTC] OnNegotiationNeeded");
            };*/
            /*_pc.OnNegotiationNeeded = () =>
            {
                Debug.Log($"[WebRTC] OnNegotiationNeeded");
                StartCoroutine(PeerNegotiationNeeded(_pc));
            };*/
            /*_pc.OnTrack = (evt) =>
            {
                Debug.Log($"[WebRTCAudioSender] OnTrack {evt.Track}");
            };*/
        }
    }

    IEnumerator SendICECandidate(RTCIceCandidate candidate)
    {
        //send ice after sdp
        while (!_offerSent)
        {
            Debug.Log("Wait for sdp to complete");
            yield return new WaitForSeconds(0.5f);
        }

        _signaling.SendICECandidate(candidate);
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
                    if (_pc != null)
                    {
                        var newCandidate_pc = _pc.AddIceCandidate(candidate);
                        yield return newCandidate_pc;

                        Debug.Log($"[WebRTC] Adding remote candidate {candidate}");
                    }
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
        _offerSent = true;
        //_signaling.SendLocalDescription(answerDesc);
    }
    IEnumerator WebRTCGotAnswer(RTCSessionDescription answer)
    {
        var remoteDesc = _pc.SetRemoteDescription(ref answer);
        yield return remoteDesc;

        Debug.Log($"[WebRTC] Set answer as remote desc {remoteDesc}");
    }

    protected IEnumerator PeerNegotiationNeeded(RTCPeerConnection pc)
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
        Debug.Log($"[WebRTC] Create offer {pc.LocalDescription}");
        _signaling.SendLocalDescription(pc.LocalDescription, "offer");
        _offerSent = true;
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
