// Copyright Epic Games, Inc. All Rights Reserved.

#include "ChineseBBQCharacter.h"
#include "BaseInteractor.h"
#include "Kismet/GameplayStatics.h"
#include "Animation/AnimInstance.h"
#include "Camera/CameraComponent.h"
#include "Components/CapsuleComponent.h"
#include "PhysicsEngine/PhysicsHandleComponent.h"
#include "EnhancedInputComponent.h"
#include "EnhancedInputSubsystems.h"


//////////////////////////////////////////////////////////////////////////
// AChineseBBQCharacter

AChineseBBQCharacter::AChineseBBQCharacter()
{
	//// Character doesnt have a rifle at start
	//bHasRifle = false;
	
	// Set size for collision capsule
	GetCapsuleComponent()->InitCapsuleSize(55.f, 96.0f);
		
	// Create a CameraComponent	
	FirstPersonCameraComponent = CreateDefaultSubobject<UCameraComponent>(TEXT("FirstPersonCamera"));
	FirstPersonCameraComponent->SetupAttachment(GetCapsuleComponent());
	FirstPersonCameraComponent->SetRelativeLocation(FVector(-10.f, 0.f, 60.f)); // Position the camera
	FirstPersonCameraComponent->bUsePawnControlRotation = true;

	//// Create a mesh component that will be used when being viewed from a '1st person' view (when controlling this pawn)
	//Mesh1P = CreateDefaultSubobject<USkeletalMeshComponent>(TEXT("CharacterMesh1P"));
	//Mesh1P->SetOnlyOwnerSee(true);
	//Mesh1P->SetupAttachment(FirstPersonCameraComponent);
	//Mesh1P->bCastDynamicShadow = false;
	//Mesh1P->CastShadow = false;
	////Mesh1P->SetRelativeRotation(FRotator(0.9f, -19.19f, 5.2f));
	//Mesh1P->SetRelativeLocation(FVector(-30.f, 0.f, -150.f));


	PhysicsHandle = CreateDefaultSubobject<UPhysicsHandleComponent>(TEXT("Physics Handle"));

	grabberLength = 500.0f;
	holdDistance = 100.0f;
	isGrabbing = false;
}

void AChineseBBQCharacter::BeginPlay()
{
	// Call the base class  
	Super::BeginPlay();

	//Add Input Mapping Context
	if (APlayerController* PlayerController = Cast<APlayerController>(Controller))
	{
		if (UEnhancedInputLocalPlayerSubsystem* Subsystem = ULocalPlayer::GetSubsystem<UEnhancedInputLocalPlayerSubsystem>(PlayerController->GetLocalPlayer()))
		{
			Subsystem->AddMappingContext(DefaultMappingContext, 0);
		}
	}

}

void AChineseBBQCharacter::Tick(float DeltaTime)
{
	if (isGrabbing&& PhysicsHandle->GetGrabbedComponent())
	{
		FVector targetLocation = FMath::Lerp(FirstPersonCameraComponent->GetForwardVector() * holdDistance + FirstPersonCameraComponent->GetComponentLocation(), PhysicsHandle->GetGrabbedComponent()->GetComponentLocation(), 0.5);
		PhysicsHandle->SetTargetLocation(targetLocation);
	}
}

//////////////////////////////////////////////////////////////////////////// Input

void AChineseBBQCharacter::SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent)
{
	// Set up action bindings
	if (UEnhancedInputComponent* EnhancedInputComponent = CastChecked<UEnhancedInputComponent>(PlayerInputComponent))
	{
		//Jumping
		EnhancedInputComponent->BindAction(JumpAction, ETriggerEvent::Triggered, this, &ACharacter::Jump);
		EnhancedInputComponent->BindAction(JumpAction, ETriggerEvent::Completed, this, &ACharacter::StopJumping);

		//Moving
		EnhancedInputComponent->BindAction(MoveAction, ETriggerEvent::Triggered, this, &AChineseBBQCharacter::Move);

		//Looking
		EnhancedInputComponent->BindAction(LookAction, ETriggerEvent::Triggered, this, &AChineseBBQCharacter::Look);
		//Grabbing
		EnhancedInputComponent->BindAction(GrabAction, ETriggerEvent::Triggered, this, &AChineseBBQCharacter::Grab);
		EnhancedInputComponent->BindAction(GrabAction, ETriggerEvent::Completed, this, &AChineseBBQCharacter::Release);
	}
}


void AChineseBBQCharacter::Move(const FInputActionValue& Value)
{
	// input is a Vector2D
	FVector2D MovementVector = Value.Get<FVector2D>();

	if (Controller != nullptr)
	{
		// add movement 
		AddMovementInput(GetActorForwardVector(), MovementVector.Y);
		AddMovementInput(GetActorRightVector(), MovementVector.X);
	}
}

void AChineseBBQCharacter::Look(const FInputActionValue& Value)
{
	// input is a Vector2D
	FVector2D LookAxisVector = Value.Get<FVector2D>();

	if (Controller != nullptr)
	{
		// add yaw and pitch input to controller
		AddControllerYawInput(LookAxisVector.X);
		AddControllerPitchInput(LookAxisVector.Y);
	}
}

void AChineseBBQCharacter::Grab(const FInputActionValue& Value)
{
	if (!isGrabbing)
	{
		FVector3d StartLocation = FirstPersonCameraComponent->GetComponentLocation();
		FVector3d EndLocation = FirstPersonCameraComponent->GetForwardVector() * grabberLength + StartLocation;
		FHitResult OutHit;
		FCollisionQueryParams CollisionParams;
		CollisionParams.AddIgnoredActor(this);
		//DrawDebugLine(GetWorld(), StartLocation, EndLocation, FColor::Green, false, 1, 0, 5);
		if (GetWorld()->LineTraceSingleByChannel(OutHit, StartLocation, EndLocation, ECC_Visibility, CollisionParams))
		{
			TObjectPtr<ABaseInteractor> HitActor = Cast<ABaseInteractor>(OutHit.GetActor());
			if (HitActor)
			{
				GrabObject = HitActor;
				GrabObject->Grabbed();
				UPrimitiveComponent* GrabableComponent;
				HitActor->GetGrabableComponent(GrabableComponent);
				PhysicsHandle->GrabComponentAtLocationWithRotation(GrabableComponent, OutHit.BoneName, OutHit.Component->GetComponentLocation(), FRotator::ZeroRotator);				
				isGrabbing = true;
			}
		}
	}
}

void AChineseBBQCharacter::Release(const FInputActionValue& Value)
{
	if (isGrabbing)
	{
		if (PhysicsHandle->GrabbedComponent)
		{
			PhysicsHandle->ReleaseComponent();
			GrabObject->Released();
		}
		isGrabbing = false;
	}
}

//void AChineseBBQCharacter::SetHasRifle(bool bNewHasRifle)
//{
//	bHasRifle = bNewHasRifle;
//}
//
//bool AChineseBBQCharacter::GetHasRifle()
//{
//	return bHasRifle;
//}