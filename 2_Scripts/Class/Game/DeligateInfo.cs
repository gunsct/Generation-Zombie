using Newtonsoft.Json;
using System;

public class DelegateInfo : ClassMng
{
#region Refresh No param
	public delegate void RefreshNormal();
	[JsonIgnore] public RefreshNormal f_RFMainMenuAlarm { get; set; }
	[JsonIgnore] public RefreshNormal f_RFPDABtnUI { get; set; }
	[JsonIgnore] public RefreshNormal f_RFMissionAlarm { get; set; }
	//오브제에 붙은 SFX_Animator에서 루프 사운드 제어
	[JsonIgnore] public RefreshNormal f_OBJSndOff { get; set; }
	[JsonIgnore] public RefreshNormal f_OBJSndOn { get; set; }
	// 하루 지남 알림 받기
	[JsonIgnore] public RefreshNormal f_DayChange { get; set; }
	//캐릭터 관리창 카드들 갱신
	[JsonIgnore] public RefreshNormal f_RFCharInfoCard { get; set; }
	//캐릭터 관리창 카드들 갱신
	[JsonIgnore] public RefreshNormal f_RFDeckCharInfoCard { get; set; }
	[JsonIgnore] public RefreshNormal f_RFGuidQuest { get; set; } 
	[JsonIgnore] public RefreshNormal f_RFVIPBtn { get; set; }
#endregion

	
#region Refresh bool param
	public delegate void RefreshNormal_bool(bool value1);
	[JsonIgnore] public RefreshNormal_bool f_RfHPLowUI { get; set; }
	[JsonIgnore] public RefreshNormal_bool f_HDClockUI { get; set; }
	[JsonIgnore] public RefreshNormal_bool f_RFInvenAlarm { get; set; }
	#endregion

	#region Refresh int param
	public delegate void RefreshNormal_int(int _val);
	[JsonIgnore] public RefreshNormal_int f_RFHubInfoUI { get; set; }
	#endregion


	#region Refresh float param
	public delegate void RefreshNormal_float(float value1);
	[JsonIgnore] public RefreshNormal_float f_VideoVolume { get; set; }
#endregion

	
#region Refresh long param
	public delegate void RefreshNormal_long(long value1);
	[JsonIgnore] public RefreshNormal_long f_RFShellUI { get; set; }
#endregion

#region Refresh enum param
	public delegate void RefreshNormal_enum<T>(T mode);
	[JsonIgnore] public RefreshNormal_enum<GuidQuestInfo.InfoType> f_RFGuidQuestUI { get; set; }
#endregion
	
#region Refresh bool, bool param
	public delegate void RefreshNormal_bool_bool(bool value1, bool value2);
	[JsonIgnore] public RefreshNormal_bool_bool f_RFInvenUI { get; set; }
#endregion
	
#region Refresh int, int param
	public delegate void RefreshNormal_int_int(int value1, int value2);
	[JsonIgnore] public RefreshNormal_int_int f_RFClockChangeUI { get; set; }
#endregion


#region Refresh long, long param
	public delegate void RefreshNormal_long_long(long value1, long value2);
	[JsonIgnore] public RefreshNormal_long_long f_RFMoneyUI { get; set; }
	[JsonIgnore] public RefreshNormal_long_long f_RFCashUI { get; set; }
	[JsonIgnore] public RefreshNormal_long_long f_RFExpUI { get; set; }
	[JsonIgnore] public RefreshNormal_long_long f_RFCoinUI { get; set; }
	[JsonIgnore] public RefreshNormal_long_long f_RFGCoinUI { get; set; }
	[JsonIgnore] public RefreshNormal_long_long f_RFTicketUI { get; set; }
	[JsonIgnore] public RefreshNormal_long_long f_RFCharTicketUI { get; set; }
	[JsonIgnore] public RefreshNormal_long_long f_RFItemTicketUI { get; set; }
	[JsonIgnore] public RefreshNormal_long_long f_RFMileageUI { get; set; }
	[JsonIgnore] public RefreshNormal_long_long f_RFPVPJunkUI { get; set; }
	[JsonIgnore] public RefreshNormal_long_long f_RFPVPCultivateUI { get; set; }
	[JsonIgnore] public RefreshNormal_long_long f_RFPVPChemicalUI { get; set; }
	#endregion

	#region Refresh etc param
	public delegate void RefreshStatUI(StatType _type, float _crntval, float _preval, float _maxval);
	[JsonIgnore] public RefreshStatUI f_RfStatUI { get; set; }

	public delegate void RefreshHPUI(float _crntval, float _preval, float _maxval, Action _cb = null);
	[JsonIgnore] public RefreshHPUI f_RfHPUI { get; set; }

	public delegate void RefreshAPUI(float _crntval, float _preval, float _maxval);
	[JsonIgnore] public RefreshAPUI f_RfAPUI { get; set; }
	public delegate void RefreshClockUI(int _day, int _time, float _delay = 0f);
	[JsonIgnore] public RefreshClockUI f_RFClockUI { get; set; }

	public delegate void RefreshModeTimer(double _time, bool _continue = false);
	[JsonIgnore] public RefreshModeTimer f_RFModeTimer { get; set; }
#endregion

}