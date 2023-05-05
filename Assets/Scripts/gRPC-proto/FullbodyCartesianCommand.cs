// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: fullbody_cartesian_command.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Reachy.Sdk.Kinematics {

  /// <summary>Holder for reflection information generated from fullbody_cartesian_command.proto</summary>
  public static partial class FullbodyCartesianCommandReflection {

    #region Descriptor
    /// <summary>File descriptor for fullbody_cartesian_command.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static FullbodyCartesianCommandReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CiBmdWxsYm9keV9jYXJ0ZXNpYW5fY29tbWFuZC5wcm90bxIVcmVhY2h5LnNk",
            "ay5raW5lbWF0aWNzGhRhcm1fa2luZW1hdGljcy5wcm90bxoVaGVhZF9raW5l",
            "bWF0aWNzLnByb3RvIoABChtGdWxsQm9keUNhcnRlc2lhbkNvbW1hbmRBY2sS",
            "IAoYbGVmdF9hcm1fY29tbWFuZF9zdWNjZXNzGAEgASgIEiEKGXJpZ2h0X2Fy",
            "bV9jb21tYW5kX3N1Y2Nlc3MYAiABKAgSHAoUaGVhZF9jb21tYW5kX3N1Y2Nl",
            "c3MYAyABKAgivQEKGEZ1bGxCb2R5Q2FydGVzaWFuQ29tbWFuZBI1CghsZWZ0",
            "X2FybRgBIAEoCzIjLnJlYWNoeS5zZGsua2luZW1hdGljcy5Bcm1JS1JlcXVl",
            "c3QSNgoJcmlnaHRfYXJtGAIgASgLMiMucmVhY2h5LnNkay5raW5lbWF0aWNz",
            "LkFybUlLUmVxdWVzdBIyCgRoZWFkGAMgASgLMiQucmVhY2h5LnNkay5raW5l",
            "bWF0aWNzLkhlYWRJS1JlcXVlc3QyswIKH0Z1bGxCb2R5Q2FydGVzaWFuQ29t",
            "bWFuZFNlcnZpY2UShAEKHVNlbmRGdWxsQm9keUNhcnRlc2lhbkNvbW1hbmRz",
            "Ei8ucmVhY2h5LnNkay5raW5lbWF0aWNzLkZ1bGxCb2R5Q2FydGVzaWFuQ29t",
            "bWFuZBoyLnJlYWNoeS5zZGsua2luZW1hdGljcy5GdWxsQm9keUNhcnRlc2lh",
            "bkNvbW1hbmRBY2sSiAEKH1N0cmVhbUZ1bGxCb2R5Q2FydGVzaWFuQ29tbWFu",
            "ZHMSLy5yZWFjaHkuc2RrLmtpbmVtYXRpY3MuRnVsbEJvZHlDYXJ0ZXNpYW5D",
            "b21tYW5kGjIucmVhY2h5LnNkay5raW5lbWF0aWNzLkZ1bGxCb2R5Q2FydGVz",
            "aWFuQ29tbWFuZEFjaygBYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Reachy.Sdk.Kinematics.ArmKinematicsReflection.Descriptor, global::Reachy.Sdk.Kinematics.HeadKinematicsReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Reachy.Sdk.Kinematics.FullBodyCartesianCommandAck), global::Reachy.Sdk.Kinematics.FullBodyCartesianCommandAck.Parser, new[]{ "LeftArmCommandSuccess", "RightArmCommandSuccess", "HeadCommandSuccess" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Reachy.Sdk.Kinematics.FullBodyCartesianCommand), global::Reachy.Sdk.Kinematics.FullBodyCartesianCommand.Parser, new[]{ "LeftArm", "RightArm", "Head" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class FullBodyCartesianCommandAck : pb::IMessage<FullBodyCartesianCommandAck>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<FullBodyCartesianCommandAck> _parser = new pb::MessageParser<FullBodyCartesianCommandAck>(() => new FullBodyCartesianCommandAck());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<FullBodyCartesianCommandAck> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Reachy.Sdk.Kinematics.FullbodyCartesianCommandReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public FullBodyCartesianCommandAck() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public FullBodyCartesianCommandAck(FullBodyCartesianCommandAck other) : this() {
      leftArmCommandSuccess_ = other.leftArmCommandSuccess_;
      rightArmCommandSuccess_ = other.rightArmCommandSuccess_;
      headCommandSuccess_ = other.headCommandSuccess_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public FullBodyCartesianCommandAck Clone() {
      return new FullBodyCartesianCommandAck(this);
    }

    /// <summary>Field number for the "left_arm_command_success" field.</summary>
    public const int LeftArmCommandSuccessFieldNumber = 1;
    private bool leftArmCommandSuccess_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool LeftArmCommandSuccess {
      get { return leftArmCommandSuccess_; }
      set {
        leftArmCommandSuccess_ = value;
      }
    }

    /// <summary>Field number for the "right_arm_command_success" field.</summary>
    public const int RightArmCommandSuccessFieldNumber = 2;
    private bool rightArmCommandSuccess_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool RightArmCommandSuccess {
      get { return rightArmCommandSuccess_; }
      set {
        rightArmCommandSuccess_ = value;
      }
    }

    /// <summary>Field number for the "head_command_success" field.</summary>
    public const int HeadCommandSuccessFieldNumber = 3;
    private bool headCommandSuccess_;
    /// <summary>
    /// Previously was:
    /// bool neck_command_success = 3;
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HeadCommandSuccess {
      get { return headCommandSuccess_; }
      set {
        headCommandSuccess_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as FullBodyCartesianCommandAck);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(FullBodyCartesianCommandAck other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (LeftArmCommandSuccess != other.LeftArmCommandSuccess) return false;
      if (RightArmCommandSuccess != other.RightArmCommandSuccess) return false;
      if (HeadCommandSuccess != other.HeadCommandSuccess) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (LeftArmCommandSuccess != false) hash ^= LeftArmCommandSuccess.GetHashCode();
      if (RightArmCommandSuccess != false) hash ^= RightArmCommandSuccess.GetHashCode();
      if (HeadCommandSuccess != false) hash ^= HeadCommandSuccess.GetHashCode();
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
      if (LeftArmCommandSuccess != false) {
        output.WriteRawTag(8);
        output.WriteBool(LeftArmCommandSuccess);
      }
      if (RightArmCommandSuccess != false) {
        output.WriteRawTag(16);
        output.WriteBool(RightArmCommandSuccess);
      }
      if (HeadCommandSuccess != false) {
        output.WriteRawTag(24);
        output.WriteBool(HeadCommandSuccess);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (LeftArmCommandSuccess != false) {
        output.WriteRawTag(8);
        output.WriteBool(LeftArmCommandSuccess);
      }
      if (RightArmCommandSuccess != false) {
        output.WriteRawTag(16);
        output.WriteBool(RightArmCommandSuccess);
      }
      if (HeadCommandSuccess != false) {
        output.WriteRawTag(24);
        output.WriteBool(HeadCommandSuccess);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (LeftArmCommandSuccess != false) {
        size += 1 + 1;
      }
      if (RightArmCommandSuccess != false) {
        size += 1 + 1;
      }
      if (HeadCommandSuccess != false) {
        size += 1 + 1;
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(FullBodyCartesianCommandAck other) {
      if (other == null) {
        return;
      }
      if (other.LeftArmCommandSuccess != false) {
        LeftArmCommandSuccess = other.LeftArmCommandSuccess;
      }
      if (other.RightArmCommandSuccess != false) {
        RightArmCommandSuccess = other.RightArmCommandSuccess;
      }
      if (other.HeadCommandSuccess != false) {
        HeadCommandSuccess = other.HeadCommandSuccess;
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
            LeftArmCommandSuccess = input.ReadBool();
            break;
          }
          case 16: {
            RightArmCommandSuccess = input.ReadBool();
            break;
          }
          case 24: {
            HeadCommandSuccess = input.ReadBool();
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
            LeftArmCommandSuccess = input.ReadBool();
            break;
          }
          case 16: {
            RightArmCommandSuccess = input.ReadBool();
            break;
          }
          case 24: {
            HeadCommandSuccess = input.ReadBool();
            break;
          }
        }
      }
    }
    #endif

  }

  public sealed partial class FullBodyCartesianCommand : pb::IMessage<FullBodyCartesianCommand>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<FullBodyCartesianCommand> _parser = new pb::MessageParser<FullBodyCartesianCommand>(() => new FullBodyCartesianCommand());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<FullBodyCartesianCommand> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Reachy.Sdk.Kinematics.FullbodyCartesianCommandReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public FullBodyCartesianCommand() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public FullBodyCartesianCommand(FullBodyCartesianCommand other) : this() {
      leftArm_ = other.leftArm_ != null ? other.leftArm_.Clone() : null;
      rightArm_ = other.rightArm_ != null ? other.rightArm_.Clone() : null;
      head_ = other.head_ != null ? other.head_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public FullBodyCartesianCommand Clone() {
      return new FullBodyCartesianCommand(this);
    }

    /// <summary>Field number for the "left_arm" field.</summary>
    public const int LeftArmFieldNumber = 1;
    private global::Reachy.Sdk.Kinematics.ArmIKRequest leftArm_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Reachy.Sdk.Kinematics.ArmIKRequest LeftArm {
      get { return leftArm_; }
      set {
        leftArm_ = value;
      }
    }

    /// <summary>Field number for the "right_arm" field.</summary>
    public const int RightArmFieldNumber = 2;
    private global::Reachy.Sdk.Kinematics.ArmIKRequest rightArm_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Reachy.Sdk.Kinematics.ArmIKRequest RightArm {
      get { return rightArm_; }
      set {
        rightArm_ = value;
      }
    }

    /// <summary>Field number for the "head" field.</summary>
    public const int HeadFieldNumber = 3;
    private global::Reachy.Sdk.Kinematics.HeadIKRequest head_;
    /// <summary>
    /// Previously was:
    /// OrbitaIKRequest neck = 3;
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Reachy.Sdk.Kinematics.HeadIKRequest Head {
      get { return head_; }
      set {
        head_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as FullBodyCartesianCommand);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(FullBodyCartesianCommand other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(LeftArm, other.LeftArm)) return false;
      if (!object.Equals(RightArm, other.RightArm)) return false;
      if (!object.Equals(Head, other.Head)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (leftArm_ != null) hash ^= LeftArm.GetHashCode();
      if (rightArm_ != null) hash ^= RightArm.GetHashCode();
      if (head_ != null) hash ^= Head.GetHashCode();
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
      if (leftArm_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(LeftArm);
      }
      if (rightArm_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(RightArm);
      }
      if (head_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(Head);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (leftArm_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(LeftArm);
      }
      if (rightArm_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(RightArm);
      }
      if (head_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(Head);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (leftArm_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(LeftArm);
      }
      if (rightArm_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(RightArm);
      }
      if (head_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Head);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(FullBodyCartesianCommand other) {
      if (other == null) {
        return;
      }
      if (other.leftArm_ != null) {
        if (leftArm_ == null) {
          LeftArm = new global::Reachy.Sdk.Kinematics.ArmIKRequest();
        }
        LeftArm.MergeFrom(other.LeftArm);
      }
      if (other.rightArm_ != null) {
        if (rightArm_ == null) {
          RightArm = new global::Reachy.Sdk.Kinematics.ArmIKRequest();
        }
        RightArm.MergeFrom(other.RightArm);
      }
      if (other.head_ != null) {
        if (head_ == null) {
          Head = new global::Reachy.Sdk.Kinematics.HeadIKRequest();
        }
        Head.MergeFrom(other.Head);
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
            if (leftArm_ == null) {
              LeftArm = new global::Reachy.Sdk.Kinematics.ArmIKRequest();
            }
            input.ReadMessage(LeftArm);
            break;
          }
          case 18: {
            if (rightArm_ == null) {
              RightArm = new global::Reachy.Sdk.Kinematics.ArmIKRequest();
            }
            input.ReadMessage(RightArm);
            break;
          }
          case 26: {
            if (head_ == null) {
              Head = new global::Reachy.Sdk.Kinematics.HeadIKRequest();
            }
            input.ReadMessage(Head);
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
            if (leftArm_ == null) {
              LeftArm = new global::Reachy.Sdk.Kinematics.ArmIKRequest();
            }
            input.ReadMessage(LeftArm);
            break;
          }
          case 18: {
            if (rightArm_ == null) {
              RightArm = new global::Reachy.Sdk.Kinematics.ArmIKRequest();
            }
            input.ReadMessage(RightArm);
            break;
          }
          case 26: {
            if (head_ == null) {
              Head = new global::Reachy.Sdk.Kinematics.HeadIKRequest();
            }
            input.ReadMessage(Head);
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
