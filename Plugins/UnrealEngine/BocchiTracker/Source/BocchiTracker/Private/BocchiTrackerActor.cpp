#include "BocchiTrackerActor.h"
#include "BocchiTrackerSettings.h"
#include "BocchiTrackerTcpSocket.h"
#include "Kismet/GameplayStatics.h"
#include "GameFramework/PlayerController.h"

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
}

void ABocchiTrackerActor::Tick(float DeltaSeconds)
{
    Super::Tick(DeltaSeconds);

    if (const auto PlayerControler = UGameplayStatics::GetPlayerController(this, 0))
    {
        // Update the tracked player's position in each tick
        const auto& Position = PlayerControler->GetPawn()->GetActorLocation();

        UE_LOG(LogTemp, Warning, TEXT("Tracked Player Position: X=%.2f, Y=%.2f, Z=%.2f"), Position.X, Position.Y, Position.Z);
    }
}

void ABocchiTrackerActor::OnReciveData(TArray<uint8> inData)
{
    //!< TODO::
}
