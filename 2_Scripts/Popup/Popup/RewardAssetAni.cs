using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

[System.Serializable] public class DicItem_RewardAssetAni_Eff : SerializableDictionary<Res_RewardType, GameObject> { }
public class RewardAssetAni : PopupBase
{

	[System.Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Item_TopAsset TopAsset;
		public DicItem_RewardAssetAni_Eff Effs;
	}
	[SerializeField] SUI m_SUI;

	Dictionary<Res_RewardType, RectTransform> Rewards;

	bool IsStart;
	bool Is_ViewTop;
	private IEnumerator Start()
	{
		IsStart = false;
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));

		IsStart = true;
	}

	bool IsActiveAsset(Item_TopAsset.TopAsset asset)
	{
		switch(asset)
		{
		case Item_TopAsset.TopAsset.Money: return Rewards.ContainsKey(Res_RewardType.Money);
		case Item_TopAsset.TopAsset.GoldTeeth: return Rewards.ContainsKey(Res_RewardType.Cash);
		case Item_TopAsset.TopAsset.CharExp: return Rewards.ContainsKey(Res_RewardType.Exp);
		}
		return false;
	}

	/// <summary> aobjValue 0 : 보상 위치 정보, 1 : 지급전 금니량, 2 : 지급전 달러량, 3 : 지급전 캐릭터 경험치량 </summary>
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		IsStart = false;
		Is_ViewTop = true;// POPUP.GetMainUI().m_Popup != PopupName.Play;

		Rewards = (Dictionary<Res_RewardType, RectTransform>)aobjValue[0];

		for (Item_TopAsset.TopAsset i = Item_TopAsset.TopAsset.Money; i < Item_TopAsset.TopAsset.End; i++) m_SUI.TopAsset.ActiveAsset(i, IsActiveAsset(i));

		m_SUI.TopAsset.gameObject.SetActive(Is_ViewTop);
		if (Is_ViewTop) {
			m_SUI.TopAsset.Active_AutoCheck(false);
			m_SUI.TopAsset.SetData(Item_TopAsset.TopAsset.Money, USERINFO.m_BMoney, USERINFO.m_BMoney);
			m_SUI.TopAsset.SetData(Item_TopAsset.TopAsset.GoldTeeth, USERINFO.m_BCash, USERINFO.m_BCash);
			m_SUI.TopAsset.SetData(Item_TopAsset.TopAsset.CharExp, USERINFO.m_BExp, USERINFO.m_BExp);
		}

		base.SetData(pos, popup, cb, aobjValue);
	}

	public void StartAction(Res_RewardType type, bool _anim = true)
	{
		if (!Rewards.ContainsKey(type)) return;
		if(m_SUI.Effs.ContainsKey(type)) m_SUI.Effs[type].transform.position = Rewards[type].position;
		switch (type)
		{
		case Res_RewardType.Cash:
			if(_anim) m_SUI.Anim.SetTrigger("Start_GoldTeeth");
			if (Is_ViewTop) m_SUI.TopAsset.SetData(Item_TopAsset.TopAsset.GoldTeeth, USERINFO.m_Cash, USERINFO.m_BCash);
			break;
		case Res_RewardType.Money:
				if (_anim) m_SUI.Anim.SetTrigger("Start_Money");
			if (Is_ViewTop) m_SUI.TopAsset.SetData(Item_TopAsset.TopAsset.Money, USERINFO.m_Money, USERINFO.m_BMoney);
			break;
		case Res_RewardType.Exp:
				if (_anim) m_SUI.Anim.SetTrigger("Start_Exp");
			if (Is_ViewTop) m_SUI.TopAsset.SetData(Item_TopAsset.TopAsset.CharExp, USERINFO.m_Exp[(int)EXPType.Ingame], USERINFO.m_BExp);
			break;
		}
	}

	public void Dealay_Close(float delaytime = 0)
	{
		StartCoroutine(DelayCheck(delaytime, () => {
			if (!IsStart) IsStart = true;
			Close(0);
		}));
	}

	IEnumerator DelayCheck(float delay, Action CB)
	{
		yield return new WaitWhile(() => !IsStart);
		yield return new WaitForSeconds(delay);
		CB?.Invoke();
	}
	public void SetOffTopAsset() {
		m_SUI.TopAsset.gameObject.SetActive(false);
	}
	public override void Close(int Result = 0)
	{
		if (!IsStart) return;
		base.Close(Result);
	}
}
