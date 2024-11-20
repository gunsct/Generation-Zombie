using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;
using UnityEngine.UI;
using TMPro;

public class PVP_Result_League : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Image RankBG;
		public Image RankIcon;
		public TextMeshProUGUI TierName;
		public Transform Bucket;
		public GameObject UserElement;      //Item_PVP_Result_List_Element
		public GameObject[] RankUpDown;     //0:up,1:down
		public RectTransform[] ListTopDownCheck;
	}
	[SerializeField] SUI m_SUI;
	int m_RankIdx;
	List<RES_PVP_USER_BASE> m_PreUsers;
	List<RES_PVP_USER_BASE> m_NowUsers;
	RES_PVP_USER_BASE[] m_MyInfo = new RES_PVP_USER_BASE[2];    //이전 내 정보, 현재 내 정보
	int[] m_Order = new int[2];                                 //0:이전,1:현재

	List<Item_PVP_Result_List_Element> m_Elements = new List<Item_PVP_Result_List_Element>();
	Item_PVP_Result_List_Element m_MyElement;

	float[] m_Interval = new float[2] { -200f, -150f };         //0:el to el, 1:el to updown
	IEnumerator m_Action;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_RankIdx = (int)aobjValue[0];
		m_PreUsers = (List<RES_PVP_USER_BASE>)aobjValue[1];
		m_NowUsers = (List<RES_PVP_USER_BASE>)aobjValue[2];
		m_MyInfo[0] = m_PreUsers.Find(o=>o.UserNo == USERINFO.m_UID);
		m_MyInfo[1] = m_NowUsers.Find(o => o.UserNo == USERINFO.m_UID);
		m_Order[0] = m_MyInfo[0].Rank;
		m_Order[1] = m_MyInfo[1].Rank;
		base.SetData(pos, popup, cb, aobjValue);
	}

	public override void SetUI() {
		base.SetUI();
		TPvPRankTable ranktdata = TDATA.GeTPvPRankTable(m_RankIdx);
		m_SUI.RankBG.color = BaseValue.GetPVPRankColor(ranktdata.m_Rank);
		m_SUI.RankIcon.sprite = ranktdata.GetRankIcon();
		m_SUI.TierName.text = string.Format("{0} {1}", ranktdata.GetRankName(), ranktdata.GetTierName());
		m_SUI.RankUpDown[0].SetActive(ranktdata.m_UpTierType == UpTierType.LEAGUERANK);
		m_SUI.RankUpDown[1].SetActive(ranktdata.m_DownTierType == DownTierType.LEAGUERANK);

		float ypos = 0;
		for (int i = 0; i < m_NowUsers.Count; i++) {
			Item_PVP_Result_List_Element element = Utile_Class.Instantiate(m_SUI.UserElement, m_SUI.Bucket).GetComponent<Item_PVP_Result_List_Element>();
			RES_PVP_USER_BASE[] infos = new RES_PVP_USER_BASE[2] { m_NowUsers[i] , m_NowUsers[i] };//m_NowUsers.Find(o=>o.UserNo == m_PreUsers[i].UserNo)
			element.SetData(infos);
			if (m_NowUsers[i].UserNo == USERINFO.m_UID) m_MyElement = element;
			m_Elements.Add(element);

			element.transform.localPosition = new Vector3(0f, ypos, 0f);
			if (ranktdata.m_UpTierType == UpTierType.LEAGUERANK && i == ranktdata.m_UpTierVal - 1) {
				ypos += m_Interval[1];
				m_SUI.RankUpDown[0].transform.localPosition = new Vector3(0f, ypos, 0f);
			}
			else if (ranktdata.m_DownTierType == DownTierType.LEAGUERANK && i == ranktdata.m_DownTierVal - 1) {
				ypos += m_Interval[1];
				m_SUI.RankUpDown[1].transform.localPosition = new Vector3(0f, ypos, 0f);
			}
			ypos += m_Interval[0];
		}
		m_MyElement.transform.SetAsLastSibling();

		//나를 중앙으로 옮기고 꼴등이 바닥보다 위면 그 차이만큼만 올려주기
		float myposy = m_MyElement.transform.localPosition.y;
		float movepivot = Mathf.Max(0f, -myposy - m_SUI.Bucket.localPosition.y); 
		m_SUI.Bucket.localPosition += new Vector3(0f, movepivot, 0f);

		float listtop = m_SUI.ListTopDownCheck[0].position.y + m_SUI.ListTopDownCheck[0].rect.yMin;
		RectTransform top = m_Elements[0].GetComponent<RectTransform>();
		float toptop = top.position.y + top.rect.yMax;
		float interval = Mathf.Max(0f, listtop - toptop);
		m_SUI.Bucket.localPosition += new Vector3(0f, interval, 0f);

		float listbtm = m_SUI.ListTopDownCheck[1].position.y + m_SUI.ListTopDownCheck[1].rect.yMax;
		RectTransform last = m_Elements[m_Elements.Count - 1].GetComponent<RectTransform>();
		float lastbtm = last.position.y + last.rect.yMin;
		interval = Mathf.Max(0f, lastbtm - listbtm);
		m_SUI.Bucket.localPosition -= new Vector3(0f, interval, 0f);

		//StartCoroutine(IE_Change());
	}

	IEnumerator IE_Change() {
		yield return new WaitForSeconds(1f);
		Vector3 premypos = m_MyElement.transform.position;
		Vector3 gopos = m_Elements[m_Order[1] - 1].transform.position;
		Vector3[] otherprepos = new Vector3[m_Elements.Count];
		for (int i = 0; i < m_Elements.Count; i++) {
			Vector3 nowpos = m_Elements[m_Elements[i].m_Info[1].Rank - 1].transform.position;
			iTween.MoveTo(m_Elements[i].gameObject, nowpos, 1f);
			m_Elements[i].OrderChange();
		}
		iTween.MoveBy(m_SUI.Bucket.gameObject, gopos - premypos, 1f);
	}
	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(_result);
	}
}
