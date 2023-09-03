#include "BocchiTrackerActor.h"
#include "BocchiTrackerSettings.h"
#include "BocchiTrackerTcpSocket.h"
#include "BocchiTrackerPacket.h"
#include "Kismet/GameplayStatics.h"
#include "GameFramework/PlayerController.h"
#include "Engine/TextureRenderTarget2D.h"
#include "Engine/GameViewportClient.h"
#include "IImageWrapper.h"
#include "Async/TaskGraphInterfaces.h"

#include "flatbuffers/flatbuffers.h"
#include "Query_generated.h"

ABocchiTrackerActor::ABocchiTrackerActor() 
{
    PrimaryActorTick.bCanEverTick = true;
}

void ABocchiTrackerActor::BeginPlay()
{
    Super::BeginPlay();

    Settings = GetDefault<UBocchiTrackerSettings>();
    if(!Socket)
    {
        FBocchiTrackerReciveDelegate ReciveDelegate;
        ReciveDelegate.BindUObject(this, &ABocchiTrackerActor::OnReciveData);

        Socket = MakeUnique<FBocchiTrackerTcpSocket>();
        Socket->CreateSocket(Settings->IPAddress, Settings->Port, ReciveDelegate);
    }
    bSentAppBasicInfo = false;
}

void ABocchiTrackerActor::Tick(float DeltaSeconds)
{
    Super::Tick(DeltaSeconds);

    if(Socket->IsConnect())
    {
        ProcessSendAppBasicInfo();

        ProcessSendPlayerPosition();
       
        TArray<uint8> outPacket;
        if(PendingProcessRequest.Dequeue(outPacket))
        {
            const BocchiTracker::ProcessLinkQuery::Queries::Packet* packet 
                = BocchiTracker::ProcessLinkQuery::Queries::GetPacket(outPacket.GetData());
            
            switch (packet->query_id_type())
            {
                case BocchiTracker::ProcessLinkQuery::Queries::QueryID::QueryID_ScreenshotRequest:
                    ProcessSendScreenshot(); break;
                
                case BocchiTracker::ProcessLinkQuery::Queries::QueryID::QueryID_IssueesRequest:
                    break;

                case BocchiTracker::ProcessLinkQuery::Queries::QueryID::QueryID_JumpRequest:
                    {
                        const BocchiTracker::ProcessLinkQuery::Queries::JumpRequest* jumpRequest = packet->query_id_as_JumpRequest();
                        BocchiTrackerLocation data;
                        data.Location = FVector(jumpRequest->location()->x(), jumpRequest->location()->y(), jumpRequest->location()->z());
                        data.Stage = FString(jumpRequest->stage()->c_str());  
                        PendingJumpRequest.Enqueue(data);
                    }
                    break;

                default:
                    break;
            }
        }
    }
}

void ABocchiTrackerActor::OnReciveData(TArray<uint8> inData)
{
    PendingProcessRequest.Enqueue(inData);
}

void ABocchiTrackerActor::ProcessSendPlayerPosition()
{
    static FVector PreviousPosition;
    FVector CurrentPosition = TrackedPosition;

    float Distance = FVector::Dist(CurrentPosition, PreviousPosition);
    if (Distance >= 100.0f)
    {
        auto PlayerPositionPacket = CreatePacketHelper::CreatePlayerPosition(TrackedPosition, Stage);
        Socket->AddSendData(PlayerPositionPacket);
        PreviousPosition = CurrentPosition;
    }
}

void ABocchiTrackerActor::ProcessSendAppBasicInfo()
{
    if(bSentAppBasicInfo)
        return;

    auto ApplicationBsicInformationPacket = CreatePacketHelper::CreateApplicationBsicInformation();
    Socket->AddSendData(ApplicationBsicInformationPacket);

    bSentAppBasicInfo = true;
}

void ABocchiTrackerActor::ProcessSendScreenshot()
{
    if(!Socket->IsConnect())
        return;

    UGameViewportClient* GameViewportClient = GEngine->GameViewport;
    if (!GameViewportClient)
        return;

    FIntPoint ViewportSize = GameViewportClient->Viewport->GetSizeXY();
    FIntRect ScreenshotRect(0, 0, ViewportSize.X, ViewportSize.Y);

    TArray<FColor> Bitmap;
    if (!GameViewportClient->Viewport->ReadPixels(Bitmap, FReadSurfaceDataFlags(), ScreenshotRect))
        return;

    TArray<uint8> ScreenshotData;
    for (const FColor& PixelColor : Bitmap)
    {
        ScreenshotData.Add(PixelColor.R);
        ScreenshotData.Add(PixelColor.G);
        ScreenshotData.Add(PixelColor.B);
        ScreenshotData.Add(255);
    }

    auto ScreenshotDataPacket = CreatePacketHelper::CreateScreenshotData(ViewportSize.X, ViewportSize.Y, ScreenshotData);
    Socket->AddSendData(ScreenshotDataPacket);
}

void ABocchiTrackerActor::SetPlayerPosition(const FVector &InTrackedPosition, const FString &InStage)
{
    TrackedPosition = InTrackedPosition;
    Stage = InStage;
}

void ABocchiTrackerActor::JumpIssueLocation(AActor* inActor)
{
    BocchiTrackerLocation outLocation;
    if(PendingJumpRequest.Dequeue(outLocation))
    {
        if (inActor)
        {
            FVector NewLocation = outLocation.Location;
            inActor->SetActorLocation(NewLocation);
        }
    }
}