#pragma once

#include "UObject/Object.h"
#include "CoreMinimal.h"

class FBocchiTrackerOutputDevice : public FOutputDevice
{
public:
	FBocchiTrackerOutputDevice();

	virtual void Serialize( const TCHAR* InData, ELogVerbosity::Type Verbosity, const class FName& Category ) override;

    virtual bool GetLog(FString& outData);

public:
	FBocchiTrackerOutputDevice(FBocchiTrackerOutputDevice&&) = default;
	FBocchiTrackerOutputDevice(const FBocchiTrackerOutputDevice&) = default;
	FBocchiTrackerOutputDevice& operator=(FBocchiTrackerOutputDevice&&) = default;
	FBocchiTrackerOutputDevice& operator=(const FBocchiTrackerOutputDevice&) = default;

private:
    static const int MAX_BUFFER = 2;
    static const int LOG_MAX_BUFFER = 1024;
    FString mBuffer[MAX_BUFFER];
    int mCurrentId = 0;
};