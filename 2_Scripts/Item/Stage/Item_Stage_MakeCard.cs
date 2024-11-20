using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Item_Stage_MakeCard : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public Transform Bucket;
		public Image Icons;
		public Animator Anim;
		public GameObject Frame;
		public CanvasGroup Alpha;
	}
	[SerializeField]
	SUI m_SUI;
	public StageMaterialType m_MatType;
	Action<Item_Stage_MakeCard> m_CB;
	public TStageMakingTable m_TData;
	public Vector3 m_SPos;
	Canvas SortCanvas;

	/// <summary> 기본 재료 세팅 </summary>
	public void SetData(StageMaterialType _type = StageMaterialType.None, TStageMakingTable _data = null, Action<Item_Stage_MakeCard> _selectcb = null) {
		if((int)_type <= (int)StageMaterialType.DefaultMat){//기본 재료
			m_SUI.Icons.sprite = UTILE.LoadImg(string.Format("Card/Stage/Material_{0}", (int)_type + 1), "png");
			m_SUI.Frame.SetActive(false);
			if(m_SUI.Anim != null)  m_SUI.Anim.SetTrigger("MaterialItem");
		}
		else {//유틸
			TStageCardTable table = TDATA.GetStageCardTable(_data.m_CardIdx);
			m_SUI.Icons.sprite = table.GetImg();
			m_SUI.Frame.SetActive(true);
			SetAnim("UseItem");
			m_CB = _selectcb;
		}
		m_SUI.Alpha.alpha = 1f;
		m_MatType = _type;
		m_TData = _data;
		m_CB = _selectcb;
	}
	public IEnumerator IE_SetDelayAnim(float _delay, string _anim) {
		yield return new WaitForSeconds(_delay);
		SetAnim(_anim);
		if (_anim.Equals("Discard")) {
			yield return new WaitForSeconds(0.1f);
			UTILE.LoadPrefab("Effect/Stage/Eff_MakeCard_DiscardEffect", true, transform);
		}
	}
	public void SetDelayAnim(float _delay, string _anim) {
		StartCoroutine(IE_SetDelayAnim(_delay, _anim));
	}
	public void SetAnim(string _anim) {
		if (m_SUI.Anim != null) {
			m_SUI.Anim.SetTrigger(_anim);
		}
	}
	/// <summary> 머지 애니 끝나고 삭제</summary>
	public void Merge(float _time) {
		SetAnim("Merge");
		Destroy(gameObject, _time);
	}
	public void Return(float _time) {
		Destroy(gameObject, _time);
	}

	public void StartRemoveAction()
	{
		m_CB = null;
		StartCoroutine(RemoveAction());
	}

	IEnumerator RemoveAction()
	{
		Vector3[] path = GetOutPath(transform.position);
		iTween.RotateAdd(gameObject, iTween.Hash("z", path[0].x > path[2].x ? 750f : -750f, "time", 1.3f, "easetype", "easeOutCirc"));
		SetFade(1.2f, 0f);
		//iTween.ColorTo(gameObject, iTween.Hash("a", 0f, "time", 0.5f, "easetype", "easeOutQuint"));
		iTween.MoveTo(gameObject, iTween.Hash("path", path, "time", 1.3f, "easetype", "easeOutQuint"));
		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));
		Return(0);
	}
	public void SetFade(float _time, float _delay) {
		iTween.ValueTo(gameObject, iTween.Hash("from", 1f, "to", 0f, "onupdate", "TW_Fade", "time", _time, "delay", _delay, "easetype", "easeOutCubic"));
	}
	void TW_Fade(float _amount) {
		m_SUI.Alpha.alpha = _amount;
	}
	public void BucketOff() {
		m_SUI.Bucket.gameObject.SetActive(false);
	}
	/// <summary> 버킷을 이동, 시작은 월드 끝은 로컬</summary>
	public void MoveBucket(Vector3 _spos, Vector3 _epos, float _time) {
		m_SUI.Bucket.position = _spos;
		m_SUI.Bucket.gameObject.SetActive(true);
		iTween.StopByName(gameObject, "Move");
		iTween.MoveTo(m_SUI.Bucket.gameObject, iTween.Hash("position", _epos, "time", _time, "easetype", "easeOutCubic", "islocal", true, "name", "Move"));
	}
	public void SetStartPos(Vector3 _pos) {
		m_SPos = _pos;
	}
	public void ClickCard() {
		//if (!MAIN.IS_State(MainState.STAGE)) return;
		//if (POPUP.IS_PopupUI() && POPUP.GetPopup().m_Popup != PopupName.Stage_Reward) return;
		//if (POPUP.IS_MsgUI()) return;
		//if (STAGE.IS_SelectAction()) return;//다른 행동중에도 가능
		if (STAGE.m_MainUI.GetCraft().GetState() != Item_Stage_Make.State.None) return;//제작대가 작동중일 때는 사용 불가
		m_CB?.Invoke(this);
	}
	public void SetBlock() {
		m_SUI.Icons.gameObject.SetActive(false);
	}
	public Vector3[] GetOutPath(Vector3 _spos, float _hrtio = 1f) {
		Vector3[] path = new Vector3[3];
		path[0] = _spos;
		path[2] = new Vector3(path[0].x + (UTILE.Get_Random(0,2) == 0 ? 1f : -1f) * UTILE.Get_Random(30, 40) * 10f, path[0].y + (536f + UTILE.Get_Random(-19, 20) * 10f) * _hrtio, 0f);
		path[1] = new Vector3((path[0].x + path[2].x) * 0.5f, path[2].y + (path[2].y - path[0].y) * 0.2f, 0f);
		return path;
	}
}
