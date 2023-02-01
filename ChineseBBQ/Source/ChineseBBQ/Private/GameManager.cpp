// Fill out your copyright notice in the Description page of Project Settings.


#include "GameManager.h"
#include "ChineseBBQGameState.h"
#include "Kismet/GameplayStatics.h"
#include "ChineseBBQ/ChineseBBQCharacter.h"
#include "PhoneUIInterface.h"
#include "cmath"

// Sets default values
AGameManager::AGameManager()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

}

// Called when the game starts or when spawned
void AGameManager::BeginPlay()
{
	Super::BeginPlay();
	//TObjectPtr<AChineseBBQGameState> GameState = Cast<AChineseBBQGameState>(GetWorld()->GetGameState());
	GetWorldTimerManager().SetTimer(TimerHandle, this, &AGameManager::AddOrder, 0.5, false);
}

// Called every frame
void AGameManager::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
}

void AGameManager::AddOrder()
{
	TObjectPtr<AChineseBBQGameState> GameState = Cast<AChineseBBQGameState>(GetWorld()->GetGameState());
	if (GameState->orderNum + 1 <= GameState->maxOrderNum)
	{
		TMap<TEnumAsByte<SpicyValue>, int> newRequirement;
		int foodNum = rand() % 4 + 1;
		for (int i = 0; i < foodNum; ++i)
		{
			TEnumAsByte<SpicyValue> spicyValue{ rand() % 3 };
			if (newRequirement.Contains(spicyValue))
				newRequirement[spicyValue] += 1;
			else
				newRequirement.Add(spicyValue, 1);
		}

		TEnumAsByte <ChinaCity> city{ rand() % 6 };
		FString name;
		if (GameState->NameList.Num() <= 0)
			name = TEXT("John Smith");
		else
			name = GameState->NameList[rand() % GameState->NameList.Num()];
		FOrder newOrder{
			GameState->totalOrderNum, name, newRequirement, city
		};

		GameState->orderList.Add(newOrder);

		TObjectPtr<AChineseBBQCharacter> player = Cast< AChineseBBQCharacter>(UGameplayStatics::GetPlayerPawn(GetWorld(), 0));
		if (player->phoneUI)
		{
			if (player->phoneUI->Implements<UPhoneUIInterface>())
			{
				IPhoneUIInterface::Execute_AddOrders(player->phoneUI, newOrder);
			}
		}
		++GameState->orderNum;
		++GameState->totalOrderNum;
	}
}

bool AGameManager::FinishOrder(int orderID, TArray<float> spicyValueList, TArray<float> cookValueList)
{
	TMap<int ,int> realSpicyLevel;
	float reputation = 0;
	float money = 0;
	int qualifyedNum = 0;
	int totalNum = spicyValueList.Num();
	TObjectPtr<AChineseBBQGameState> GameState = Cast<AChineseBBQGameState>(GetWorld()->GetGameState());
	int spicyModifier = GameState->CityToSpicyMap[GameState->orderList[orderID].Location];
	for (int i = 0; i < spicyValueList.Num(); ++i)
	{
		if (spicyValueList[i] == 0)
			if (realSpicyLevel.Contains(NoSpicy))
				realSpicyLevel[0] += 1;
			else
				realSpicyLevel.Add(0,1);
		else
		{
			int temp = int(spicyValueList[i]) / 25 + 1;
			if (realSpicyLevel.Contains(temp))
				realSpicyLevel[temp] +=1;
			else
				realSpicyLevel.Add(temp, 1);
		}
	}

	for (int i = 0; i < cookValueList.Num(); ++i)
	{
		if (cookValueList[i] >= 90 && cookValueList[i] <= 150)
			qualifyedNum++;
	}

	money += qualifyedNum * GameState->price / 2.0;
	reputation -= (totalNum - qualifyedNum) * 0.25;

	TMap<int, int> reqMap;
	int rightSpicyNum = 0;
	for (auto temp : GameState->orderList[orderID].RequirementsMap)
	{
		reqMap.Add(temp.Key + spicyModifier, temp.Value);
	}

	for (auto req : reqMap)
	{
		for (int i = 0; i < req.Value; i++)
		{
			if (realSpicyLevel.Contains(req.Key) && realSpicyLevel[req.Key] > 0)
			{
				realSpicyLevel[req.Key] -= 1;
				money += GameState->price / 2.0;
				reputation += 0.5;
				rightSpicyNum++;
			}
			else
				if (int(req.Key) - 1>=0 && realSpicyLevel.Contains(int(req.Key) - 1) && realSpicyLevel[int(req.Key) - 1] > 0)
				{
					realSpicyLevel[int(req.Key) - 1] -= 1;
					money += GameState->price / 4.0;
					reputation += 0.25;
					rightSpicyNum++;
				}
		}
	}

	for (auto temp : realSpicyLevel)
	{
		for (int i = 0; i < temp.Value; i++)
		{
			if (int(temp.Key) + 1 <= 4 && reqMap.Contains(int(temp.Key) + 1) && reqMap[int(temp.Key) + 1] > 0)
			{
				money += GameState->price / 4 + 2;
				reputation += 0.25;
				rightSpicyNum++;
			}
		}
	}

	reputation -= (totalNum - rightSpicyNum) * 0.25;
	GameState->reputation += reputation;
	GameState->money += money * GetValueModifier();
	GameState->orderList.RemoveAt(orderID);
	--GameState->orderNum;
	TObjectPtr<AChineseBBQCharacter> player = Cast< AChineseBBQCharacter>(UGameplayStatics::GetPlayerPawn(GetWorld(), 0));
	if (player->phoneUI)
	{
		if (player->phoneUI->Implements<UPhoneUIInterface>())
		{
			IPhoneUIInterface::Execute_RemoveOrders(player->phoneUI);
		}
	}
	AddOrder();
	return true;
}

float AGameManager::GetValueModifier()
{
	TObjectPtr<AChineseBBQGameState> GameState = Cast<AChineseBBQGameState>(GetWorld()->GetGameState());
	return std::pow(FMath::Max(GameState->reputation, 0), 1 / 3.0);
}

