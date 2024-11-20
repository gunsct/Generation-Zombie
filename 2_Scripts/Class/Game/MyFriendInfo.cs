using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using static LS_Web;

public class MyFriendInfo : ClassMng
{
	/// <summary> 제거 횟수 0부터 시작 </summary>
	public int RemoveCnt;
	/// <summary> 제거한 시간(하루가 지났으면 자동으로 초기화 하면됨) </summary>
	public long RemoveTime;
	/// <summary> 선물 보낸 횟수 </summary>
	public int RecieveCnt;
	/// <summary> 선물 받은 횟수 </summary>
	public long RecieveTime;
	/// <summary> 친구 리스트 (요청 포함됨) </summary>
	public List<RES_FRIENDINFO> Friends = new List<RES_FRIENDINFO>();

	public void SetDATA(RES_FRIEND Info)
	{
		RemoveCnt = Info.RemoveCnt;
		RemoveTime = Info.RemoveTime;
		Friends = Info.Friends;
	}

	public void LoadFriendInfo(Action CB)
	{
		// 친구 정보 받아두기
		WEB.SEND_REQ_FRIEND((res) => { CB?.Invoke(); });
	}

	public void CheckRemoveCnt()
	{
		var time = (long)UTILE.Get_ServerTime_Milli();
		time = time - time % 86400000L;
		if (RemoveTime < time)
		{
			RemoveCnt = 0;
			RemoveTime = (long)UTILE.Get_ServerTime_Milli();
		}
	}

	public void Invate(RES_RECOMMEND_USER user, Action<int> action)
	{
		if (Friends.Find(o => o.UserNo == user.UserNo && o.State != Friend_State.Deleted) != null)
		{
			action?.Invoke(EResultCode.SUCCESS);
			return;
		}
		WEB.SEND_REQ_FRIEND_INVITE((res) => {
			if (res.IsSuccess()) {
				PlayEffSound(SND_IDX.SFX_0116);
				USERINFO.Check_Mission(MissionType.AddFriend, 0, 0, 1);
				// 임시 대이터 생성
				Friends.Add(new RES_FRIENDINFO()
				{
					State = Friend_State.Invited,
					CTime = (long)UTILE.Get_ServerTime_Milli(),
					UserNo = user.UserNo,
					Name = user.Name,
					Profile = user.Profile,
					LV = user.LV,
					Stage = user.Stage,
					Power = user.Power,
					UTime = user.UTime
				});
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(666));
			}
			else
			{

				switch (res.result_code)
				{
				case EResultCode.ERROR_FRIEND:
					// 이미 친구이므로 성공으로 셋팅해줌
					Friends.Add(new RES_FRIENDINFO()
					{
						State = Friend_State.Friend,
						CTime = (long)UTILE.Get_ServerTime_Milli(),
						UserNo = user.UserNo,
						Name = user.Name,
						Profile = user.Profile,
						LV = user.LV,
						Stage = user.Stage,
						Power = user.Power,
						UTime = user.UTime
					});
					break;
				case EResultCode.ERROR_FRIEND_RICEIVE_INVITE_CNT:
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(669));
					break;
				case EResultCode.ERROR_FRIEND_DELETED:
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(832));
					break;
				default:
					WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
					break;
				}
			}
			action?.Invoke(res.result_code);
		}, user.Get_RefCode());
	}

	public void Delete(RES_FRIENDINFO user, Action<int> action)
	{
		CheckRemoveCnt();
		bool IsFirend = user.State == Friend_State.Friend;
		if (IsFirend && TDATA.GetConfig_Int32(ConfigType.MaxFriendDelCount) <= RemoveCnt)
		{
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, string.Format(TDATA.GetString(672), TDATA.GetConfig_Int32(ConfigType.MaxFriendDelCount)));
			return;
		}

		PlayEffSound(IsFirend ? SND_IDX.SFX_0118 : SND_IDX.SFX_0205);
		POPUP.Set_MsgBox(PopupName.Msg_YN, "", TDATA.GetString(IsFirend ? 673 : 671), (btn, obj) =>
		{
			if ((EMsgBtn)btn == EMsgBtn.BTN_YES)
			{
				WEB.SEND_REQ_FRIEND_DELETE((res) => {
					if (!res.IsSuccess())
					{
						switch (res.result_code)
						{
						case EResultCode.ERROR_FRIEND_DEL_CNT:
							POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, string.Format(TDATA.GetString(672), TDATA.GetConfig_Int32(ConfigType.MaxFriendDelCount)));
							break;
						default:
							WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
							break;
						}
						action?.Invoke(res.result_code);
						return;
					}
					PlayEffSound(SND_IDX.SFX_0205);
					Friends.Remove(user);
					action?.Invoke(EResultCode.SUCCESS);
				}, user.UserNo);
			}
		});
	}
}


