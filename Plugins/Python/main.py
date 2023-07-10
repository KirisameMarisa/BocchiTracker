import socket
import flatbuffers
from BocchiTracker.ProcessLinkQuery.Queries import Packet
from BocchiTracker.ProcessLinkQuery.Queries import QueryID
from BocchiTracker.ProcessLinkQuery.Queries import PlayerPosition


def create_player_position_packet(x, y, z, stage):
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
    port = 12345  # 接続先のポート番号

    # サーバーに接続
    client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    client_socket.connect((host, port))

    try:
        # PlayerPositionパケットを作成
        player_position_packet = create_player_position_packet(10.0, 5.0, 3.0, "Stage 1")

        # パケットをサーバーに送信
        client_socket.sendall(player_position_packet)

        # サーバーからのレスポンスの受信
        data = client_socket.recv(1024)
        print(f'Received from server: {data.decode()}')

    finally:
        # 接続を閉じる
        client_socket.close()

if __name__ == '__main__':
    main()