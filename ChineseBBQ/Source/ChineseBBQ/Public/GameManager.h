// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "GameManager.generated.h"

UCLASS()
class CHINESEBBQ_API AGameManager : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	AGameManager();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

private:
	FTimerHandle TimerHandle;

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;
	
	UFUNCTION(BlueprintCallable)
	void AddOrder();
	UFUNCTION(BlueprintCallable)
	bool FinishOrder(int orderID, TArray<float> spicyValueList, TArray<float> cookValueList);
	UFUNCTION(BlueprintCallable)
	float GetValueModifier();
};
