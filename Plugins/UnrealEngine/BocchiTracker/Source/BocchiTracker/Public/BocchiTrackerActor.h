#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "BocchiTrackerActor.generated.h"

class UBocchiTrackerSettings;
class FBocchiTrackerTcpSocket;

class BocchiTrackerLocation
{        
public:
	FVector Location;
	FString Stage;
};

UCLASS(Blueprintable, BlueprintType)
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

    void ProcessSendPlayerPosition();

    void ProcessSendAppBasicInfo();

    void ProcessSendScreenshot();

public:
    UFUNCTION(BlueprintCallable, Category="BocchiTracker")
    void SetPlayerPosition(const FVector& InTrackedPosition, const FString& InStage);

    UFUNCTION(BlueprintCallable, Category="BocchiTracker")
    void JumpIssueLocation(AActor* inActor);

private:
    FVector TrackedPosition;
    FString Stage;

private:
    bool bSentAppBasicInfo;
    const UBocchiTrackerSettings* Settings;
    TUniquePtr<FBocchiTrackerTcpSocket> Socket;
    TQueue<TArray<uint8>> PendingProcessRequest;
    TQueue<BocchiTrackerLocation> PendingJumpRequest;
};