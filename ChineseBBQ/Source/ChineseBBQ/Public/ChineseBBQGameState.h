// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/GameStateBase.h"
#include "ChineseBBQGameState.generated.h"

/**
 * 
 */

class ABaseInteractor;

UENUM(BlueprintType)
enum ChinaCity
{
	Guangdong	UMETA(DisplayName = "Guangdong"),
	Zhejiang		UMETA(DisplayName = "Zhejiang"),
	Shandong	UMETA(DisplayName = "Shandong"),
	Beijing			UMETA(DisplayName = "Beijing"),
	Sichuan		UMETA(DisplayName = "Sichuan"),
	Hunan			UMETA(DisplayName = "Hunan")
};

UENUM(BlueprintType)
enum SpicyValue
{
	NoSpicy		UMETA(DisplayName = "NoSpicy"),
	LittleSpicy	UMETA(DisplayName = "LittleSpicy"),
	Spicy			UMETA(DisplayName = "Spicy")
};

USTRUCT(BlueprintType)
struct FOrder
{
	GENERATED_BODY()
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Status)
	int orderID;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Status)
	FString OwnerName;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Status)
	TMap<TEnumAsByte<SpicyValue>, int> RequirementsMap;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Status)
	TEnumAsByte<ChinaCity> Location;

	FOrder() {};
	FOrder(int _orderID, FString _OwnerName, TMap<TEnumAsByte<SpicyValue>, int> _RequirementsMap, TEnumAsByte<ChinaCity> _Location) :
		orderID(_orderID), OwnerName(_OwnerName), RequirementsMap(_RequirementsMap), Location(_Location) {};
};

UCLASS(Blueprintable)
class CHINESEBBQ_API AChineseBBQGameState : public AGameStateBase
{
	GENERATED_BODY()

public:
	AChineseBBQGameState();

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Status)
		float orderInterval;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Status)
		int maxOrderNum;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Status)
		TArray<FOrder> orderList;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Status)
		int orderNum; 
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Status)
		int totalOrderNum;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Status)
		float reputation;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Status)
		float money;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Status)
		float matchLength;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Status)
		float price;
	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category = Status)
		TMap<TEnumAsByte<ChinaCity>, int> CityToSpicyMap;

	TArray<FString> NameList;

public:

};
