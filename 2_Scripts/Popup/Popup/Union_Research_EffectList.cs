using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static LS_Web;
using TMPro;
using System.Linq;
using DanielLochner.Assets.SimpleScrollSnap;

public class Union_Research_EffectList : PopupBase
{
	[Serializable]
    public struct SUI
	{
		public Animator Anim;

		public GameObject Empty;

		public ScrollRect Scroll;
		public RectTransform Prefab;
	}

	[SerializeField] SUI m_SUI;

	bool IsStart;
	int MyGuildStep;
	int Select;
	int LV;
	long Exp;
	List<TGuild_ResearchTable> TDataList = new List<TGuild_ResearchTable>();

	private IEnumerator Start()
	{
		yield return Utile_Class.CheckAniPlay(m_SUI.Anim);
		IsStart = true;
	}

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		IsStart = false;
		base.SetData(pos, popup, cb, aobjValue);
	}

	public override void SetUI() {
		base.SetUI();
		LoadList();
	}

	public void LoadList()
	{
		GuildInfo Info = USERINFO.m_Guild;
		if(Info.EndRes.Count < 1)
		{
			m_SUI.Empty.SetActive(true);
			m_SUI.Scroll.viewport.gameObject.SetActive(false);
		}
		else
		{
			m_SUI.Empty.SetActive(false);
			m_SUI.Scroll.viewport.gameObject.SetActive(true);

			var list = new List<TResearchTable.Effect>();
			for (int i = Info.EndRes.Count - 1; i > -1; i--)
			{
				var tdata = TDATA.GetGuildRes(Info.EndRes[i]);
				var data = list.Find(o => o.m_Eff == tdata.m_Eff.m_Eff);
				if (data == null)
				{
					data = new TResearchTable.Effect();
					data.m_Eff = tdata.m_Eff.m_Eff;
					data.m_Value = 0;
					list.Add(data);
				}
				data.m_Value += tdata.m_Eff.m_Value;
			}

			list.Sort((befor, after) =>
			{
				return befor.m_Eff.CompareTo(after.m_Eff);
			});

			int Max = list.Count;
			UTILE.Load_Prefab_List(Max, m_SUI.Scroll.content, m_SUI.Prefab);

			for (int i = 0; i < Max; i++)
			{
				Item_Union_Research_EffectList_Element element = m_SUI.Scroll.content.GetChild(i).GetComponent<Item_Union_Research_EffectList_Element>();
				element.SetData(list[i]);
			}
		}
	}

#region Btn
	public override void Close(int Result = 0)
	{
		if (!IsStart) return;
		StartCoroutine(CloseAction(Result));
	}
	IEnumerator CloseAction(int _result)
	{
		IsStart = false;
		m_SUI.Anim.SetTrigger("Close");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(_result);
	} 
#endregion
}
