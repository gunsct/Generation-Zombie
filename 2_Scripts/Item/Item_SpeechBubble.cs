using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item_SpeechBubble : ObjMng
{
	[System.Serializable]
	public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI Speech;
		public Transform Tail;
		public GameObject[] TailObjs;
		public CanvasGroup m_Alpha;
	}
	[SerializeField]
	SUI m_SUI;
	float m_Timer = 0f;
	Transform m_Target;
	Vector3 m_InitPos;
	TConditionDialogueGroupTable m_TData;
	int m_Step = 0;
	List<int> m_UsePos = new List<int>();
	private void Update() {
		if (m_Timer > 0f)
			m_Timer -= Time.deltaTime;
		else if (m_TData.m_Personalitys.Count > m_Step && m_TData.m_Personalitys[m_Step].Prop > UTILE.Get_Random(0f, 1f)) {
			if (m_UsePos.Count < 1) for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) m_UsePos.Add(i);
			Vector3 pos = m_InitPos;
			int charpos = UTILE.Get_Random(0, m_UsePos.Count);
			m_UsePos.RemoveAt(charpos);
			Item_Stage_Char info = STAGE.m_Chars[charpos];
			m_Target = info.transform;
			//pos = Utile_Class.GetCanvasPosition(info.transform.position + new Vector3(0f, -info.transform.position.y - 1f, 0f));
			SetData(m_Target, m_TData, 1.5f, ++m_Step);//pos
		}
		else {
			m_Step = 0;
			m_Timer = 0f;
			m_Target = null;
			gameObject.SetActive(false);
		}
		if (gameObject.activeSelf && m_Target != null) {
			transform.position = Utile_Class.GetCanvasPosition(m_Target.position + new Vector3(0f, -m_Target.position.y - 1f, 0f));
		}

		if(POPUP.IS_PopupUI() && m_SUI.m_Alpha.alpha > 0) {
			m_SUI.m_Alpha.alpha -= 2 * Time.deltaTime;
		}
		else if(!POPUP.IS_PopupUI() && m_SUI.m_Alpha.alpha < 1) {
			m_SUI.m_Alpha.alpha += 2 * Time.deltaTime;
		}
	}
	public void SetData(Transform _target, TConditionDialogueGroupTable _table, float _time, int _step = 0) {
		m_Target = _target;
		SetData(Utile_Class.GetCanvasPosition(m_Target.position + new Vector3(0f, -m_Target.position.y - 1f, 0f)), _table, _time, _step);
	}
	public void SetData(Vector3 _pos, TConditionDialogueGroupTable _table, float _time, int _step = 0) {
		if (m_Timer > 0f) return;
		transform.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, 500f);
		m_TData = _table;

		transform.position = _pos;
		if (_step == 0) m_InitPos = _pos;
		if (_step > 0) {
			if (m_TData.m_Personalitys.Count <= _step - 1 || TDATA.GetPersonalityDialogueGroupTable(m_TData.m_Condition, m_TData.m_Personalitys[_step - 1].Type) == null) {
				return; 
			}
			m_SUI.Speech.text = TDATA.GetPersonalityDialogueGroupTable(m_TData.m_Condition, m_TData.m_Personalitys[_step - 1].Type).GetDesc();
		}
		else m_SUI.Speech.text = m_TData.GetStr();

		gameObject.SetActive(true);

		m_Timer = _time;
		m_SUI.Anim.SetTrigger(m_TData.m_SpeechType.ToString());
		for(int i = 0; i < 5; i++) {
			m_SUI.TailObjs[i].SetActive(i == (int)m_TData.m_SpeechType);
		}
		SetCommon();
		m_SUI.Tail.position = new Vector3(_pos.x, m_SUI.Tail.position.y, m_SUI.Tail.position.z);
	}
	public void SetData(Transform _target, string _txt, float _time) {
		m_Target = _target;
		SetData(Utile_Class.GetCanvasPosition(m_Target.position + new Vector3(0f, -m_Target.position.y - 1f, 0f)), _txt, _time);
	}
	public void SetData(Vector3 _pos, string _txt, float _time) {
		transform.position = _pos;
		m_SUI.Speech.text = _txt;

		gameObject.SetActive(true);

		m_Timer = _time;
		m_SUI.Anim.SetTrigger(CharDialogueType.Say.ToString());
		for (int i = 0; i < 5; i++) {
			m_SUI.TailObjs[i].SetActive(i == (int)CharDialogueType.Say);
		}
		SetCommon();
		m_SUI.Tail.position = new Vector3(_pos.x, m_SUI.Tail.position.y, m_SUI.Tail.position.z);
	}
	void SetCommon() {
		//좌우 나가는거 고려해서 밀어주기
		Canvas.ForceUpdateCanvases();
		float size = m_SUI.Speech.transform.parent.GetComponent<RectTransform>().sizeDelta.x / 2f;
		if (transform.position.x - size < 0)
			transform.localPosition += new Vector3(-(transform.position.x - size), 0f, 0f);
		else if (transform.position.x + size > Screen.width)
			transform.localPosition -= new Vector3(transform.position.x + size - Screen.width, 0f, 0f);

	}
	void TW_Fade(float _amount) {
		m_SUI.m_Alpha.alpha = _amount;
	}
}
