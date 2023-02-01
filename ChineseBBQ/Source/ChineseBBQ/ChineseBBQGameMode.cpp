// Copyright Epic Games, Inc. All Rights Reserved.

#include "ChineseBBQGameMode.h"
#include "ChineseBBQCharacter.h"
#include "ChineseBBQGameState.h"
#include "Kismet/GameplayStatics.h"
#include "UObject/ConstructorHelpers.h"

AChineseBBQGameMode::AChineseBBQGameMode()
	: Super()
{
	// set default pawn class to our Blueprinted character
	static ConstructorHelpers::FClassFinder<APawn> PlayerPawnClassFinder(TEXT("/Game/Blueprints/BP_FirstPersonCharacter"));
	DefaultPawnClass = PlayerPawnClassFinder.Class;

	GameStateClass = AChineseBBQGameState::StaticClass();
}
