using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class DataLoad : PopupBase
{
#pragma warning disable 0649
#pragma warning disable 0414
	[System.Serializable]
	struct SUI
	{
		public Image Gauge;
		public TextMeshProUGUI Info;
		public Transform TutoBucket;
		public List<string> PrefabNames;
	}

	[SerializeField] SUI m_sUI;
	int m_FileCnt, m_Cnt;
	long m_Total, m_Now;
	float m_LoadProc;
	Item_CDN_Tuto m_Tuto;
#pragma warning restore 0414
#pragma warning restore 0649

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		base.SetData(pos, popup, cb, aobjValue);
	}

	public void Start()
	{
		StartCoroutine(Loading());
		StartCoroutine(DotRender());
		StartCoroutine(ShowTuto());
	}

	public void Update()
	{
		m_sUI.Gauge.fillAmount = m_LoadProc;
	}

	IEnumerator Loading()
	{
		yield return UTILE.AllLoadAsset();
		m_Cnt = 0;
		m_LoadProc = 0f;
		float filegap = 0f;
		float gap = 0f;
		// 1000개의 라인마다 쉬도록 설정
		ToolFile.LINE_SLIP_CNT = 200000;
		yield return TDATA.LoadAllTablesAsync(true
		, (filecnt) => { m_FileCnt = filecnt; filegap = 1f / (float)filecnt; }
		, (path, total) => {
			m_Total = total;
			m_Now = 0;
			gap = 1f / (float)total * filegap;
		}
		, (loaded) => {
			m_Now = loaded;
			m_LoadProc += gap;
		});

#if NOT_USE_NET
		MAIN.Load_UserInfo();
#else
		bool end = false;
		WEB.SEND_REQ_ALL_INFO((res) => { end = true; }, true);
		yield return new WaitWhile(() => !end);

		end = false;
		USERINFO.m_Auction.Load(() => { end = true; }, false);
		yield return new WaitWhile(() => !end);

		// 유저 미션정보 받기
		end = false;
		WEB.SEND_REQ_MISSIONINFO((res) => { end = true; });
		yield return new WaitWhile(() => !end);

		PlayerPrefs.SetString($"Challenge_Now_{USERINFO.m_UID}", "");
		PlayerPrefs.SetString($"Challenge_Next_{USERINFO.m_UID}", "");
		PlayerPrefs.Save();
		end = false;
		WEB.SEND_REQ_CHALLENGEINFO_ALL((res) => { end = true; });
		yield return new WaitWhile(() => !end);

		end = false;
		USERINFO.m_Guild.LoadGuild(() => {
			if (USERINFO.m_Guild.UID != 0)
			{
				if (USERINFO.m_GuildKickCheck.UID == 0) USERINFO.m_Guild.Set_AlramOff();
				USERINFO.m_GuildKickCheck.Save();
			}
			end = true;
		}, 0, true, true, true);
		yield return new WaitWhile(() => !end);
		if (USERINFO.m_Guild.Master?.UserNo == USERINFO.m_UID)
		{
			end = false;
			USERINFO.m_Guild.LoadGuildReqJoin(() => { end = true; });
			yield return new WaitWhile(() => !end);
		}

		end = false;
		WEB.SEND_REQ_SHOP_INFO((res) => {
			USERINFO.SetDATA(res);
			IAP.Init();
			end = true;
		});
		yield return new WaitWhile(() => !end);

		end = false;
		USERINFO.m_Event.Load((res) => { end = true; }, false);
		yield return new WaitWhile(() => !end);

		end = false;
		HIVE.ShopInit(() => { end = true; });
		yield return new WaitWhile(() => !end);

		yield return HIVE.Restore();
#endif
		TUTO.Start(TutoStartPos.TitleEnd);
		if(TUTO.IsTutoPlay()) {
			yield return new WaitWhile(() => !TUTO.IsTuto(TutoKind.Stage_101, (int)TutoType_Stage_101.DelayStartEnd));
		}
		Close();
	}

	IEnumerator DotRender()
	{
		int dotCnt = 0;
		while (true)
		{
			if (dotCnt == 0) m_sUI.Info.text = "Loading";
			else m_sUI.Info.text += ".";
			dotCnt++;
			dotCnt %= 4;
			yield return new WaitForSeconds(0.5f);
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
			m_Tuto.SetData(Item_CDN_Tuto.State.DataLoad);
			yield return new WaitWhile(() => m_Tuto != null);
		}
	}
}
