using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_ItemSquare_RewardGroup : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Image Icon;
		public Image BG;
		public Image Frame;
		public TextMeshProUGUI Grade;
		public GameObject GradeGroup;
		public GameObject Special;
	}
	[SerializeField] SUI m_SUI;

	public void SetData(ItemType _type, int[] _grades, int _idx, bool _sp = false) {
		m_SUI.Icon.sprite = BaseValue.GetGroupItemIcon(_type);
		m_SUI.BG.sprite = BaseValue.ItemGradeBG(_type, _grades[1]);
		m_SUI.Frame.sprite = BaseValue.GradeFrame(_type, _grades[1], _idx);
		if (_type == ItemType.DNAMaterial) {
			m_SUI.Frame.color = BaseValue.RNAFrameColor(_idx)[0];
		}
		m_SUI.GradeGroup.SetActive(_grades[0] != _grades[1]);
		string grade = _grades[0] == _grades[1] ? _grades[0].ToString() : string.Format("{0}~{1}", _grades[0], _grades[1]);
		m_SUI.Grade.text = string.Format("{0}<size=80%>{1}</size>", grade, TDATA.GetString(274));
		m_SUI.Anim.SetInteger("Grade", _grades[1]);
		m_SUI.Special.SetActive(_sp);
	}
}
