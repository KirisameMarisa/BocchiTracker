#pragma once

#include "CoreMinimal.h"
#include "Math/Vector.h"

class CreatePacketHelper
{
public:
    static TArray<uint8> CreatePlayerPosition(const FVector& inPlayerPosition, const FString& inStage);

    static TArray<uint8> CreateApplicationBsicInformation();

    static TArray<uint8> CreateScreenshotData(int inWidth, int inHeight, const TArray<uint8>& inData);

    static TArray<uint8> CreateLogData(const FString& inData);
};