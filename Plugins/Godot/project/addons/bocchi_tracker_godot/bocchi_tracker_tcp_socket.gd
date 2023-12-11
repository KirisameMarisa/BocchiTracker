extends Node

signal receive_callback(data: PackedByteArray)
signal on_connect()
signal on_disconnect()

var server_address:String = ""
var server_port:int = 0
var socket: StreamPeerTCP = StreamPeerTCP.new()

var send_data_queue := []

enum ConnectStatus {
	Connect,
	Disconnect,
	Connecting,
}
var connect_status:ConnectStatus = ConnectStatus.Connect

func update(delta):
	if send_data_queue.size() > 0:
		var data = send_data_queue.pop_front()
		print(send_data_queue.size())
		_process_send_data(data)
	_process_receive_data()

# Initiates a connection to the specified IP address and port.
func initialize(ip_address, port):
	server_address = ip_address
	server_port = port

# Adds data to the send queue for transmission.
func add_send_data(data):
	send_data_queue.append(data)

# Process send data
func _process_send_data(data):
	if not is_connect():
		return

	var bytes_sent = socket.put_data(data)
	print("Putting data:", data, "(", len(data), ")")
	var error: int = socket.put_data(data)
	if error != OK:
		print("Error writing to stream: ", error)
		return false
	return true

# Process receive data
func _process_receive_data():
	if not is_connect():
		return

	var available_bytes: int = socket.get_available_bytes()
	if available_bytes > 0:
		var received_data: Array = socket.get_partial_data(available_bytes)
		var status_code = received_data[0]
		var data = received_data[1]
		if status_code == Error.OK:
			print("ProcessReceiveData::Success, size=" + str(data.size()))
			emit_signal("receive_callback", data)
		
		
func _process_connect():
			
	match connect_status:
		
		ConnectStatus.Connect:
			socket.connect_to_host(server_address, server_port)
			connect_status = ConnectStatus.Connecting
			emit_signal("on_connect")
			return false
			
		ConnectStatus.Disconnect:
			socket.disconnect_from_host()
			connect_status = ConnectStatus.Connect
			emit_signal("on_disconnect")
			return false
			
		ConnectStatus.Connecting:
			return true
			
# Checks if the socket is currently connected.
func is_connect():
	if socket == null:
		return false
		
	if server_address == "" or server_port == 0:
		return false
		
	if not _process_connect():
		return false
		
	if socket.poll() != Error.OK:
		return false
	
	var status = socket.get_status()
	match status:
		
		socket.STATUS_NONE:
			connect_status = ConnectStatus.Disconnect
			return false
			
		socket.STATUS_CONNECTING:
			return false
			
		socket.STATUS_CONNECTED:
			return true
			
		socket.STATUS_ERROR:
			connect_status = ConnectStatus.Disconnect
			return false
