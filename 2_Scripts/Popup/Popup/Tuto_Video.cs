using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System;
using UnityEngine.UI;
using TMPro;

public class Tuto_Video : PopupBase
{
	[Serializable]
    public struct SUI
	{
		public TextMeshProUGUI Title;
		public TextMeshProUGUI Desc;
		public VideoPlayer VPlayer;
		public RawImage Img;
		public GameObject NonVideoGroup;
		public GameObject[] PreNextBtns;
		public TextMeshProUGUI NextCompBtnTxt;
		public GameObject CloseBtn;
	}
	[SerializeField] SUI m_SUI;
	int m_Step;
	string[] m_FileNames;
	bool Is_End;
	TutoVideoType m_Type;
	int m_Val;
	IEnumerator m_CorPlay;

	private void Awake() {
		m_SUI.VPlayer.prepareCompleted += Prepared;
		m_SUI.VPlayer.loopPointReached += EndReached;
		m_SUI.VPlayer.errorReceived += ErrorEventHandler;
		DLGTINFO.f_VideoVolume += Volume;
		Volume(Convert.ToBoolean(PlayerPrefs.GetInt("BGSND_MUTE", 0)) ? 0f : 1f);
	}
	private void OnDestroy() {
		if (MainMng.IsValid() && DLGTINFO != null) DLGTINFO.f_VideoVolume -= Volume;
	}
	void Volume(float _amount) {
		m_SUI.VPlayer.SetDirectAudioVolume(0, _amount);
	}

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_Type = (TutoVideoType)aobjValue[0];
		switch (m_Type) {
			case TutoVideoType.NoteBattle:
				m_Val = (int)aobjValue[1];
				switch (m_Val) {//NoteBattle_Catch 는 나중에
					case 1002003: m_FileNames = new string[] { "NoteBattle_Slash" }; break;
					case 1002006: m_FileNames = new string[] { "NoteBattle_Combo" }; break;
					case 1002008: m_FileNames = new string[] { "NoteBattle_Holder" }; break;
					case 1002011: m_FileNames = new string[] { "NoteBattle_Chain" }; break;
					//1002001
					default: m_FileNames = new string[] { "NoteBattle_Normal", "NoteBattle_Guard", "NoteBattle_aviod" }; break;
				}
				break;
			case TutoVideoType.Training:
				m_FileNames = new string[] { "TrainingTuto" };
				break;
			case TutoVideoType.Tower:
				m_FileNames = new string[] { "Subway_Basic", "Subway_Battle", "Subway_SupplyBox", "Subway_Rest", "Subway_SecrectEvent", "Subway_RandomEvent", "Subway_Refugee" };
				break;
		}

		base.SetData(pos, popup, cb, aobjValue);

		CheckBtnNtxt();
		if (m_SUI.VPlayer == null) m_SUI.VPlayer = GetComponent<VideoPlayer>();
		StartCoroutine(m_CorPlay = ReadyToPlay());
	}// 동영상 준비하기.
	IEnumerator ReadyToPlay() {
		if (m_SUI.VPlayer.isPlaying) m_SUI.VPlayer.Stop();
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => !m_SUI.Img.gameObject.activeSelf);
		m_SUI.Img.enabled = false;


//#if UNITY_EDITOR
//		m_SUI.VPlayer.url = $"{Application.dataPath}/StreamingAssets/Videos/{_name}.mp4";
//#elif UNITY_ANDROID
//		m_SUI.VPlayer.url = $"jar:file://{Application.dataPath}!/assets/Videos/{_name}.mp4";
//#elif UNITY_IOS || UNITY_IPHONE
//		m_SUI.VPlayer.url = $"jar:file://{Application.dataPath}!/assets/Videos/{_name}.mp4";
//#endif
		m_SUI.VPlayer.url = $"{Application.streamingAssetsPath}/Videos/{m_FileNames[m_Step]}.mp4";

//#if UNITY_EDITOR
//		m_SUI.VPlayer.url = $"{Application.dataPath}/0_PatchData/Videos/{m_FileNames[m_Step]}.mp4";//Application.dataPath}
//#else
//		m_SUI.VPlayer.url = $"{UTILE.Get_FilePath()}/Videos/{m_FileNames[m_Step]}.mp4";
//#endif
		m_SUI.VPlayer.Prepare();
//#if USE_STREAMING_VIDEO
//#	if UNITY_EDITOR
//		m_SUI.VPlayer.url = $"{Application.dataPath}/Videos/{m_FileNames[m_Step]}.mp4";
//#	elif NOT_USE_NET
//		Close();
//		yield break;
//#	else
//		m_SUI.VPlayer.url = $"{WEB.GetConfig(EServerConfig.Vedeo_url)}{m_FileNames[m_Step]}.mp4";
//#	endif
//		m_SUI.VPlayer.Prepare();
//#else
//		var clip = UTILE.LoadVideo(m_FileNames[m_Step]);
//		yield return clip;

