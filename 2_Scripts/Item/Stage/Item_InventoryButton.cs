using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_InventoryButton : ObjMng
{
	[SerializeField] GameObject m_NewAlarm;
	[SerializeField] GameObject m_WarnGlow;
	[SerializeField] GameObject m_WarnAlarm;

	private void Awake() {
		if (MainMng.IsValid()) {
			DLGTINFO.f_RFInvenUI += SetData;
		}
	}
	private void OnDestroy() {
		if (MainMng.IsValid() && DLGTINFO != null) {
			DLGTINFO.f_RFInvenUI -= SetData;
		}
	}
	public void SetData(bool newget, bool capwarn) {
		m_NewAlarm.SetActive(newget);
		m_WarnAlarm.SetActive(capwarn);
		m_WarnGlow.SetActive(capwarn);
	}
	public void ClickInven() {
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Inventory, (result, obj) => { });
	}
}
