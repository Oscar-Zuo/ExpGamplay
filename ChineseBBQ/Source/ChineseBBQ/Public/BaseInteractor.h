// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "BaseInteractor.generated.h"

UCLASS(Blueprintable)
class CHINESEBBQ_API ABaseInteractor : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	ABaseInteractor();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Status)
	bool IsGrabbed = false;

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;
	virtual void Grabbed();
	virtual void Released();
	UFUNCTION(BlueprintImplementableEvent)
	void GetGrabableComponent(UPrimitiveComponent*& GrabableComponent);
};
