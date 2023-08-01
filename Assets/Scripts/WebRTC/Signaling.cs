using System;
using UnityEngine;

using Unity.WebRTC;

using WebSocketSharp;

public enum RoomStatus
{
    Waiting,
    Ready,
    Kicked,
}

public class RoomStatusEventArgs : EventArgs
{
    private RoomStatus _status;

    public RoomStatusEventArgs(string status)
    {
        switch (status)
        {
            case "waiting":
                _status = RoomStatus.Waiting;
                break;

            case "ready":
                _status = RoomStatus.Ready;
                break;

            case "kicked":
                _status = RoomStatus.Kicked;
                break;

            default:
                throw new ArgumentException("Invalid status value", status);
        }
    }

    public RoomStatus RoomStatus
    {
        get
        {
            return _status;
        }
    }
}

public class OfferEventArgs : EventArgs
{
    private RTCSessionDescription _offer;

    public OfferEventArgs(RTCSessionDescription offer)
    {
        _offer = offer;
    }
    public RTCSessionDescription Offer
    {
        get
        {
            return _offer;
        }
    }
}

public class AnswerEventArgs : EventArgs
{
    private RTCSessionDescription _answer;

    public AnswerEventArgs(RTCSessionDescription answer)
    {
        _answer = answer;
    }
    public RTCSessionDescription Answer
    {
        get
        {
            return _answer;
        }
    }
}

public class ICECandidateEventArgs : EventArgs
{
    private RTCIceCandidate _candidate;

    public ICECandidateEventArgs(RTCIceCandidate candidate)
    {
        _candidate = candidate;
    }
    public RTCIceCandidate Candidate
    {
        get
        {
            return _candidate;
        }
    }
}

public class Signaling
{
    private WebSocket webSocket;
    private string room;
    private string robot_uid;

    public event EventHandler<RoomStatusEventArgs> OnRoomStatus;
    public event EventHandler<OfferEventArgs> OnOffer;
    public event EventHandler<AnswerEventArgs> OnAnswer;
    public event EventHandler<ICECandidateEventArgs> OnICECandidate;
    public event EventHandler OnServerDisconnection;
    public event EventHandler OnServerConnection;

