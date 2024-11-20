using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class Down_CDN : PopupBase
{
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public GameObject Active;
		public Image Gauge;
		public TextMeshProUGUI FileName;
		public TextMeshProUGUI ConceptDesc;
		public TextMeshProUGUI DownSize;
		public Transform TutoBucket;
		public List<string> PrefabNames;
	}

	[SerializeField] SUI m_sUI;
	List<file_data> m_Files;
	public List<file_data> m_ErrorFiles = new List<file_data>();
	long m_TotalSize = 0;
	long m_SucSize = 0;
	file_data m_DownFile;
	Item_CDN_Tuto m_Tuto;
#pragma warning restore 0649

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		base.SetData(pos, popup, cb, aobjValue);
		m_Files = (List<file_data>)aobjValue[0];
		m_TotalSize = m_Files.Sum(o => o.size);
		StartCoroutine(ShowToolTip());
	}

	public void Start()
	{
		m_sUI.Active.SetActive(false);
		Start_DownLoadMsg();
	}

	void Start_DownLoadMsg()
	{
		// 패치 데이터가 있음
		POPUP.Set_MsgBox(PopupName.Msg_CDN, (btn, obj) =>
		{
			if ((EMsgBtn)btn == EMsgBtn.BTN_YES)
			{
				StartDownLoad();
				StartCoroutine(ShowTuto());
			}
			else Close(-1);
		}, m_TotalSize);
	}

	IEnumerator ShowToolTip()
	{
		while(true)
		{
			m_sUI.ConceptDesc.text = TDATA.GetToolTip();
			yield return new WaitForSeconds(2.5f);
		}
	}

	IEnumerator ShowTuto() {
		List<string> prefabs = new List<string>(m_sUI.PrefabNames);
		string tip = string.Empty;

		while (true) {
			if (prefabs.Count < 1)
				prefabs = new List<string>(m_sUI.PrefabNames);

			int idx = UTILE.Get_Random(0, prefabs.Count);
			tip = string.Format("Item/{0}", prefabs[idx]);
			prefabs.RemoveAt(idx);

			m_Tuto = UTILE.LoadPrefab(tip, true, m_sUI.TutoBucket).GetComponent<Item_CDN_Tuto>();
			m_Tuto.SetData(Item_CDN_Tuto.State.CDN);
			yield return new WaitWhile(() => m_Tuto != null);
		}
	}
	void StartDownLoad()
	{
		m_sUI.Active.SetActive(true);
		m_ErrorFiles.Clear();
		m_SucSize = 0;
		m_sUI.Gauge.fillAmount = 0;
		//m_sUI.FileName.text = "";
		m_sUI.DownSize.text = string.Format("({0} / {1})", UTILE.Get_FileSize(m_SucSize), UTILE.Get_FileSize(m_TotalSize));
		WEB.StartDownLoad(m_Files, (file) =>
		{
			if(m_DownFile != null)
			{
				m_SucSize += m_DownFile.size;
				m_sUI.DownSize.text = string.Format("({0} / {1})", UTILE.Get_FileSize(m_SucSize), UTILE.Get_FileSize(m_TotalSize));
			}

			m_DownFile = file;
			m_sUI.FileName.text = m_DownFile.file;
		}, (proc) =>
		{
			long size = m_SucSize + (long)(m_DownFile.size * proc);
			m_sUI.Gauge.fillAmount = (float)((double)size / (double)m_TotalSize);
			m_sUI.DownSize.text = string.Format("({0} / {1})", UTILE.Get_FileSize(size), UTILE.Get_FileSize(m_TotalSize));
		}, (error) => 
		{
			m_ErrorFiles.Add(m_DownFile);
			m_DownFile = null;
			WEB.StartStateMsg(error);
		}, ()=>
		{
			Close(m_ErrorFiles.Count());
		});
	}
}
