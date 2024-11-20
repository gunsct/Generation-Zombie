using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class StageMng : ObjMng
{
	[ContextMenu("제작재료 999 개 넣기")]
	void MakingTest_AddMaterial()
	{
		for (StageMaterialType i = StageMaterialType.Bullet; i < StageMaterialType.None; i++) AddMaterial(i, 999);
	}
	[ContextMenu("시너지 테스트(군인)")]
	void SynergyTest_Soldier()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Soldier);
	}
	[ContextMenu("시너지 테스트(경찰)")]
	void SynergyTest_Police()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Police);
	}
	[ContextMenu("시너지 테스트(스파이)")]
	void SynergyTest_Spy()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Spy);
	}
	[ContextMenu("시너지 테스트(탐정)")]
	void SynergyTest_Detective()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Detective);
	}
	[ContextMenu("시너지 테스트(탐험가)")]
	void SynergyTest_Explorer()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Explorer);
	}
	[ContextMenu("시너지 테스트(운동선수)")]
	void SynergyTest_Athlete()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Athlete);
	}
	[ContextMenu("시너지 테스트(경비)")]
	void SynergyTest_Guard()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Guard);
	}
	[ContextMenu("시너지 테스트(소매치기)")]
	void SynergyTest_Pickpocket()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Pickpocket);
	}
	[ContextMenu("시너지 테스트(마피아)")]
	void SynergyTest_Mafia()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Mafia);
	}
	[ContextMenu("시너지 테스트(암살자)")]
	void SynergyTest_Assassin()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Assassin);
	}
	[ContextMenu("시너지 테스트(소방수)")]
	void SynergyTest_Firefighter()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Firefighter);
	}
	[ContextMenu("시너지 테스트(사냥꾼)")]
	void SynergyTest_Hunter()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Hunter);
	}
	[ContextMenu("시너지 테스트(과학자)")]
	void SynergyTest_Scientist()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Scientist);
	}
	[ContextMenu("시너지 테스트(엔지니어)")]
	void SynergyTest_Engineer()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Engineer);
	}
	[ContextMenu("시너지 테스트(연구원)")]
	void SynergyTest_Researcher()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Researcher);
	}
	[ContextMenu("시너지 테스트(선생님)")]
	void SynergyTest_Teacher()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Teacher);
	}
	[ContextMenu("시너지 테스트(의사)")]
	void SynergyTest_Doctor()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Doctor);
	}
	[ContextMenu("시너지 테스트(간호사)")]
	void SynergyTest_Nurse()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Nurse);
	}
	[ContextMenu("시너지 테스트(약사)")]
	void SynergyTest_Pharmacist()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Pharmacist);
	}
	[ContextMenu("시너지 테스트(장의사)")]
	void SynergyTest_Undertaker()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Undertaker);
	}
	[ContextMenu("시너지 테스트(변호사)")]
	void SynergyTest_Lawyer()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Lawyer);
	}
	[ContextMenu("시너지 테스트(작사)")]
	void SynergyTest_Author()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Author);
	}
	[ContextMenu("시너지 테스트(예술가)")]
	void SynergyTest_Artist()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Artist);
	}
	[ContextMenu("시너지 테스트(사업가)")]
	void SynergyTest_Businessman()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Businessman);
	}
	[ContextMenu("시너지 테스트(요리사)")]
	void SynergyTest_Shef()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Shef);
	}
	[ContextMenu("시너지 테스트(신부)")]
	void SynergyTest_Father()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Father);
	}
	[ContextMenu("시너지 테스트(정치인)")]
	void SynergyTest_Politician()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Politician);
	}
	[ContextMenu("시너지 테스트(백수)")]
	void SynergyTest_Freetimer()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Freetimer);
	}
	[ContextMenu("시너지 테스트(학생)")]
	void SynergyTest_Student()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Student);
	}
	[ContextMenu("시너지 테스트(자원봉사자)")]
	void SynergyTest_Volunteer()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Volunteer);
	}
	[ContextMenu("시너지 테스트(카운셀러)")]
	void SynergyTest_Counselor()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Counselor);
	}
	[ContextMenu("시너지 테스트(미치광이)")]
	void SynergyTest_Lunatic()
	{
		SetBuff(EStageBuffKind.Synergy, (int)JobType.Lunatic);
	}
}
