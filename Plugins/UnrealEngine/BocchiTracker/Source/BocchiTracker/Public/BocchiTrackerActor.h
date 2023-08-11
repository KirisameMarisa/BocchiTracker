#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "BocchiTrackerActor.generated.h"

class UBocchiTrackerSettings;
class FBocchiTrackerTcpSocket;

UCLASS()
class ABocchiTrackerActor : public AActor
{
    GENERATED_BODY()

public:
    ABocchiTrackerActor();

protected:
    virtual void BeginPlay() override;

    virtual void Tick(float DeltaSeconds) override;

private:
    void OnReciveData(TArray<uint8> inData);

private:
    const UBocchiTrackerSettings* Settings;
    TUniquePtr<FBocchiTrackerTcpSocket> Socket;
};