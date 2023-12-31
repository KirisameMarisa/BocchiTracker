// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace BocchiTracker.ProcessLinkQuery.Queries
{

using global::System;
using global::System.Collections.Generic;
using global::Google.FlatBuffers;

public struct PlayerPosition : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_5_26(); }
  public static PlayerPosition GetRootAsPlayerPosition(ByteBuffer _bb) { return GetRootAsPlayerPosition(_bb, new PlayerPosition()); }
  public static PlayerPosition GetRootAsPlayerPosition(ByteBuffer _bb, PlayerPosition obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public PlayerPosition __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public float X { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetFloat(o + __p.bb_pos) : (float)0.0f; } }
  public float Y { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetFloat(o + __p.bb_pos) : (float)0.0f; } }
  public float Z { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetFloat(o + __p.bb_pos) : (float)0.0f; } }
  public string Stage { get { int o = __p.__offset(10); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetStageBytes() { return __p.__vector_as_span<byte>(10, 1); }
#else
  public ArraySegment<byte>? GetStageBytes() { return __p.__vector_as_arraysegment(10); }
#endif
  public byte[] GetStageArray() { return __p.__vector_as_array<byte>(10); }

  public static Offset<BocchiTracker.ProcessLinkQuery.Queries.PlayerPosition> CreatePlayerPosition(FlatBufferBuilder builder,
      float x = 0.0f,
      float y = 0.0f,
      float z = 0.0f,
      StringOffset stageOffset = default(StringOffset)) {
    builder.StartTable(4);
    PlayerPosition.AddStage(builder, stageOffset);
    PlayerPosition.AddZ(builder, z);
    PlayerPosition.AddY(builder, y);
    PlayerPosition.AddX(builder, x);
    return PlayerPosition.EndPlayerPosition(builder);
  }

  public static void StartPlayerPosition(FlatBufferBuilder builder) { builder.StartTable(4); }
  public static void AddX(FlatBufferBuilder builder, float x) { builder.AddFloat(0, x, 0.0f); }
  public static void AddY(FlatBufferBuilder builder, float y) { builder.AddFloat(1, y, 0.0f); }
  public static void AddZ(FlatBufferBuilder builder, float z) { builder.AddFloat(2, z, 0.0f); }
  public static void AddStage(FlatBufferBuilder builder, StringOffset stageOffset) { builder.AddOffset(3, stageOffset.Value, 0); }
  public static Offset<BocchiTracker.ProcessLinkQuery.Queries.PlayerPosition> EndPlayerPosition(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<BocchiTracker.ProcessLinkQuery.Queries.PlayerPosition>(o);
  }
}


static public class PlayerPositionVerify
{
  static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
  {
    return verifier.VerifyTableStart(tablePos)
      && verifier.VerifyField(tablePos, 4 /*X*/, 4 /*float*/, 4, false)
      && verifier.VerifyField(tablePos, 6 /*Y*/, 4 /*float*/, 4, false)
      && verifier.VerifyField(tablePos, 8 /*Z*/, 4 /*float*/, 4, false)
      && verifier.VerifyString(tablePos, 10 /*Stage*/, false)
      && verifier.VerifyTableEnd(tablePos);
  }
}

}
