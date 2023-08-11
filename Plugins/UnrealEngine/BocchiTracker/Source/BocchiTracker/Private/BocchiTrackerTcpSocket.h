#pragma once

#include "UObject/Object.h"
#include "CoreMinimal.h"
#include "IPAddress.h"
#include "SocketTypes.h"

DECLARE_DELEGATE_OneParam(FBocchiTrackerReciveDelegate, TArray<uint8>);

class FInternetAddr;
class FBocchiTrackerReceiveWorker;

class FBocchiTrackerTcpSocket : public FRunnable
{
public:
	FBocchiTrackerTcpSocket();

	virtual ~FBocchiTrackerTcpSocket();

public:
	using Binary = TArray<uint8>;

	enum class ESocketState : uint8
	{
		None,
		Connecting,
		Connected,
		Error,
		Stopping,
	};

public:
    void Stop() override;

	uint32 Run() override;

	void CreateSocket(const FString& InIP, int32 InPort, const FBocchiTrackerReciveDelegate& InReciveCallback);

	void AddSendData(const Binary& InData) { SendDataQueue.Enqueue(InData); }

	bool IsConnect() const;

private:
	void ProcessOpenSocket();

	void ProcessSendData(TArray<uint8> InData);

	void ProcessReciveData();

private:
	bool bIsStopping = false;
	ESocketState SocketState = ESocketState::None;
	
	FSocket* Socket = nullptr;
	TSharedPtr<FInternetAddr> InternetAddress;
	FBocchiTrackerReciveDelegate OnRecive;
	TQueue<Binary> SendDataQueue;
	FRunnableThread* Thread = nullptr;
};