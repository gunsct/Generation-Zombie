using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Info_Item : PopupBase
{
	public enum InfoChange
	{
		None = 0,
		Use,
		UnEquip,
		Equip,
		LVUP,
		End
	}
#pragma warning disable 0649
	[System.Serializable]
	public struct SUI
	{
		public Item_Inventory_Item Item;
		public Image GradeBG;
		public TextMeshProUGUI GradeName;
		public TextMeshProUGUI Name;
	}
	[SerializeField] protected SUI m_SUI;
	[SerializeField] protected Animator m_Ani;

	protected Action<InfoChange, object[]> m_ChangeCB;
	protected ItemInfo m_Info;
	protected PopupName m_Parent;
	protected IEnumerator m_Action;
#pragma warning restore 0649
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		m_Info = (ItemInfo)aobjValue[0];
		m_Parent = (PopupName)aobjValue[1];
		m_ChangeCB = (Action<InfoChange, object[]>)aobjValue[2];

		base.SetData(pos, popup, cb, aobjValue);
		StartCoroutine(StartAniCheck());
	}

	public override void SetUI()
	{
		m_SUI.Item.SetData(m_Info, null, null, LockMarkMode: Item_RewardItem_Card.LockActiveMode.Normal);
		m_SUI.Item.StateChange(Inventory.EState.Normal);
		m_SUI.Name.text = m_Info.GetName();
		if (m_SUI.GradeName != null) m_SUI.GradeName.text = m_Info.GetGradeGroupName();
		
		ItemInvenGroupType group = m_Info.m_TData.GetInvenGroupType();
		// 등급 정보
		if (m_SUI.GradeName != null) m_SUI.GradeName.color = BaseValue.GradeColor(m_Info.m_Grade);
		if (m_SUI.GradeBG != null) m_SUI.GradeBG.gameObject.SetActive(group != ItemInvenGroupType.CharaterPiece);
		if (m_SUI.GradeBG != null) m_SUI.GradeBG.sprite = BaseValue.GetInfoGradeBG(m_Info.m_Grade);
	}
	public override void Close(int Result = 0)
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		if (m_Ani == null)
		{
			base.Close(Result);
			return;
		}
		StartCoroutine(m_Action = StartEndAni(Result));
	}


	IEnumerator StartAniCheck()
	{
		yield return Utile_Class.CheckAniPlay(m_Ani);
		if (TUTO.IsTuto(TutoKind.EquipCharLVUP, (int)TutoType_EquipCharLVUP.Focus_Weapon)) TUTO.Next();
	}

	IEnumerator StartEndAni(int Result)
	{
		m_Ani.SetTrigger("Close");
		yield return Utile_Class.CheckAniPlay(m_Ani);

		base.Close(Result);
	}
}
