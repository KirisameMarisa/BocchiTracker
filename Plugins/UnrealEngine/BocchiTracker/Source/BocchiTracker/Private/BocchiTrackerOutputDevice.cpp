#include "BocchiTrackerOutputDevice.h"
#include "HAL/PlatformAtomics.h"

FBocchiTrackerOutputDevice::FBocchiTrackerOutputDevice()
{
    bAutoEmitLineTerminator = false;
}

void FBocchiTrackerOutputDevice::Serialize( const TCHAR* InData, ELogVerbosity::Type Verbosity, const class FName& Category )
{
    if (GEngine && GEngine->GameViewport && GEngine->GameViewport->Viewport)
    {
        mBuffer[mCurrentId].Append((TCHAR*)InData);
        mBuffer[mCurrentId].Append(LINE_TERMINATOR);
    }
}

bool FBocchiTrackerOutputDevice::GetLog(FString& outData)
{
    if (mBuffer[mCurrentId].Len() > LOG_MAX_BUFFER)
    {
        outData = mBuffer[mCurrentId];
        int PreviousId = mCurrentId;
        FPlatformAtomics::InterlockedExchange(&mCurrentId, (mCurrentId + 1) % 2);

        mBuffer[PreviousId] = "";
        return true;
    }

    return false;
}