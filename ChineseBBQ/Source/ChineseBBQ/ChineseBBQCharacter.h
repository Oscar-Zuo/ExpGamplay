// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Character.h"
#include "InputActionValue.h"
#include "ChineseBBQCharacter.generated.h"

class UInputComponent;
class USkeletalMeshComponent;
class USceneComponent;
class UCameraComponent;
class UAnimMontage;
class USoundBase;
class UPhysicsHandleComponent;
class ABaseInteractor;

UCLASS(config=Game)
class AChineseBBQCharacter : public ACharacter
{
	GENERATED_BODY()

	///** Pawn mesh: 1st person view (arms; seen only by self) */
	//UPROPERTY(VisibleDefaultsOnly, Category=Mesh)
	//USkeletalMeshComponent* Mesh1P;

	/** First person camera */
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = Camera, meta = (AllowPrivateAccess = "true"))
	UCameraComponent* FirstPersonCameraComponent;

	/** MappingContext */
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category=Input, meta=(AllowPrivateAccess = "true"))
	class UInputMappingContext* DefaultMappingContext;

	/** Jump Input Action */
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category=Input, meta=(AllowPrivateAccess = "true"))
	class UInputAction* JumpAction;

	/** Move Input Action */
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category=Input, meta=(AllowPrivateAccess = "true"))
	class UInputAction* MoveAction;

	/** Grab Input Action */
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Input, meta = (AllowPrivateAccess = "true"))
		class UInputAction* GrabAction;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Physics, meta = (AllowPrivateAccess = "true"))
	class UPhysicsHandleComponent* PhysicsHandle;
	
public:
	AChineseBBQCharacter();

protected:
	virtual void BeginPlay();

	virtual void Tick(float DeltaTime) override;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Status)
		float grabberLength;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Status)
		float holdDistance;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Status)
		TObjectPtr<ABaseInteractor> GrabObject;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Status)
		bool isGrabbing;

public:
	/** Look Input Action */
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Input, meta = (AllowPrivateAccess = "true"))
	class UInputAction* LookAction;

	///** Bool for AnimBP to switch to another animation set */
	//UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = Weapon)
	//bool bHasRifle;

	///** Setter to set the bool */
	//UFUNCTION(BlueprintCallable, Category = Weapon)
	//void SetHasRifle(bool bNewHasRifle);

	///** Getter for the bool */
	//UFUNCTION(BlueprintCallable, Category = Weapon)
	//bool GetHasRifle();

protected:
	/** Called for movement input */
	void Move(const FInputActionValue& Value);

	/** Called for looking input */
	void Look(const FInputActionValue& Value);

	// Grab things in front of player
	void Grab(const FInputActionValue& Value);
	void Release(const FInputActionValue& Value);
protected:
	// APawn interface
	virtual void SetupPlayerInputComponent(UInputComponent* InputComponent) override;
	// End of APawn interface

public:
	///** Returns Mesh1P subobject **/
	//USkeletalMeshComponent* GetMesh1P() const { return Mesh1P; }
	/** Returns FirstPersonCameraComponent subobject **/
	UCameraComponent* GetFirstPersonCameraComponent() const { return FirstPersonCameraComponent; }
};

