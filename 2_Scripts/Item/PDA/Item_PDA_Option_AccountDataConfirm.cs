using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class Item_PDA_Option_AccountDataConfirm : Item_PDA_Base
{
	[Serializable]
	public struct SInfoUI
	{
		public Image Profile;
		public Text Name;
		public TextMeshProUGUI LV;
		public TextMeshProUGUI Cash;
	}
	[Serializable]
	public struct SUI
	{
		public Animator Anim;

		public SInfoUI Info;

		public TextMeshProUGUI Notice;
		public TextMeshProUGUI Label;
		public TextMeshProUGUI Btn;
		public GameObject NewMark;
	}
	[SerializeField]
	SUI m_SUI;
	bool IsAction = false;
	Item_PDA_Option_AccountData.SelectPos Pos;
	RES_ACC_INFO[] Infos;
	ACC_STATE LoginType;
	private void OnEnable() {
		StartCoroutine(AnimEnd());
	}
	public override void SetData(Action<object, object[]> CloaseCB, object[] args)
	{
		Pos = (Item_PDA_Option_AccountData.SelectPos)args[0];
		Infos = (RES_ACC_INFO[])args[1];
		LoginType = (ACC_STATE)args[2];
		base.SetData(CloaseCB, args);
		bool IsNew = Pos == Item_PDA_Option_AccountData.SelectPos.Now;
		m_SUI.NewMark.SetActive(IsNew);
		m_SUI.Notice.text = TDATA.GetString(!IsNew ? 864 : 863);
		m_SUI.Label.text = TDATA.GetString(!IsNew ? 857 : 856);
		m_SUI.Btn.text = TDATA.GetString(!IsNew ? 866 : 865);

		var data = Infos[(int)Pos];
		m_SUI.Info.Profile.sprite = TDATA.GetUserProfileImage(data.Profile);
		m_SUI.Info.Name.text = data.m_Name;
		m_SUI.Info.LV.text = data.LV.ToString();
		m_SUI.Info.Cash.text = Utile_Class.CommaValue(data.m_Cash);
	}

	IEnumerator AnimEnd()
	{
		IsAction = true;
		m_SUI.Anim.SetTrigger("Start");

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		IsAction = false;
	}

	public void OnYES()
	{
		if (IsAction) return;
		var selectinfo = Infos[(int)Pos].HivePlayerInfo;
		POPUP.SetConnecting(true, UIMng.ConnectingTrigger.Now);
		HIVE.SelectAccConnect(selectinfo.playerId, (result, isReset) => {
			POPUP.SetConnecting(false);
			if (result.isSuccess())
			{
				// hive쪽 변경만 하면 되므로 서버 프로토콜 변경 필요없음
				switch (Pos)
				{
				case Item_PDA_Option_AccountData.SelectPos.Befor:
					// 이전 idp계정 선택 
					MAIN.ReStart();
					break;
				case Item_PDA_Option_AccountData.SelectPos.Now:
					// 게스트 계정으로 idp연결
					OnClose();
					//IFAccount ifacc = ACC.GetIFAcc(LoginType);
					//string ID = ifacc.GetID();
					//USERINFO.ACC_CHANGE(LoginType, ID, () => {
					//	OnClose();
					//});
					break;
				}
			}
			else POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, result.message);
		});
	}
}