    public Signaling(string url, string room, string robot_uid, string role)
    {
        webSocket = new WebSocket(url);
        this.room = room;
        this.robot_uid = robot_uid;

        webSocket.EmitOnPing = true;

        webSocket.OnOpen += (sender, e) =>
        {
            JoinRoom(role);
            ServerConnection(EventArgs.Empty);
        };
        webSocket.OnMessage += (sender, e) =>
        {
            if (e.IsText)
            {
                var msg = JsonUtility.FromJson<SignalingMessage>(e.Data);
                Debug.LogWarning(e.Data);
                if (msg.room_status != null && msg.room_status != "")
                {
                    OnRoomStatus.Invoke(this, new RoomStatusEventArgs(msg.room_status));
                }
                else if (msg.signaling != null)
                {
                    if (msg.signaling.type == "offer")
                    {
                        var offer = new RTCSessionDescription
                        {
                            type = RTCSdpType.Offer,
                            sdp = msg.signaling.sdp,
                        };
                        OnOffer?.Invoke(this, new OfferEventArgs(offer));

                    }
                    else if (msg.signaling.type == "answer")
                    {
                        var answer = new RTCSessionDescription
                        {
                            type = RTCSdpType.Answer,
                            sdp = msg.signaling.sdp,
                        };
                        OnAnswer?.Invoke(this, new AnswerEventArgs(answer));
                    }
                    else if (msg.signaling.candidate != null)
                    {
                        RTCIceCandidate candidate = new RTCIceCandidate(
                            new RTCIceCandidateInit
                            {
                                candidate = msg.signaling.candidate.candidate,
                                sdpMid = msg.signaling.candidate.sdpMid,
                                sdpMLineIndex = msg.signaling.candidate.sdpMLineIndex,
                            }
                        );
                        OnICECandidate?.Invoke(this, new ICECandidateEventArgs(candidate));
                    }
                    else
                    {
                        Debug.LogWarning("here");
                        throw new ArgumentException("Unknown signaling message", e.Data);
                    }
                }
                else
                {
                    Debug.LogError("Unrecognized message !");
                }
            }
        };
        webSocket.OnError += (sender, e) =>
        {
            Debug.LogError($"WS error {e} : {e.Message}");
            ServerDisconnection(EventArgs.Empty);
        };
        webSocket.OnClose += (sender, e) =>
        {
            Debug.Log($"WS {this.room} closed");
            ServerDisconnection(EventArgs.Empty);
        };
    }
    public void Connect()
    {
        webSocket.ConnectAsync();
    }
    public void Close()
    {
        if (webSocket.IsAlive)
        {
            LeaveRoom();
        }
        webSocket.Close();
    }
    public void SendLocalDescription(RTCSessionDescription desc)
    {
        var msg = JsonUtility.ToJson(new SignalingMessage
        {
            signaling = new SignalingData(desc)
        });

        webSocket.Send(msg);
    }
    public void SendICECandidate(RTCIceCandidate candidate)
    {
        var msg = JsonUtility.ToJson(new SignalingMessage
        {
            signaling = new SignalingData(candidate)
        });
        webSocket.Send(msg);
    }
    private void JoinRoom(string role)
    {
        Debug.Log("Joining room " + this.room);
        string msg = JsonUtility.ToJson(new ActionJoinRoom
        {
            join_room = new Operator
            {
                room = this.room,
                uid = robot_uid,
                role = role
            }
        });
        webSocket.SendAsync(msg, null);
    }
    private void LeaveRoom()
    {
        Debug.Log("Leaving room " + this.room);
        string msg = JsonUtility.ToJson(new ActionLeaveRoom
        {
            leave_room = new Operator
            {
                room = this.room,
                uid = PlayerPrefs.GetString("robot_uid"),
            }
        });
        webSocket.Send(msg);
    }

    public virtual void ServerDisconnection(EventArgs e)
    {
        EventHandler handler = OnServerDisconnection;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    public virtual void ServerConnection(EventArgs e)
    {
        EventHandler handler = OnServerConnection;
        if (handler != null)
        {
            handler(this, e);
        }
    }
}

[System.Serializable]
class Operator
{
    public string room;
    public string role = "operator";
    public string uid;
}

[System.Serializable]
class ActionJoinRoom
{
    public Operator join_room;
}

[System.Serializable]
class ActionLeaveRoom
{
    public Operator leave_room;
}

[System.Serializable]
class SignalingData
{
    public string type;
    public string sdp;
    public ICECandidateMessage candidate;

    public SignalingData(RTCSessionDescription desc)
    {
        switch (desc.type)
        {
            case RTCSdpType.Offer:
                type = "offer";
                break;

            case RTCSdpType.Answer:
                type = "answer";
                break;

            default:
                throw new ArgumentException("Unsupported RTCSessionDescription type");
        }
        sdp = desc.sdp;
    }
    public SignalingData(RTCIceCandidate candidate)
    {
        this.type = "icecandidate";
        this.candidate = new ICECandidateMessage(candidate);
    }
}

[System.Serializable]
class ICECandidateMessage
{
    public string candidate;
    public string sdpMid;
    public int sdpMLineIndex;
    public string usernameFragment;

    public ICECandidateMessage(RTCIceCandidate candidate)
    {
        this.candidate = candidate.Candidate;
        this.sdpMid = candidate.SdpMid;
        this.sdpMLineIndex = candidate.SdpMLineIndex ?? 0;
        this.usernameFragment = candidate.UserNameFragment;
    }
}

[System.Serializable]
class SignalingMessage
{
    public string room_status;
    public SignalingData signaling;
}