//		RectTransform rtfMain = (RectTransform)transform;
//		RectTransform rtf = (RectTransform)m_SUI.VPlayer.transform;
//		m_SUI.VPlayer.clip = clip;//.asset
//		m_SUI.Img.texture = m_SUI.VPlayer.targetTexture = new RenderTexture((int)m_SUI.VPlayer.clip.width, (int)m_SUI.VPlayer.clip.height, 0, RenderTextureFormat.ARGB32);
//		rtf.sizeDelta = new Vector2(m_SUI.VPlayer.clip.width, m_SUI.VPlayer.clip.height);
//		//float scale = Mathf.Min((float)rtfMain.rect.width / (float)m_SUI.VPlayer.clip.width, (float)rtfMain.rect.height / (float)m_SUI.VPlayer.clip.height);
//		//rtf.localScale = new Vector3(scale, scale, 1f);
//		m_SUI.VPlayer.Prepare();
//#endif
	}

	void Prepared(VideoPlayer source) {
		SND.AllMute(true);

		RectTransform rtfMain = (RectTransform)transform;
		RectTransform rtf = (RectTransform)m_SUI.VPlayer.transform;
		m_SUI.Img.texture = m_SUI.VPlayer.targetTexture = new RenderTexture((int)m_SUI.VPlayer.width, (int)m_SUI.VPlayer.height, 0, RenderTextureFormat.ARGB32);
		rtf.sizeDelta = new Vector2(m_SUI.VPlayer.width, m_SUI.VPlayer.height);
		float scale = Mathf.Min((float)rtfMain.rect.width / (float)m_SUI.VPlayer.width, (float)rtfMain.rect.height / (float)m_SUI.VPlayer.height);
		rtf.localScale = new Vector3(scale, scale, 1f);

		m_SUI.Img.enabled = true;
		m_SUI.VPlayer.Play();
	}
	void EndReached(VideoPlayer source) {
		SND.AllMute(false);
		StartCoroutine(m_CorPlay = ReadyToPlay());
	}
	void ErrorEventHandler(VideoPlayer source, string message)
	{
		SND.AllMute(false);
		StartCoroutine(m_CorPlay = ReadyToPlay());
	}
	void CheckBtnNtxt() {
		m_SUI.PreNextBtns[0].SetActive(m_Step > 0);
		//m_SUI.PreNextBtns[1].SetActive(m_Step < m_FileNames.Length - 1);
		m_SUI.NextCompBtnTxt.text = TDATA.GetString(m_Step < m_FileNames.Length - 1 ? 526 : 199);
		m_SUI.CloseBtn.transform.localPosition = new Vector3(!m_SUI.PreNextBtns[0].activeSelf && !m_SUI.PreNextBtns[1].activeSelf ? 0f : -488f, m_SUI.CloseBtn.transform.localPosition.y, m_SUI.CloseBtn.transform.localPosition.z);
		if (!Is_End && m_Step == m_FileNames.Length - 1) Is_End = true;
		m_SUI.CloseBtn.SetActive(Is_End);

		string title = string.Empty, desc = string.Empty;
		switch (m_Type) {
			case TutoVideoType.NoteBattle:
				switch (m_Step) {
					case 0:
						switch (m_Val) {
							case 1002001:
								title = TDATA.GetString(542);
								desc = TDATA.GetString(543);
								break;
							case 1002003:
								title = TDATA.GetString(548);
								desc = TDATA.GetString(549);
								break;
							case 1002006:
								title = TDATA.GetString(544);
								desc = TDATA.GetString(545);
								break;
							case 1002008:
								title = TDATA.GetString(546);
								desc = TDATA.GetString(547);
								break;
							case 1002011:
								title = TDATA.GetString(550);
								desc = TDATA.GetString(551);
								break;
								//체인
								//title = TDATA.GetString(1901);
								//desc = TDATA.GetString(1902);
						}
						break;
					case 1:
						title = TDATA.GetString(1903);
						desc = TDATA.GetString(1904);
						break;
					case 2:
						title = TDATA.GetString(1905);
						desc = TDATA.GetString(1906);
						break;
				}
				break;
			case TutoVideoType.Training:
				switch (m_Step) {
					case 0:
						title = TDATA.GetString(1925);
						desc = TDATA.GetString(1926);
						break;
				}
				break;
			case TutoVideoType.Tower:
				switch (m_Step) {
					case 0:
						title = TDATA.GetString(1911);
						desc = TDATA.GetString(1912);
						break;
					case 1:
						title = TDATA.GetString(1913);
						desc = TDATA.GetString(1914);
						break;
					case 2:
						title = TDATA.GetString(1915);
						desc = TDATA.GetString(1916);
						break;
					case 3:
						title = TDATA.GetString(1917);
						desc = TDATA.GetString(1918);
						break;
					case 4:
						title = TDATA.GetString(1919);
						desc = TDATA.GetString(1920);
						break;
					case 5:
						title = TDATA.GetString(1921);
						desc = TDATA.GetString(1922);
						break;
					case 6:
						title = TDATA.GetString(1923);
						desc = TDATA.GetString(1924);
						break;
				}
				break;
		}
		m_SUI.Title.text = title;
		m_SUI.Desc.text = desc;
	}
	public void Click_PreNext(bool _pre) {
		if (!_pre && m_Step == m_FileNames.Length - 1) {
			Close();
			return;
		}
		m_Step += _pre ? -1 : 1; 
		CheckBtnNtxt();
		if (m_CorPlay != null) StopCoroutine(m_CorPlay);
		StartCoroutine(m_CorPlay = ReadyToPlay());
	}
	public override void Close(int Result = 0) {
		SND.AllMute(false);
		base.Close(Result);
	}
}
