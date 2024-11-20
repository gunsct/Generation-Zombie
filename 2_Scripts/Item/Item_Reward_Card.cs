using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item_Reward_Card : ObjMng
{
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public Animator Anim;
		public BoxCollider Collider;
		public Transform Panel;
		public SpriteRenderer Img;
		public TextMeshPro Name;
		public TextMeshPro Sub;
		public TextMeshPro InfoSub;
		public TextMeshPro InfoDesc;
	}

	[SerializeField] SUI m_SUI;
	public StageRewardState m_State;
	public StageCardInfo m_Info;
	public void SetData(StageCardInfo _reward, StageRewardState _state, bool _real = false) {
		m_Info = _reward;
		m_State = _state;

		
		//if (m_SUI.Collider != null) m_SUI.Collider.enabled = IsLock();
		m_SUI.Name.text = _state == StageRewardState.Blind ? "?????" : _reward.GetName() ;
		m_SUI.Img.sprite = _state == StageRewardState.MenLow ? UTILE.LoadImg("Card/Stage/Stage_0", "png") : _reward.GetImg();
		if (m_SUI.InfoSub != null) m_SUI.InfoSub.text = _state == StageRewardState.Blind ? "?????" : TDATA.GetString(48) ;
		if (m_SUI.InfoDesc != null) m_SUI.InfoDesc.text = _state == StageRewardState.Blind ? "?????" : _reward.m_GambleIdx == 0 ? _reward.GetDesc() : TDATA.GetGambleCardTable(_reward.m_GambleIdx).GetAllDesc();

		if (m_SUI.Anim != null) {
			m_SUI.Anim.SetTrigger(_real ? "Flip2" : m_State == StageRewardState.Blind ? "Back" : "Flip");
			m_SUI.Anim.speed = _real ? 1f : 1.2f;
		}
	}
	private void OnEnable() {
		if (m_SUI.Anim != null) {
			m_SUI.Anim.SetTrigger(m_State == StageRewardState.Blind ? "Back" : "Flip");
			m_SUI.Anim.speed = 1.2f;
		}
	}
	public bool IsLock() {
		return m_State != StageRewardState.Normal && m_State != StageRewardState.Blind;
	}
}
