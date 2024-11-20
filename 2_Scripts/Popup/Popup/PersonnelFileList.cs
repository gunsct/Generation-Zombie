using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PersonnelFileList : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Transform FilePrefab;//Item_PersonnelFileChange
		public Transform FIleBucket;
		public GameObject[] GradeTapOn;
		public GameObject[] GradeTapOff;
	}
	[SerializeField] SUI m_SUI;
	List<Item_PersonnelFileChange> m_Files = new List<Item_PersonnelFileChange>();
	IEnumerator m_Action;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		List<TCharacterTable> tables =  new List<TCharacterTable>(TDATA.GetAllCharacterTable().Values);
		UTILE.Load_Prefab_List(tables.Count, m_SUI.FIleBucket, m_SUI.FilePrefab);

		for (int i = tables.Count - 1; i >= 0 ; i--) {
			int filecnt = USERINFO.GetItemCount(tables[i].m_PieceIdx);
			Item_PersonnelFileChange card = m_SUI.FIleBucket.GetChild(i).GetComponent<Item_PersonnelFileChange>();
			m_Files.Add(card);
			int grade = TDATA.GetItemTable(tables[i].m_PieceIdx).m_Grade;
			card.SetData(tables[i].m_PieceIdx, grade, filecnt, ChangeCB);
		}

		m_Files.Sort((Item_PersonnelFileChange _bf, Item_PersonnelFileChange _af) => {
			if (_bf.m_Grade != _af.m_Grade) return _bf.m_Grade.CompareTo(_af.m_Grade);
			return _bf.m_Idx.CompareTo(_af.m_Idx);
		});
		for (int i = m_Files.Count - 1; i >= 0; i--) {
			m_Files[i].transform.SetAsLastSibling();
		}
		Click_ViewGrade(0);
	}
	
	void ChangeCB() {
		if (m_Action != null) return;
		for (int i = 0;i< m_Files.Count; i++) {
			int filecnt = USERINFO.GetItemCount(m_Files[i].m_Idx);
			m_Files[i].SetData(m_Files[i].m_Idx, m_Files[i].m_Grade, filecnt, ChangeCB);
		}
	}

	public void Click_ViewGrade(int _grade = 0) {
		if (m_Action != null) return;
		for (int i = 0;i< m_Files.Count; i++) {
			m_Files[i].gameObject.SetActive(_grade == 0 ? true : m_Files[i].m_Grade == _grade);
		}
		for(int i = 0; i < m_SUI.GradeTapOn.Length; i++) {
			m_SUI.GradeTapOn[i].SetActive(i == _grade);
			m_SUI.GradeTapOff[i].SetActive(i != _grade);
		}
	}
	public override void Close(int Result = 0) {
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int Result) {
		m_SUI.Anim.SetTrigger("Close");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(Result);
	}
}
