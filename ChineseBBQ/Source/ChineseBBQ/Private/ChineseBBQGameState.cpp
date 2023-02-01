// Fill out your copyright notice in the Description page of Project Settings.


#include "ChineseBBQGameState.h"

AChineseBBQGameState::AChineseBBQGameState()
{
	orderInterval = 20.0f;
	maxOrderNum = 3;
	orderNum = 0;
	totalOrderNum = 0;
	reputation = 1;
	money = 0;
	matchLength = 300;
	price = 5;
	CityToSpicyMap = { {ChinaCity::Guangdong, 0},  {ChinaCity::Zhejiang, 0},
		{ChinaCity::Shandong, 1}, {ChinaCity::Beijing, 1},
		{ChinaCity::Hunan, 2}, {ChinaCity::Sichuan, 2} };

	FString file = FPaths::ProjectContentDir();
	file.Append(TEXT("Names.txt"));
	IPlatformFile& FileManager = FPlatformFileManager::Get().GetPlatformFile();
	FString FileContent;
	if (FileManager.FileExists(*file))
	{
		if (FFileHelper::LoadFileToString(FileContent, *file, FFileHelper::EHashOptions::None))
		{
			FileContent.ParseIntoArrayLines(NameList);
		}
	}
}
