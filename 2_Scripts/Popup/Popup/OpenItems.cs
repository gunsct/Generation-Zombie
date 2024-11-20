using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public enum OpenItemType
{
	Character,
	Item,
	Mileage
}
public class OpenItem:ClassMng
{
	public int result_code;
	public OpenItemType m_Type;
	public int m_Idx;
	public int m_Cnt;
	public int[] m_Grade = new int[2];
	public CharInfo m_CharInfo { get { return USERINFO.m_Chars.Find(t => t.m_Idx == m_Idx); } }
}

public class OpenItems : PopupBase
{
  
#pragma warning disable 0649
	[SerializeField] Animator m_Ani;
	[SerializeField] RectTransform m_Bag;
	[SerializeField] GameObject m_TargetTexture;
	[SerializeField] RenderTexture m_RT;
	[SerializeField] GameObject m_ItemPrefab;
	GameObject m_ItemPanel;
	List<OpenItem> m_Items = new List<OpenItem>();

	/// <summary> 좌표 </summary>
	Vector3[] REWARD_POS = {
		new Vector3(-0.11f, -2.05f, 0f),
		new Vector3(-1.5f, -0.32f, 0f),
		new Vector3(1.84f, -1.02f, 0f),
		new Vector3(-1.76f, 2.26f, 0f),
		new Vector3(1.68f, 1.6f, 0f),
		new Vector3(0.25f, 2.59f, 0f),
		Vector3.zero };
#pragma warning restore 0649

	private void OnDestroy() {
		if (m_ItemPanel != null) {
			GameObject.Destroy(m_ItemPanel);
			m_ItemPanel = null;
		}
	}

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		m_TargetTexture.SetActive(!MAIN.IS_State(MainState.BATTLE));
		m_Items = (List<OpenItem>)aobjValue[0];

		if (m_ItemPanel == null) m_ItemPanel = new GameObject("RewardItemPanel");
		Camera cam = new GameObject("RTCAM").AddComponent<Camera>();
		cam.transform.parent = m_ItemPanel.transform;
		cam.transform.localPosition = new Vector3(0f, 0f, -10f);
		cam.cullingMask = 1 << LayerMask.NameToLayer("RenderTexture");
		cam.targetTexture = m_RT;

		StartCoroutine(Action());
	}

	void GetAni() {
		m_Ani.SetTrigger("Get");
	}
	void GetSound() {
		PlayEffSound(SND_IDX.SFX_0003);
	}

	IEnumerator Action() {
		m_Ani.SetTrigger("Start");

		int remaining = m_Items.Count;
		int startpos = 0;
		while (startpos < remaining) {
			// 아이템 생성
			int cnt = Mathf.Min(6, remaining - startpos);
			yield return ItemAction(startpos, cnt);
			startpos += cnt;
		}

		m_Ani.SetTrigger("End");

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_Ani));
		Close();
	}

	IEnumerator ItemAction(int Offset, int Cnt) {
		Dictionary<int, Item_Open_Card> group = new Dictionary<int, Item_Open_Card>();
		List<int> posidx;
		Vector3 BaseScale = Vector3.one * 0.5f;
		if (Cnt == 1) posidx = new List<int> { 6 };
		else {
			// 랜덤위치에 생성하게하기
			posidx = new List<int> { 0, 1, 2, 3, 4, 5 };
			BaseScale = Vector3.one * 0.35f;
			Utile_Class.Shuffle<int>(posidx);
		}

		float CreateTime = 1.5f;

		for (int i = Cnt - 1; i > -1; i--, Offset++) {
			int createPos = posidx[i];
			Item_Open_Card card = Utile_Class.Instantiate(m_ItemPrefab, m_ItemPanel.transform).GetComponent<Item_Open_Card>();
			card.GetComponent<SortingGroup>().sortingOrder = 7 - createPos;
			card.transform.position = REWARD_POS[createPos];
			group.Add(createPos, card);
			card.SetStartScale(BaseScale);
			card.SetData(m_Items[Offset], CreateTime, Offset);
		}

		m_ItemPanel.transform.position = Vector3.zero;
		m_ItemPanel.transform.localScale = Vector3.zero;
		iTween.ScaleTo(m_ItemPanel, iTween.Hash("scale", Vector3.one, "time", 1.5f, "easetype", "easeOutQuart"));
		yield return new WaitForSeconds(CreateTime + 3f);


		// 가까운 순서대로 하나씩 이동해준다.
		List<int> keys = new List<int>(group.Keys);
		keys.Sort((b, a) => b.CompareTo(a));
		Vector3 BagPos = Utile_Class.GetWorldPosition(m_Bag.position);
		float MoveTime = 0.5f;
		for (int i = 0; i < Cnt; i++) {
			group[keys[i]].MoveBag(BagPos, MoveTime);
			Invoke("GetSound", MoveTime - 0.2f);
			Invoke("GetAni", MoveTime); 
			 yield return new WaitForSeconds(0.3f);
		}

		yield return new WaitForSeconds(MoveTime);
	}
}
