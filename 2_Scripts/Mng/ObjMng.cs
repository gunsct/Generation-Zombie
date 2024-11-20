using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjMng : MonoBehaviour
{
	[JsonIgnore] public MainMng MAIN { get { return MainMng.Instance; } }
	[JsonIgnore] public Utile_Class UTILE { get { return MainMng.IsValid() ? MAIN.m_Utile : null; } }
	[JsonIgnore] public UIMng POPUP { get { return MAIN.m_UIMng; } }


	[JsonIgnore] public AppInfo APPINFO { get { return MAIN.m_AppInfo; } }
	[JsonIgnore] public LS_Web WEB { get { return MAIN.m_Web; } }
	[JsonIgnore] public AccountMng ACC { get { return MAIN.m_AccountMng; } }
	[JsonIgnore] public ToolData TDATA { get { return MAIN.m_ToolData; } }
	[JsonIgnore] public UserInfo USERINFO { get { return MAIN.m_UserInfo; } }
	[JsonIgnore] public TutoCheck TUTO { get { return USERINFO.m_Tuto; } }
	[JsonIgnore] public DelegateInfo DLGTINFO { get { return MAIN.m_DelegateInfo; } }
	[JsonIgnore] public StageInfo STAGEINFO { get { return MAIN.m_StageInfo; } }
	[JsonIgnore] public StageUser STAGE_USERINFO { get { return STAGEINFO.m_User; } }
	[JsonIgnore] public BattleInfo BATTLEINFO { get { return MAIN.m_BattleInfo; } }
	[JsonIgnore] public PVPInfo PVPINFO { get { return MAIN.m_PVPInfo; } }
	// 사운드
	[JsonIgnore] public SoundMng SND { get { return SoundMng.Instance; } }


	// 타이틀에서만 사용할것
	[JsonIgnore] public TitleMng TITLE { get { return TitleMng.Instance; } }

	// 플레이에서만 사용할것
	[JsonIgnore] public PlayMng PLAY { get { return PlayMng.Instance; } }
	// 스테이지에서만 사용할것
	[JsonIgnore] public StageMng STAGE { get { return StageMng.Instance; } }
	[JsonIgnore] public BattleMng BATTLE { get { return BattleMng.Instance; } }
	[JsonIgnore] public TowerMng TOWER { get { return TowerMng.Instance; } }
	[JsonIgnore] public PVPMng PVP { get { return PVPMng.Instance; } }

	//하이브
	[JsonIgnore] public HiveMng HIVE { get { return MAIN.m_Hive; } }
	//광고
	[JsonIgnore] public AdsMng ADS { get { return MAIN.m_ADS; } }
	//인앱결제
	[JsonIgnore] public IAPMng IAP { get { return MAIN.m_IAP; } }


	public void PlayBGSound(SND_IDX Idx)
	{
		if (!MainMng.IsValid()) return;
		SND.PlayBgSound(Idx);
	}
	public void PlayFXSnd(int _idx)
	{
		if (!MainMng.IsValid()) return;
		SND.PlayEffSound((SND_IDX)_idx);
	}
	public AudioSource PlayEffSound(SND_IDX Idx, float _volume = 1f)
	{
		if (!MainMng.IsValid()) return null;
		return SND.DirectPlayEffSound(Idx, false, _volume);
	}
	public void DelayPlayFXSND(float _delay, SND_IDX _idx, float _vol = 1f)
	{
		if (!MainMng.IsValid()) return;
		SND.DelayPlayEffSound(_delay, _idx, false, _vol);
	}
	public void PlayMainZombieSnd(int _idx)
	{
		if (!MainMng.IsValid()) return;
		int diff = USERINFO.GetDifficulty();
		int idx = USERINFO.m_Stage[StageContentType.Stage].Idxs[diff].Idx;
		TStageTable tdata = TDATA.GetStageTable(idx, diff);
		if (tdata.m_InZombie) SND.PlayEffSound((SND_IDX)_idx);
	}
	public void PlayStageBGSound()
	{
		if (!MainMng.IsValid()) return;
		SND.PlayStageBG();
	}
	public void PlayAddStatSnd(StatType type)
	{
		if (!MainMng.IsValid()) return;
		switch (type)
		{
		case StatType.Men:
			// 이펙트
			PlayEffSound(SND_IDX.SFX_0451);
			break;
		case StatType.Hyg:
			// 이펙트
			PlayEffSound(SND_IDX.SFX_0453);
			break;
		case StatType.Sat:
			// 이펙트
			PlayEffSound(SND_IDX.SFX_0452);
			break;
		case StatType.HP:
			// 이펙트
			PlayEffSound(SND_IDX.SFX_0450);
			break;
		}
	}
	public void PlayVoiceSnd(List<SND_IDX> _idx)
	{
		if (!MainMng.IsValid()) return;
		SND.Play_VoiceSnd(_idx);
	}
	public void PlayCommVoiceSnd(VoiceType _type)
	{
		if (!MainMng.IsValid()) return;
		SND.Play_CommVoiceSnd(_type);
	}
	public void AllStopSound()
	{
		if (!MainMng.IsValid()) return;
		SND.AllStop();
	}

	public void LoadingStopSound()
	{
		if (!MainMng.IsValid()) return;
		SND.LoadingStopSND();
	}
	
}
