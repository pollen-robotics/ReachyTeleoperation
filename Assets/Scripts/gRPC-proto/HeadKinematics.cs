// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: head_kinematics.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Reachy.Sdk.Kinematics {

  /// <summary>Holder for reflection information generated from head_kinematics.proto</summary>
  public static partial class HeadKinematicsReflection {

    #region Descriptor
    /// <summary>File descriptor for head_kinematics.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static HeadKinematicsReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChVoZWFkX2tpbmVtYXRpY3MucHJvdG8SFXJlYWNoeS5zZGsua2luZW1hdGlj",
            "cxoQa2luZW1hdGljcy5wcm90byJvCg1IZWFkSUtSZXF1ZXN0EiwKAXEYASAB",
            "KAsyIS5yZWFjaHkuc2RrLmtpbmVtYXRpY3MuUXVhdGVybmlvbhIwCgJxMBgC",
            "IAEoCzIkLnJlYWNoeS5zZGsua2luZW1hdGljcy5Kb2ludFBvc2l0aW9uIkwK",
            "DUhlYWRGS1JlcXVlc3QSOwoNbmVja19wb3NpdGlvbhgBIAEoCzIkLnJlYWNo",
            "eS5zZGsua2luZW1hdGljcy5Kb2ludFBvc2l0aW9uIk8KDkhlYWRGS1NvbHV0",
            "aW9uEg8KB3N1Y2Nlc3MYASABKAgSLAoBcRgCIAEoCzIhLnJlYWNoeS5zZGsu",
            "a2luZW1hdGljcy5RdWF0ZXJuaW9uIl4KDkhlYWRJS1NvbHV0aW9uEg8KB3N1",
            "Y2Nlc3MYASABKAgSOwoNbmVja19wb3NpdGlvbhgCIAEoCzIkLnJlYWNoeS5z",
            "ZGsua2luZW1hdGljcy5Kb2ludFBvc2l0aW9uMswBCg5IZWFkS2luZW1hdGlj",
            "cxJcCg1Db21wdXRlSGVhZEZLEiQucmVhY2h5LnNkay5raW5lbWF0aWNzLkhl",
            "YWRGS1JlcXVlc3QaJS5yZWFjaHkuc2RrLmtpbmVtYXRpY3MuSGVhZEZLU29s",
            "dXRpb24SXAoNQ29tcHV0ZUhlYWRJSxIkLnJlYWNoeS5zZGsua2luZW1hdGlj",
            "cy5IZWFkSUtSZXF1ZXN0GiUucmVhY2h5LnNkay5raW5lbWF0aWNzLkhlYWRJ",
            "S1NvbHV0aW9uYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Reachy.Sdk.Kinematics.KinematicsReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Reachy.Sdk.Kinematics.HeadIKRequest), global::Reachy.Sdk.Kinematics.HeadIKRequest.Parser, new[]{ "Q", "Q0" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Reachy.Sdk.Kinematics.HeadFKRequest), global::Reachy.Sdk.Kinematics.HeadFKRequest.Parser, new[]{ "NeckPosition" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Reachy.Sdk.Kinematics.HeadFKSolution), global::Reachy.Sdk.Kinematics.HeadFKSolution.Parser, new[]{ "Success", "Q" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Reachy.Sdk.Kinematics.HeadIKSolution), global::Reachy.Sdk.Kinematics.HeadIKSolution.Parser, new[]{ "Success", "NeckPosition" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class HeadIKRequest : pb::IMessage<HeadIKRequest>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<HeadIKRequest> _parser = new pb::MessageParser<HeadIKRequest>(() => new HeadIKRequest());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<HeadIKRequest> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Reachy.Sdk.Kinematics.HeadKinematicsReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HeadIKRequest() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HeadIKRequest(HeadIKRequest other) : this() {
      q_ = other.q_ != null ? other.q_.Clone() : null;
      q0_ = other.q0_ != null ? other.q0_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HeadIKRequest Clone() {
      return new HeadIKRequest(this);
    }

    /// <summary>Field number for the "q" field.</summary>
    public const int QFieldNumber = 1;
    private global::Reachy.Sdk.Kinematics.Quaternion q_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Reachy.Sdk.Kinematics.Quaternion Q {
      get { return q_; }
      set {
        q_ = value;
      }
    }

    /// <summary>Field number for the "q0" field.</summary>
    public const int Q0FieldNumber = 2;
    private global::Reachy.Sdk.Kinematics.JointPosition q0_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Reachy.Sdk.Kinematics.JointPosition Q0 {
      get { return q0_; }
      set {
        q0_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as HeadIKRequest);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(HeadIKRequest other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(Q, other.Q)) return false;
      if (!object.Equals(Q0, other.Q0)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (q_ != null) hash ^= Q.GetHashCode();
      if (q0_ != null) hash ^= Q0.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (q_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Q);
      }
      if (q0_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Q0);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (q_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Q);
      }
      if (q0_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Q0);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (q_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Q);
      }
      if (q0_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Q0);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(HeadIKRequest other) {
      if (other == null) {
        return;
      }
      if (other.q_ != null) {
        if (q_ == null) {
          Q = new global::Reachy.Sdk.Kinematics.Quaternion();
        }
        Q.MergeFrom(other.Q);
      }
      if (other.q0_ != null) {
        if (q0_ == null) {
          Q0 = new global::Reachy.Sdk.Kinematics.JointPosition();
        }
        Q0.MergeFrom(other.Q0);
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            if (q_ == null) {
              Q = new global::Reachy.Sdk.Kinematics.Quaternion();
            }
            input.ReadMessage(Q);
            break;
          }
          case 18: {
            if (q0_ == null) {
              Q0 = new global::Reachy.Sdk.Kinematics.JointPosition();
            }
            input.ReadMessage(Q0);
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            if (q_ == null) {
              Q = new global::Reachy.Sdk.Kinematics.Quaternion();
            }
            input.ReadMessage(Q);
            break;
          }
          case 18: {
            if (q0_ == null) {
              Q0 = new global::Reachy.Sdk.Kinematics.JointPosition();
            }
            input.ReadMessage(Q0);
            break;
          }
        }
      }
    }
    #endif

  }

  public sealed partial class HeadFKRequest : pb::IMessage<HeadFKRequest>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<HeadFKRequest> _parser = new pb::MessageParser<HeadFKRequest>(() => new HeadFKRequest());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<HeadFKRequest> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Reachy.Sdk.Kinematics.HeadKinematicsReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HeadFKRequest() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HeadFKRequest(HeadFKRequest other) : this() {
      neckPosition_ = other.neckPosition_ != null ? other.neckPosition_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HeadFKRequest Clone() {
      return new HeadFKRequest(this);
    }

    /// <summary>Field number for the "neck_position" field.</summary>
    public const int NeckPositionFieldNumber = 1;
    private global::Reachy.Sdk.Kinematics.JointPosition neckPosition_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Reachy.Sdk.Kinematics.JointPosition NeckPosition {
      get { return neckPosition_; }
      set {
        neckPosition_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as HeadFKRequest);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(HeadFKRequest other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(NeckPosition, other.NeckPosition)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (neckPosition_ != null) hash ^= NeckPosition.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (neckPosition_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(NeckPosition);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (neckPosition_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(NeckPosition);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (neckPosition_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(NeckPosition);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(HeadFKRequest other) {
      if (other == null) {
        return;
      }
      if (other.neckPosition_ != null) {
        if (neckPosition_ == null) {
          NeckPosition = new global::Reachy.Sdk.Kinematics.JointPosition();
        }
        NeckPosition.MergeFrom(other.NeckPosition);
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            if (neckPosition_ == null) {
              NeckPosition = new global::Reachy.Sdk.Kinematics.JointPosition();
            }
            input.ReadMessage(NeckPosition);
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            if (neckPosition_ == null) {
              NeckPosition = new global::Reachy.Sdk.Kinematics.JointPosition();
            }
            input.ReadMessage(NeckPosition);
            break;
          }
        }
      }
    }
    #endif

  }

  public sealed partial class HeadFKSolution : pb::IMessage<HeadFKSolution>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<HeadFKSolution> _parser = new pb::MessageParser<HeadFKSolution>(() => new HeadFKSolution());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<HeadFKSolution> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Reachy.Sdk.Kinematics.HeadKinematicsReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HeadFKSolution() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HeadFKSolution(HeadFKSolution other) : this() {
      success_ = other.success_;
      q_ = other.q_ != null ? other.q_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HeadFKSolution Clone() {
      return new HeadFKSolution(this);
    }

    /// <summary>Field number for the "success" field.</summary>
    public const int SuccessFieldNumber = 1;
    private bool success_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Success {
      get { return success_; }
      set {
        success_ = value;
      }
    }

    /// <summary>Field number for the "q" field.</summary>
    public const int QFieldNumber = 2;
    private global::Reachy.Sdk.Kinematics.Quaternion q_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Reachy.Sdk.Kinematics.Quaternion Q {
      get { return q_; }
      set {
        q_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as HeadFKSolution);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(HeadFKSolution other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Success != other.Success) return false;
      if (!object.Equals(Q, other.Q)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Success != false) hash ^= Success.GetHashCode();
      if (q_ != null) hash ^= Q.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (Success != false) {
        output.WriteRawTag(8);
        output.WriteBool(Success);
      }
      if (q_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Q);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (Success != false) {
        output.WriteRawTag(8);
        output.WriteBool(Success);
      }
      if (q_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Q);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Success != false) {
        size += 1 + 1;
      }
      if (q_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Q);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(HeadFKSolution other) {
      if (other == null) {
        return;
      }
      if (other.Success != false) {
        Success = other.Success;
      }
      if (other.q_ != null) {
        if (q_ == null) {
          Q = new global::Reachy.Sdk.Kinematics.Quaternion();
        }
        Q.MergeFrom(other.Q);
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Success = input.ReadBool();
            break;
          }
          case 18: {
            if (q_ == null) {
              Q = new global::Reachy.Sdk.Kinematics.Quaternion();
            }
            input.ReadMessage(Q);
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 8: {
            Success = input.ReadBool();
            break;
          }
          case 18: {
            if (q_ == null) {
              Q = new global::Reachy.Sdk.Kinematics.Quaternion();
            }
            input.ReadMessage(Q);
            break;
          }
        }
      }
    }
    #endif

  }

  public sealed partial class HeadIKSolution : pb::IMessage<HeadIKSolution>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<HeadIKSolution> _parser = new pb::MessageParser<HeadIKSolution>(() => new HeadIKSolution());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<HeadIKSolution> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Reachy.Sdk.Kinematics.HeadKinematicsReflection.Descriptor.MessageTypes[3]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HeadIKSolution() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HeadIKSolution(HeadIKSolution other) : this() {
      success_ = other.success_;
      neckPosition_ = other.neckPosition_ != null ? other.neckPosition_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public HeadIKSolution Clone() {
      return new HeadIKSolution(this);
    }

    /// <summary>Field number for the "success" field.</summary>
    public const int SuccessFieldNumber = 1;
    private bool success_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Success {
      get { return success_; }
      set {
        success_ = value;
      }
    }

    /// <summary>Field number for the "neck_position" field.</summary>
    public const int NeckPositionFieldNumber = 2;
    private global::Reachy.Sdk.Kinematics.JointPosition neckPosition_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Reachy.Sdk.Kinematics.JointPosition NeckPosition {
      get { return neckPosition_; }
      set {
        neckPosition_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as HeadIKSolution);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(HeadIKSolution other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Success != other.Success) return false;
      if (!object.Equals(NeckPosition, other.NeckPosition)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Success != false) hash ^= Success.GetHashCode();
      if (neckPosition_ != null) hash ^= NeckPosition.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (Success != false) {
        output.WriteRawTag(8);
        output.WriteBool(Success);
      }
      if (neckPosition_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(NeckPosition);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (Success != false) {
        output.WriteRawTag(8);
        output.WriteBool(Success);
      }
      if (neckPosition_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(NeckPosition);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Success != false) {
        size += 1 + 1;
      }
      if (neckPosition_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(NeckPosition);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(HeadIKSolution other) {
      if (other == null) {
        return;
      }
      if (other.Success != false) {
        Success = other.Success;
      }
      if (other.neckPosition_ != null) {
        if (neckPosition_ == null) {
          NeckPosition = new global::Reachy.Sdk.Kinematics.JointPosition();
        }
        NeckPosition.MergeFrom(other.NeckPosition);
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Success = input.ReadBool();
            break;
          }
          case 18: {
            if (neckPosition_ == null) {
              NeckPosition = new global::Reachy.Sdk.Kinematics.JointPosition();
            }
            input.ReadMessage(NeckPosition);
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 8: {
            Success = input.ReadBool();
            break;
          }
          case 18: {
            if (neckPosition_ == null) {
              NeckPosition = new global::Reachy.Sdk.Kinematics.JointPosition();
            }
            input.ReadMessage(NeckPosition);
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code
