using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;

public class Item_PVP_Main_EnemyChar : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Item_CharMember_PVP[] AtkDef;
		public GameObject[] SecretObjs;
		public GameObject[] NonSecretObjs;
	}
	[SerializeField] SUI m_SUI;

	public void SetData(RES_PVP_CHAR[] _infos, bool _open) {
		for(int i = 0; i < m_SUI.SecretObjs.Length; i++) {
			m_SUI.SecretObjs[i].SetActive(!_open);
		}
		for (int i = 0; i < m_SUI.NonSecretObjs.Length; i++) {
			m_SUI.NonSecretObjs[i].SetActive(_open);
		}
		for (int i = 0; i < _infos.Length; i++) {
			m_SUI.AtkDef[i].SetData(_infos[i]);
		}
	}
}
