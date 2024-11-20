using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;
using System.Linq;

[System.Serializable] public class DicCollectionTypeSprite : SerializableDictionary<CollectionType, Sprite> { }

public class Item_PDA_Collection_BuffList : Item_PDA_Base
{
	[Serializable]
	public struct SUI
	{
		public Transform Bucket;
		public GameObject TitleElement;      //Item_PDA_Collection_BuffList_Element_Title
		public GameObject StatElement;       //Item_PDA_Collection_BuffList_Element_Stat
		public Animator Ani;
	}
	[SerializeField] SUI m_SUI;
	CollectionType m_Type;

	public override void SetData(Action<object, object[]> CloaseCB, object[] args) {
		
		base.SetData(CloaseCB, args);
		m_Type = (CollectionType)args[0];
		SetUI();
	}
	void SetUI()
	{
		for(int i = m_SUI.Bucket.childCount -1;i> -1; i--) {
			DestroyImmediate(m_SUI.Bucket.GetChild(i).gameObject);
		}
		//컬렉션 순서대로 있으면 타이틀 만들고 아래 달아주기
		for (CollectionType type = CollectionType.Character; type < CollectionType.END; type++) {
			int cnt = 0;
			for (StatType stat = StatType.Men; stat < StatType.Max; stat++) {
				float val = USERINFO.m_Collection.GetStatValue(stat, type);
				if(val > 0) {
					Item_PDA_Collection_BuffList_Element_Stat statelement = Utile_Class.Instantiate(m_SUI.StatElement, m_SUI.Bucket).GetComponent<Item_PDA_Collection_BuffList_Element_Stat>();
					statelement.SetData(stat, val);
					cnt++;
				}
			}
			if (cnt > 0) {
				Item_PDA_Collection_BuffList_Element_Title titleelement = Utile_Class.Instantiate(m_SUI.TitleElement, m_SUI.Bucket).GetComponent<Item_PDA_Collection_BuffList_Element_Title>();
				titleelement.SetData(type);
				titleelement.transform.SetSiblingIndex(m_SUI.Bucket.childCount - 1 - cnt);
			}
		}

	}

	void SetList()
	{
		//List<TCollectionTable> list = USERINFO.m_Collection.GetBuffList(m_View);
		//RectTransform content = m_SUI.Scroll.content;
		//UTILE.Load_Prefab_List(list.Count, content, m_SUI.Prefab);

		//for (int i = 0, iMax = list.Count; i < iMax; i++)
		//{
		//	content.GetChild(i).GetComponent<Item_PDA_Collection_BuffList_Element>().SetData(list[i]);
		//}
		//InitScrollPosition();
	}
	public override void OnClose()
	{
		m_CloaseCB?.Invoke(Item_PDA_Achieve.State.Collection_Main, new object[] { m_Type });
	}
}
