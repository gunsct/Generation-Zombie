using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Video;
using UnityEngine.UI;

public class COMPnPDSignature : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public VideoPlayer VPlayer;
		public RawImage Img;
		public string[] FileNames;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_CorPlay;

	int m_Step = 0;
	private void Awake() {
		if (m_SUI.VPlayer == null) m_SUI.VPlayer = GetComponent<VideoPlayer>();
		m_SUI.Img.enabled = false;
		m_SUI.VPlayer.prepareCompleted += Prepared;
		m_SUI.VPlayer.loopPointReached += EndReached;
		DLGTINFO.f_VideoVolume += Volume;
		Volume(Convert.ToBoolean(PlayerPrefs.GetInt("BGSND_MUTE", 0)) ? 0f : 1f);
	}
	private void OnDestroy() {
		if (MainMng.IsValid()) DLGTINFO.f_VideoVolume -= Volume;
	}
	void Volume(float _amount) {
		m_SUI.VPlayer.SetDirectAudioVolume(0, _amount);
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		if (m_SUI.VPlayer == null) m_SUI.VPlayer = GetComponent<VideoPlayer>();
		m_CorPlay = ReadyToPlay(m_SUI.FileNames[m_Step]);
		StartCoroutine(m_CorPlay);
	}

	IEnumerator ReadyToPlay(string _name) {
		if (m_SUI.VPlayer.isPlaying) m_SUI.VPlayer.Stop();
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => !m_SUI.Img.gameObject.activeSelf);
		//string filePath = $"jar:file://{Application.dataPath}!/assets/Videos/{_name}.mp4";
		//string filePath = streamingAssetsDirectory + filename;
		m_SUI.Img.enabled = false;
		//var clip = UTILE.LoadVideo(_name);
		//yield return clip;

		//m_SUI.VPlayer.clip = clip;//.asset
//#if UNITY_EDITOR
//		m_SUI.VPlayer.url = $"{Application.dataPath}/StreamingAssets/Videos/{_name}.mp4";
//#elif UNITY_ANDROID
//		m_SUI.VPlayer.url = $"jar:file://{Application.dataPath}!/assets/Videos/{_name}.mp4";
//#elif UNITY_IOS || UNITY_IPHONE
//		m_SUI.VPlayer.url = $"jar:file://{Application.dataPath}!/assets/Videos/{_name}.mp4";
//#endif
		m_SUI.VPlayer.url = $"{Application.streamingAssetsPath}/Videos/{_name}.mp4";
		//m_SUI.VPlayer.isLooping = false;
		m_SUI.VPlayer.Prepare();
	}

	void Prepared(VideoPlayer source) {
		SND.AllMute(true);

		RectTransform rtfMain = (RectTransform)transform;
		RectTransform rtf = (RectTransform)m_SUI.VPlayer.transform;
		m_SUI.Img.texture = m_SUI.VPlayer.targetTexture = new RenderTexture((int)m_SUI.VPlayer.width, (int)m_SUI.VPlayer.height, 0, RenderTextureFormat.ARGB32);
		rtf.sizeDelta = new Vector2(m_SUI.VPlayer.width, m_SUI.VPlayer.height);
		float scale = Mathf.Min((float)rtfMain.rect.width / (float)m_SUI.VPlayer.width, (float)rtfMain.rect.height / (float)m_SUI.VPlayer.height);
		rtf.localScale = new Vector3(scale, scale, 1f);

		m_SUI.VPlayer.Play();
		m_SUI.Img.enabled = true;
	}
	void EndReached(VideoPlayer source) {
		SND.AllMute(false);
		m_Step++;
		if (m_Step < m_SUI.FileNames.Length) StartCoroutine(m_CorPlay = ReadyToPlay(m_SUI.FileNames[m_Step]));
		else {
			Close(1);
		}
	}
	public override void Close(int Result = 0) {
		if (!MAIN.Is_LoadServerConfig && Result == 0) return;
		SND.AllMute(false);
		base.Close(Result);
	}
}
