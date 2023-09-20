using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System;

using UnityEngine.UI;
using Unity.WebRTC;

using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class WebRTCAudioSender : WebRTCBase
{
    [SerializeField] private AudioSource inputAudioSource;

    private MediaStream _sendStream;

    private AudioStreamTrack m_audioTrack;

    const int sampleRate = 48000;

    private AudioClip m_clipInput;

    private RTCRtpSender _sender = null;

    string _deviceName;


    protected override void Start()
    {
        base.Start();
        inputAudioSource = GetComponent<AudioSource>();
        _deviceName = Microphone.devices[0];
        Debug.Log("Microphone: " + _deviceName);
        Microphone.GetDeviceCaps(_deviceName, out int minFreq, out int maxFreq);
    }


    protected override void WebRTCCall()
    {
        base.WebRTCCall();
        _sendStream = new MediaStream();

        if (_pc != null)
        {
            _pc.OnNegotiationNeeded = () =>
            {
                Debug.Log($"[WebRTC] OnNegotiationNeeded");
                StartCoroutine(PeerNegotiationNeeded(_pc));
            };

            int m_lengthSeconds = 1;

            m_clipInput = Microphone.Start(_deviceName, true, m_lengthSeconds, sampleRate);
            // set the latency to “0” samples before the audio starts to play.
            while (!(Microphone.GetPosition(_deviceName) > 0)) { }

            inputAudioSource.loop = true;
            inputAudioSource.clip = m_clipInput;
            inputAudioSource.Play();

            m_audioTrack = new AudioStreamTrack(inputAudioSource);
            m_audioTrack.Loopback = false;

            Debug.Log("[WebRTCAudioSender] After AudioStreamTrack");
            _sender = _pc.AddTrack(m_audioTrack, _sendStream);

            Debug.Log(m_audioTrack.ReadyState);

            var codecs = RTCRtpSender.GetCapabilities(TrackKind.Audio).codecs;

            var excludeCodecTypes = new[] { "audio/CN", "audio/telephone-event" };

            List<RTCRtpCodecCapability> availableCodecs = new List<RTCRtpCodecCapability>();
            foreach (var codec in codecs)
            {
                if (excludeCodecTypes.Count(type => codec.mimeType.Contains(type)) > 0)
                    continue;
                /*if (codec.mimeType.Contains("audio/opus"))
                {
                    //to force opus
                    availableCodecs.Add(codec);
                }*/
            }

            var transceiver1 = _pc.GetTransceivers().First();
            Debug.Log("codec " + transceiver1 + " " + availableCodecs);
            var error = transceiver1.SetCodecPreferences(availableCodecs.ToArray());
            if (error != RTCErrorType.None)
                Debug.LogError(error);
            //var transceiver1 = _pc.GetTransceivers().First();
            transceiver1.Direction = RTCRtpTransceiverDirection.SendOnly;

        }
    }

    protected override void WebRTCHangUp()
    {
        if (_pc != null && _sender != null)
            _pc.RemoveTrack(_sender);

        Microphone.End(_deviceName);
        m_audioTrack?.Dispose();
        _sendStream?.Dispose();
        inputAudioSource.Stop();
        base.WebRTCHangUp();
    }
}