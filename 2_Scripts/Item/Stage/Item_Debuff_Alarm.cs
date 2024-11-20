using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Item_Debuff_Alarm : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Desc;
		public Image Icon;
	}
	[SerializeField]
	SUI m_SUI;
	TStatusDebuffTable m_TData;

	private void OnDisable() {
		m_TData = null;
	}
	//tem_Debuff_Alram 출력 -> 100프레임에 Item_StgModeAlram_Debuff 애니메이션 Start 출력
	public void SetData(TStatusDebuffTable _table) {
		m_SUI.Anim.speed = 1f / Time.timeScale;
		if(m_TData != null && m_TData != _table) m_SUI.Anim.SetTrigger("Start");
		switch (_table.m_StatType) {
			case StatType.Men:
				m_SUI.Icon.sprite = UTILE.LoadImg("UI/Icon/Icon_Gimmik_97", "png");
				break;
			case StatType.Hyg:
				m_SUI.Icon.sprite = UTILE.LoadImg("UI/Icon/Icon_Gimmik_98", "png");
				break;
			case StatType.Sat:
				m_SUI.Icon.sprite = UTILE.LoadImg("UI/Icon/Icon_Gimmik_99", "png");
				break;
		}

		m_SUI.Name.text = _table.GetName();
		m_SUI.Desc.text = _table.GetDesc();

		m_TData = _table;
	}
	public bool IS_AlarmTiming(float _normal = 1f) {
		return Utile_Class.IsAniPlay(m_SUI.Anim, _normal);
	}
}
