using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item_TowerCard : ObjMng
{
	public enum AniName
	{
		Start = 0,
		End
	}

	[Serializable]
	struct SCnt
	{
		public GameObject Active;
		public TextMeshPro Cnt;
	}
	[SerializeField] SCnt m_CntUI;
	[SerializeField] Animator m_Ani;
	[SerializeField] ItemCardRenderer m_Icon;
	[SerializeField] TextMeshPro m_Label;
	[HideInInspector] public StageCardInfo m_Info;
	[HideInInspector] public TTowerMapTable m_MapData;
	int m_MaxCnt;
	public bool m_Lock;

	public void SetData(TStageCardTable table, TTowerMapTable _map)
	{
		m_Info = new StageCardInfo(table.m_Idx);
		m_MapData = _map;

		m_Label.text = m_Info.GetName();
		if (m_Info.m_TData.m_Type == StageCardType.CountBox) m_CntUI.Active.SetActive(true);
		else m_CntUI.Active.SetActive(false);

		Sprite Img = m_Info.GetImg();
		m_Icon.Init();
		m_Icon.SetMainTexture(Img);
		m_Icon.SetTexture();

		SetLock();
	}

	public void SetLock(TTowerMapTable _table = null) {
		bool isnext = false;
		if (_table == null) m_Lock = true;
		else {
			for (int i = 0; i < _table.m_Ways.Length; i++) {
				if (_table.m_Ways[i] == m_MapData.m_Idx) isnext = true;
			}
			m_Lock = (m_MapData.m_Row != _table.m_Row + 1 || !isnext);
		}
		SetColor(m_Lock ? Color.gray : Color.white);
	}
	public void SetCnt(int cnt, int max)
	{
		m_MaxCnt = max;
		m_CntUI.Cnt.text = string.Format("{0}/{1}", cnt, max);
	}

	public void SetColor(Color color)
	{
		m_Icon.SetColor(color);
	}

	public void PlayAni(AniName ani, float waittime, Action EndCB = null)
	{
		StartCoroutine(Ani_EndCheck(ani, waittime, EndCB));
	}

	IEnumerator Ani_EndCheck(AniName ani, float waittime, Action EndCB)
	{
		yield return new WaitForSeconds(waittime);
		m_Ani.SetTrigger(ani.ToString());
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_Ani));
		EndCB?.Invoke();
	}
}
