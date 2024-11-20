using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EF_BuffCenterAlarm : ObjMng
{
	[System.Serializable]
	public struct SUI
	{
		public GameObject AlarmPrefab;
		public Transform[] UPDownParent;
	}
	[SerializeField]
	SUI m_SUI;

	GameObject m_AlarmObj;
	Coroutine m_Cor;

	/// <summary> 카드 타입과 object 값으로 호출 </summary>
	/// <param name="_obj"> 포지션용 </param>
	/// <param name="_type"> 카드 타입</param>
	/// <param name="_val"> 생존스탯4개만 int 값 나머지는 string </param>
	public void SetData(Transform _obj, StageCardType _type, object _val, StageCardInfo _info = null) {
		if (_obj == null) transform.position = Vector3.zero;
		else transform.position = Utile_Class.GetCanvasPosition(_obj.position);
		transform.localPosition = new Vector3(POPUP.GetComponent<RectTransform>().rect.width * 0.5f, 0f, 0f);

		Item_BuffCenterAlarm alarm = Utile_Class.Instantiate(m_SUI.AlarmPrefab, (float)_val > 0 ? m_SUI.UPDownParent[0] : m_SUI.UPDownParent[1]).GetComponent<Item_BuffCenterAlarm>();
		if ((float)_val > 0) alarm.transform.SetAsLastSibling();
		else alarm.transform.SetAsFirstSibling();
		alarm.SetData(_type, _val, _info);
		m_AlarmObj = alarm.gameObject;

		if (m_Cor != null)
			StopCoroutine(m_Cor);
		m_Cor = StartCoroutine(IE_AutoDestroy());
	}
	/// <summary> 스탯 타입별로 단일 사용시 </summary>
	public void SetData(Transform _obj, StatType _type, int _val, StageCardInfo _info = null, string _name = null) {
		if (_val == 0) return;
		if (_obj == null) transform.position = Vector3.zero;
		else transform.position = Utile_Class.GetCanvasPosition(_obj.position);
		transform.localPosition = new Vector3(POPUP.GetComponent<RectTransform>().rect.width * 0.5f, 0f, 0f);

		Item_BuffCenterAlarm alarm = Utile_Class.Instantiate(m_SUI.AlarmPrefab, _val > 0 ? m_SUI.UPDownParent[0] : m_SUI.UPDownParent[1]).GetComponent<Item_BuffCenterAlarm>();
		if (_val > 0) alarm.transform.SetAsLastSibling();
		else alarm.transform.SetAsFirstSibling();
		alarm.SetData(_type, _val, _name == null ?string.Empty : _name, _info);
		m_AlarmObj = alarm.gameObject;

		if (m_Cor != null)
			StopCoroutine(m_Cor);
		m_Cor = StartCoroutine(IE_AutoDestroy());
	}
	IEnumerator IE_AutoDestroy() {
		yield return new WaitForEndOfFrame();
		yield return new WaitUntil(() => m_AlarmObj == null);
		Destroy(gameObject);
	}
}
