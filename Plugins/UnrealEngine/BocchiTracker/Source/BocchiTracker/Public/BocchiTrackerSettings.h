#pragma once

#include "CoreMinimal.h"
#include "BocchiTrackerSettings.generated.h"

UCLASS(config = Engine, defaultconfig)
class UBocchiTrackerSettings : public UObject
{
	GENERATED_BODY()
	
public:
	UPROPERTY(Config, EditAnywhere, Category = "BocchiTracker")
	FString IPAddress;

	UPROPERTY(Config, EditAnywhere, Category = "BocchiTracker")
	int Port = 8888;
};