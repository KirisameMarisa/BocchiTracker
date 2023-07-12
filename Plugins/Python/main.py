import socket
import flatbuffers
from BocchiTracker.ProcessLinkQuery.Queries import Packet
from BocchiTracker.ProcessLinkQuery.Queries import QueryID
from BocchiTracker.ProcessLinkQuery.Queries import PlayerPosition
from BocchiTracker.ProcessLinkQuery.Queries import AppBasicInfo


def CreateAppBasicInfoPacket(inPid, inApplicationName, inArgs, inPlatform):
    builder = flatbuffers.Builder(0)
    appName = builder.CreateString(inApplicationName)
    args = builder.CreateString(inArgs)
    platform = builder.CreateString(inPlatform)

    # Create PlayerPosition object
    AppBasicInfo.AppBasicInfoStart(builder)
    AppBasicInfo.AddPid(builder, inPid)
    AppBasicInfo.AddAppName(builder, appName)
    AppBasicInfo.AddArgs(builder, args)
    AppBasicInfo.AddPlatform(builder, platform)
    table = AppBasicInfo.AppBasicInfoEnd(builder)

    # Create Packet object
    Packet.PacketStart(builder)
    Packet.PacketAddQueryIdType(builder, QueryID.QueryID.AppBasicInfo)
    Packet.PacketAddQueryId(builder, table)
    packet = Packet.PacketEnd(builder)

    builder.Finish(packet)

    # パケットのサイズを先頭に追加
    packet_bin = builder.Output() 
    packet_size = len(packet_bin)
    size_data = packet_size.to_bytes(4, 'little')

    return size_data + packet_bin

def CreatePlayerPositionPacket(x, y, z, stage):
    builder = flatbuffers.Builder(0)
    stage = builder.CreateString(stage)

    # Create PlayerPosition object
    PlayerPosition.PlayerPositionStart(builder)
    PlayerPosition.PlayerPositionAddX(builder, x)
    PlayerPosition.PlayerPositionAddY(builder, y)
    PlayerPosition.PlayerPositionAddZ(builder, z)
    PlayerPosition.PlayerPositionAddStage(builder, stage)
    player_position = PlayerPosition.PlayerPositionEnd(builder)

    # Create Packet object
    Packet.PacketStart(builder)
    Packet.PacketAddQueryIdType(builder, QueryID.QueryID.PlayerPosition)
    Packet.PacketAddQueryId(builder, player_position)
    packet = Packet.PacketEnd(builder)

    builder.Finish(packet)

    # パケットのサイズを先頭に追加
    packet_bin = builder.Output() 
    packet_size = len(packet_bin)
    size_data = packet_size.to_bytes(4, 'little')

    return size_data + packet_bin

def main():
    host = 'localhost'  # 接続先のホスト名またはIPアドレス
    port = 8888  # 接続先のポート番号

    # サーバーに接続
    client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    client_socket.connect((host, port))

    try:
        app_basic_info = CreateAppBasicInfoPacket(10009, "Python", "", "Windows")
        client_socket.sendall(app_basic_info)

        player_position_packet = CreatePlayerPositionPacket(10.0, 5.0, 3.0, "Stage 1")
        client_socket.sendall(player_position_packet)

    finally:
        # 接続を閉じる
        client_socket.close()

if __name__ == '__main__':
    main()