using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class Store_GatchaMileage : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI Timer;
		public GameObject[] TimeGroup;
		public GameObject Element;		//Item_Store_GatchaMileage
		public Transform Bucket;
		public ScrollRect Scroll;
	}
	[SerializeField] SUI m_SUI;
	Dictionary<int, int> m_ShopInfos = new Dictionary<int, int>();
	IEnumerator m_Action;

	private void Update() {
		m_SUI.TimeGroup[0].SetActive(USERINFO.m_ShopInfo.NowSeason != null);
		m_SUI.TimeGroup[1].SetActive(USERINFO.m_ShopInfo.NowSeason == null);
		m_SUI.Timer.text = string.Format(TDATA.GetString(4005), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.single, USERINFO.m_ShopInfo.NowSeason == null ? 0d : (USERINFO.m_ShopInfo.NowSeason.ETime - UTILE.Get_ServerTime_Milli()) * 0.001d));
		if (m_ShopInfos.Count < 1) SetUI();
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		DLGTINFO?.f_RFMileageUI?.Invoke(USERINFO.m_Mileage, USERINFO.m_Mileage);
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		m_ShopInfos = USERINFO.GetMileageStore();
		if (m_ShopInfos.Count < 1) {
			UTILE.Load_Prefab_List(0, m_SUI.Bucket, m_SUI.Element.transform);
			return;
		}
		List<TShopTable> tdatas = m_ShopInfos.Select(o => TDATA.GetShopTable(o.Value)).ToList();
		tdatas.Sort((TShopTable _before, TShopTable _after) => {
			//if (_before.m_LimitCnt != _after.m_LimitCnt && !(_before.m_LimitCnt > 0 && _after.m_LimitCnt > 0)) return _after.m_LimitCnt.CompareTo(_before.m_LimitCnt);
			return _before.m_Level.CompareTo(_after.m_Level);
		});

		m_SUI.Scroll.enabled = tdatas.Count > 9;

		UTILE.Load_Prefab_List(tdatas.Count, m_SUI.Bucket, m_SUI.Element.transform);
		for(int i = 0;i< tdatas.Count; i++) {
			Item_Store_GatchaMileage element = m_SUI.Bucket.GetChild(i).GetComponent<Item_Store_GatchaMileage>();
			element.SetData(tdatas[i], Click_Buy, SetUI);
		}

		base.SetUI();
	}
	void Click_Buy(int _sidx, List<int> _pickup = null) {
		USERINFO.ITEM_BUY(_sidx, 1, (res) => {
			if (res.IsSuccess()) {
				MAIN.SetRewardList(new object[] { res.GetRewards() }, () => { 
					SetUI();
				});
			}
		}, true, null, null, true, _pickup);
	}

	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("Close");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(_result);
	}
}
