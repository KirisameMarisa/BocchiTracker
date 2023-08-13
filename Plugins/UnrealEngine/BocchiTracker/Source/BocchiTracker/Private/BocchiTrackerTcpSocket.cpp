#include "BocchiTrackerTcpSocket.h"
#include "Common/TcpSocketBuilder.h"
#include "BocchiTrackerPrivate.h"

FBocchiTrackerTcpSocket::FBocchiTrackerTcpSocket() 
    : bIsStopping()
    , Socket()
    , InternetAddress()
    , OnRecive()
    , SendDataQueue()
    , Thread()
{
}

FBocchiTrackerTcpSocket::~FBocchiTrackerTcpSocket()
{
    if(Thread != nullptr)
    {
        Thread->Kill(true);
		delete Thread;
		Thread = nullptr;
    }

    if (Socket)
	{
		ISocketSubsystem* SocketSubsystem = ISocketSubsystem::Get(PLATFORM_SOCKETSUBSYSTEM);
		SocketSubsystem->DestroySocket(Socket);
	}
}

void FBocchiTrackerTcpSocket::Stop()
{
    SocketState = ESocketState::Stopping;
}

uint32 FBocchiTrackerTcpSocket::Run()
{
    while(true)
    {
        ESocketState CurrentSocketState = SocketState;
        if(IsConnect())
        {
            ISocketSubsystem* SocketSubsystem = ISocketSubsystem::Get(PLATFORM_SOCKETSUBSYSTEM);
            const ESocketErrors LastErrorCode = SocketSubsystem->GetLastErrorCode();
            if(LastErrorCode > ESocketErrors::SE_EINPROGRESS)
            {
                CurrentSocketState = ESocketState::Error;	
            }
        }

        switch(CurrentSocketState)
        {
            case ESocketState::Stopping:
            return 0;

            case ESocketState::Connected:
                {
                    Binary OutData;
                    while(SendDataQueue.Dequeue(OutData))
		            {
                        ProcessSendData(OutData);
                    }
                    ProcessReciveData();
                }
            break;

            case ESocketState::Connecting:
            case ESocketState::Error:
            default:
                {
                    const ESocketConnectionState ConnectionState = Socket->GetConnectionState();
                    if(ConnectionState == ESocketConnectionState::SCS_Connected)
                    {
                        SocketState = ESocketState::Connected;
                    }
                    // Socket isn't connected
                    else
                    {
                        ProcessOpenSocket();
                    }
                }
            break;
        }
        FPlatformProcess::Sleep(1.f);
    }
    return 0;
}

void FBocchiTrackerTcpSocket::CreateSocket(const FString &InIP, int32 inPort, const FBocchiTrackerReciveDelegate& InDelegate)
{
    OnRecive = InDelegate;
    if(ISocketSubsystem* SocketSubsystem = ISocketSubsystem::Get(PLATFORM_SOCKETSUBSYSTEM))
    {
        FString IPAddress = InIP;
        if(IPAddress.IsEmpty())
        {
            bool bCanBindAll;
            TSharedPtr<class FInternetAddr> Addr = SocketSubsystem->GetLocalHostAddr(*GLog, bCanBindAll);
            IPAddress = Addr->ToString(false);
        }

        InternetAddress = SocketSubsystem->CreateInternetAddr();

        bool IsValid = false;
        InternetAddress->SetIp(*IPAddress, IsValid);
        InternetAddress->SetPort(inPort);

        Socket = FTcpSocketBuilder(TEXT("BocchiTracker.TcpSocket"))
            .WithSendBufferSize(BOCCHI_TRACKER_SEND_BUFFER_SIZE)
            .WithReceiveBufferSize(BOCCHI_TRACKER_RECEIVE_BUFFER_SIZE);
        Socket->SetNonBlocking(true);
    }
    SocketState = ESocketState::None;
    Thread = FRunnableThread::Create(this, TEXT("FBocchiTrackerTcpSocketThread"));
}

void FBocchiTrackerTcpSocket::ProcessOpenSocket()
{
    if(Socket)
	{
		SocketState = ESocketState::Connecting;
		
		const double TimeoutTime = FPlatformTime::Seconds() + 10.0f;
		do
		{
			if(Socket->Connect(*InternetAddress))
			{
				const ESocketConnectionState SocketConnectionState = Socket->GetConnectionState();
				SocketState = SocketConnectionState == SCS_NotConnected
								? ESocketState::Connecting
								: ESocketState::Error; // ESocketConnectionState maps to EMQTTSocketState
				return;
			}
			FPlatformProcess::SleepNoStats(0.5f);
 		}
		while(FPlatformTime::Seconds() < TimeoutTime && SocketState != ESocketState::Stopping);

		if(SocketState != ESocketState::Stopping)
		{
			SocketState = ESocketState::Error;
		}
	}
}

void FBocchiTrackerTcpSocket::ProcessSendData(TArray<uint8> InData)
{
    if(!IsConnect())
        return;

    const uint8 *Data = InData.GetData();
    int32 DataSize = InData.Num();
    int32 BytesSent = 0;

    if (Socket->Send(Data, DataSize, BytesSent))
    {
        UE_LOG(LogTemp, Log, TEXT("Data sent successfully: %d bytes"), BytesSent);
    }
    else
    {
        UE_LOG(LogTemp, Error, TEXT("Failed to send data."));
    }
}

void FBocchiTrackerTcpSocket::ProcessReciveData()
{
    if(!IsConnect())
        return;

    TArray<uint8> ReceivedData;
    ReceivedData.SetNumUninitialized(1024);
    int32 BytesRead = 0;
    if (Socket->Recv(ReceivedData.GetData(), ReceivedData.Num(), BytesRead))
    {
        if (BytesRead > 0)
        {
            UE_LOG(LogTemp, Error, TEXT("ProcessReciveData::Success, size=%d"), BytesRead);
            ReceivedData.SetNum(BytesRead);
            OnRecive.ExecuteIfBound(ReceivedData);
        }
    }
}

bool FBocchiTrackerTcpSocket::IsConnect() const 
{
    return SocketState == ESocketState::Connected && Socket && Socket->GetConnectionState() == ESocketConnectionState::SCS_Connected;
}
