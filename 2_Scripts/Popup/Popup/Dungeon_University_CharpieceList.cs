using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;

public class Dungeon_University_CharpieceList : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI Desc;
		public TextMeshProUGUI Msg;//734 난이도 153 무작위
		public Transform Element;//Item_University_PieceList_Element
		public Transform Bucket;
	}
	[SerializeField] SUI m_SUI;
	private IEnumerator m_Action;
	TStageTable m_StgTData;
	int m_Lv;
	List<RES_REWARD_BASE> m_Rewards = new List<RES_REWARD_BASE>();

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		DLGTINFO?.f_RFMoneyUI?.Invoke(USERINFO.m_Money, USERINFO.m_Money);
		DLGTINFO?.f_RFCashUI?.Invoke(USERINFO.m_Cash, USERINFO.m_Cash);

		m_StgTData = (TStageTable)aobjValue[0];
		m_Lv = (int)aobjValue[1];
		m_Rewards = (List<RES_REWARD_BASE>)aobjValue[2];

		UTILE.Load_Prefab_List(m_Rewards.Count, m_SUI.Bucket, m_SUI.Element);
		for (int i = 0; i < m_SUI.Bucket.childCount; i++)
			m_SUI.Bucket.GetChild(i).GetComponent<Item_University_PieceList_Element>().SetData(m_Rewards[i], null, false);
		int cnt = 0;
		for (int i = 0; i < m_Rewards.Count; i++)
			cnt += ((RES_REWARD_ITEM)m_Rewards[i]).Cnt;
		m_SUI.Desc.text = string.Format("{0} - {1} {2}", m_StgTData.GetName(), TDATA.GetString(734), m_Lv + 1);
		m_SUI.Msg.text = string.Format(TDATA.GetString(153), cnt);

	}
	public override void Close(int Result = 0) {
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int Result) {
		m_SUI.Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(Result);
	}
}
