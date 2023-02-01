// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "BaseInteractor.generated.h"

class AChineseBBQCharacter;

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

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Status)
		TObjectPtr<AChineseBBQCharacter> GrabbedBy;

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;
	UFUNCTION(BlueprintNativeEvent)
	void OnGrabbing(AChineseBBQCharacter* character);
	virtual void OnGrabbing_Implementation(TObjectPtr<AChineseBBQCharacter> character);
	UFUNCTION(BlueprintNativeEvent)
	void OnReleasing();
	virtual void OnReleasing_Implementation();
	UFUNCTION(BlueprintImplementableEvent)
	void GetGrabableComponent(UPrimitiveComponent*& GrabableComponent);
};
