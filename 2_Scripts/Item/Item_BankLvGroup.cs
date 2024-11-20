using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Item_BankLvGroup : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public GameObject First;
		public GameObject Empty;
		public Item_DG_Bank_Stg[] Elements;
	}
	[SerializeField] SUI m_SUI;

	private void Awake() {
		m_SUI.First.SetActive(false);
		m_SUI.Empty.SetActive(false);
		for (int i = 0; i < 3; i++) m_SUI.Elements[i].gameObject.SetActive(false);
	}

	public int SetData(bool _first, int _type, int _gencnt, int _genmax) {//first{0} ,{2,1,x,0,x} 루프
		int cnt = 0;
		if (_genmax - _gencnt < 3) {
			for (int i = 0; i < _genmax - _gencnt; i++) {
				m_SUI.Elements[i].gameObject.SetActive(true);
				cnt++;
			}
		}
		else {
			if (_first) {
				m_SUI.First.SetActive(true);
				for (int i = 0; i < 2; i++) {
					m_SUI.Elements[i].gameObject.SetActive(true);
					cnt++;
				}
			}
			else {
				switch (_type) {
					case 0:
						for (int i = 0; i < 2; i++) {
							m_SUI.Elements[i].gameObject.SetActive(true);
							cnt++;
						}
						m_SUI.Empty.SetActive(true);
						m_SUI.Empty.transform.SetSiblingIndex(3);
						break;
					case 1:
						for (int i = 0; i < 2; i++) {
							m_SUI.Elements[i].gameObject.SetActive(true);
							cnt++;
						}
						m_SUI.Empty.SetActive(true);
						m_SUI.Empty.transform.SetSiblingIndex(2);
						break;
					case 3:
						for (int i = 0; i < 2; i++) {
							m_SUI.Elements[i].gameObject.SetActive(true);
							cnt++;
						}
						m_SUI.Empty.SetActive(true);
						m_SUI.Empty.transform.SetSiblingIndex(1);
						break;
					case 2:
					case 4:
						for (int i = 0; i < 3; i++) {
							m_SUI.Elements[i].gameObject.SetActive(true);
							cnt++;
						}
						break;
				}
			}
		}
		return cnt;
	}

	//세팅된것들 
	public List<Item_DG_Bank_Stg> GetElements() {
		List<Item_DG_Bank_Stg> elements = new List<Item_DG_Bank_Stg>();
		for(int i = 0; i < m_SUI.Elements.Length; i++) {
			if (m_SUI.Elements[i].gameObject.activeSelf) elements.Add(m_SUI.Elements[i]);
		}
		return elements;
	}
}
