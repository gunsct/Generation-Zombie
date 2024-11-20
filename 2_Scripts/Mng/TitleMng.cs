using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum TitleState
{
	END
}

public class TitleMng : ObjMng
{
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// Instance
	private static TitleMng m_Instance = null;
	public static TitleMng Instance
	{
		get
		{
			return m_Instance;
		}
	}

	public static bool IsValid()
	{
		return m_Instance != null;
	}

	// Start is called before the first frame update
	void Awake()
	{
		MAIN.m_UserInfo = new UserInfo();
		m_Instance = this;
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	private void Start()
	{
		//yield return UTILE.AllLoadAsset();
		Init();
	}

	public void Init()
	{
		STAGEINFO.Init();
		// 배경과 Title메인시작과 같이 플레이되어야함
		PlayBGSound(SND_IDX.BGM_0002);
		POPUP.Set_Popup(PopupPos.MAINUI, PopupName.Title);
	}

}
