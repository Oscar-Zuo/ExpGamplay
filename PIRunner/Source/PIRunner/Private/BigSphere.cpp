// Fill out your copyright notice in the Description page of Project Settings.

#include "BigSphere.h"
#include "NumberCharacter.h"
#include "Math/UnrealMathUtility.h"
#include "Kismet/KismetSystemLibrary.h"
#include "Kismet/KismetMathLibrary.h"
#include "Kismet/GameplayStatics.h"
#include "Components/SphereComponent.h"

// Sets default values
ABigSphere::ABigSphere()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;
	sphereRadius = 4000.0f;
	generateCharactersRadius = 1000.0f;
	generateCharactersNumber = 10;

	static ConstructorHelpers::FObjectFinder<UStaticMesh>MeshAsset(TEXT("/Game/StarterContent/Props/MaterialSphere"));
	UStaticMesh* Asset = MeshAsset.Object;
	sphere = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Sphere"));
	sphere->SetStaticMesh(Asset);
	sphere->SetRelativeScale3D(FVector(sphereRadius/50, sphereRadius / 50, sphereRadius / 50));
	SetRootComponent(sphere);

	sphereCollider = CreateDefaultSubobject <USphereComponent>(TEXT("SphereCollider"));
	sphereCollider->InitSphereRadius(sphereRadius / 50);
	sphereCollider->SetupAttachment(sphere);
}

// Called when the game starts or when spawned
void ABigSphere::BeginPlay()
{
	Super::BeginPlay();
}

// Called every frame
void ABigSphere::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
}

void ABigSphere::GenerateCharacters(FVector centerLocation, FVector playerForwardVector, int number)
{
	GenerateCharcatersInSphereArea(centerLocation, number);
	playerForwardVector.Normalize();
	//Generate Characters in front of Player
	if (!playerForwardVector.IsZero())
		GenerateCharcatersInSphereArea(centerLocation + playerForwardVector * generateCharactersRadius, number);
}

bool ABigSphere::SpawnCharacter(FVector centerLocation, TSubclassOf<ANumberCharacter> targetCharacterType)
{
	if (!targetCharacterType)
		return false;
	int counter = 0;
	FVector spawnPosition, normalVector;
	bool result;
	do
	{
		FHitResult hitResult;
		FVector sphereCenter = GetActorLocation();
		FVector coneCenterVector = centerLocation - sphereCenter;
		float halfRadian = FMath::Acos(1 - generateCharactersRadius * generateCharactersRadius / sphereRadius / sphereRadius / 2);
		normalVector = FMath::VRandCone(coneCenterVector, halfRadian);
		spawnPosition = normalVector * sphereRadius + sphereCenter;
		TArray < AActor* > actorToIgnore{ this };
		result = UKismetSystemLibrary::BoxTraceSingle(GetWorld(), spawnPosition, spawnPosition + normalVector * 50, FVector(200, 200, 200), UKismetMathLibrary::MakeRotFromZ(normalVector), TraceTypeQuery1, false, actorToIgnore, EDrawDebugTrace::None, hitResult, true);
		/*if (GEngine&& hitResult.GetActor())
			GEngine->AddOnScreenDebugMessage(-1, 15.0f, FColor::Yellow, hitResult.GetActor()->GetName());*/
	} while (result && ++counter<=20);
	if (counter > 20)
		return false;
	auto rotator = UKismetMathLibrary::MakeRotFromZ(normalVector);
	FActorSpawnParameters spawnInfo;
	spawnInfo.SpawnCollisionHandlingOverride = ESpawnActorCollisionHandlingMethod::AdjustIfPossibleButAlwaysSpawn;
	TObjectPtr<AActor> actor = GetWorld()->SpawnActor<AActor>(targetCharacterType, spawnPosition, rotator, spawnInfo);
	return true;
}

void ABigSphere::GenerateCharcatersInSphereArea(FVector centerLocation, int number)
{
	int currentCharacterNum = 0;

	TArray < FHitResult> hitResultList;
	FVector normalVector = centerLocation - GetActorLocation();
	normalVector.Normalize();
	TArray < AActor* > actorToIgnore{ this , UGameplayStatics::GetPlayerPawn(GetWorld(), 0) };
	UKismetSystemLibrary::SphereTraceMulti(GetWorld(), centerLocation - normalVector * 50, centerLocation + normalVector * 50, generateCharactersRadius, TraceTypeQuery1, false, actorToIgnore, EDrawDebugTrace::None, hitResultList, true);

	bool missingNumerCharacter[10];
	for (int i = 0; i < 10; i++)
		missingNumerCharacter[i] = true;
	for (auto hitResult : hitResultList)
	{
		TWeakObjectPtr<ANumberCharacter> numberCharacter = Cast<ANumberCharacter>(hitResult.GetActor());
		if (numberCharacter.IsValid() && !numberCharacter->repesentingCharacter.IsEmpty())
		{
			++currentCharacterNum;
			int num = numberCharacter->repesentingCharacter[0] - '0';
			missingNumerCharacter[num] = false;
		}
	}

	for (int i = 0; i < 10 && currentCharacterNum < number; i++)
	{
		if (missingNumerCharacter[i])
		{
			// I know it is not best approach, you have to make sure characterTypeList has 0-9 NumberCharacters
			// in order manually to make it work.
			SpawnCharacter(centerLocation, characterTypeList[i]);
			++currentCharacterNum;
		}
	}

	for (int i = 0; i < number - currentCharacterNum; i++)
	{
		if (!SpawnCharacter(centerLocation, characterTypeList[rand() % 10]))
			break;
	}
}
