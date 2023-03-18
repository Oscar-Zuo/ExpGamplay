// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "BigSphere.generated.h"

class USphereComponent;
class ANumberCharacter;

UCLASS()
class PIRUNNER_API ABigSphere : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	ABigSphere();
	// Called every frame
	virtual void Tick(float DeltaTime) override;

	UFUNCTION(BlueprintCallable)
	void GenerateCharacters(FVector centerLocation, FVector playerForwardVector, int number);

	UFUNCTION(BlueprintCallable)
	bool SpawnCharacter(FVector centerLocation, TSubclassOf<ANumberCharacter> targetCharacterType);
	
	void GenerateCharcatersInSphereArea(FVector centerLocation, int number);
protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float sphereRadius;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	TObjectPtr<UStaticMeshComponent> sphere;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	TObjectPtr<USphereComponent> sphereCollider;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float generateCharactersRadius;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	int generateCharactersNumber;
	
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	TArray<TSubclassOf<ANumberCharacter>> characterTypeList;
};
