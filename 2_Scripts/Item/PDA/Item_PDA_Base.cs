using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Item_PDA_Base : ObjMng
{
	protected Action<object, object[]> m_CloaseCB;
	RewardAssetAni m_RewardAssetAni = null;
	/// <summary> 메인화면으로 돌리기 </summary>
	public virtual void SetData(Action<object, object[]> CloaseCB, object[] args) {
		m_CloaseCB = CloaseCB;
	}

	public virtual void OnClose()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Play_PDA_Cloase, 0)) return;
		PlayEffSound(SND_IDX.SFX_0121);
		if (m_RewardAssetAni != null) {
			m_RewardAssetAni.Close();
		}
		m_CloaseCB?.Invoke(Item_PDA_Menu.State.Main, null);
	}

	public virtual bool OnBack()
	{
		OnClose();
		return true;
	}
	public void SetRewardAssetAni(Dictionary<LS_Web.Res_RewardType, RectTransform> _rewards, float _delay = 1f, bool _ani = false) {
		m_RewardAssetAni = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.RewardAssetAni, (result, obj) => { m_RewardAssetAni = null; }, _rewards).GetComponent<RewardAssetAni>();
		m_RewardAssetAni.Dealay_Close(_delay);
		for (int i = 0; i < _rewards.Count; i++) {
			m_RewardAssetAni.StartAction(_rewards.ElementAt(i).Key, _ani);
		}
	}
}
