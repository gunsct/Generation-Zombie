using Coffee.UIEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Intro : PopupBase
{
	[Serializable]
	public enum State
	{
		Loading = 0,
		Play,
		End
	}

#pragma warning disable 0649
#pragma warning disable 0414
	[SerializeField] VideoPlayer VPlayer;
	[SerializeField] RawImage Img;
	[SerializeField] string FileName;
	State PlayerState = State.Loading;

#pragma warning restore 0414
#pragma warning restore 0649

	private void Awake() {
		VPlayer.prepareCompleted += Prepared;
		VPlayer.loopPointReached += EndReached;
		VPlayer.errorReceived += ErrorEventHandler;
		DLGTINFO.f_VideoVolume += Volume;
		Volume(Convert.ToBoolean(PlayerPrefs.GetInt("BGSND_MUTE", 0)) ? 0f : 1f);
	}
	private void OnDestroy() {
		if (MainMng.IsValid() && DLGTINFO != null) DLGTINFO.f_VideoVolume -= Volume;
	}
	void Volume(float _amount) {
		VPlayer.SetDirectAudioVolume(0, _amount);
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		base.SetData(pos, popup, cb, aobjValue);
		if (VPlayer == null) VPlayer = GetComponent<VideoPlayer>();
		StartCoroutine(ReadyToPlay());
	}

	// 동영상 준비하기.
	IEnumerator ReadyToPlay()
	{
		if (VPlayer.isPlaying) VPlayer.Stop();
		PlayerState = State.Loading;
		Img.enabled = false;
		yield return new WaitForEndOfFrame();


//#if UNITY_EDITOR
//		m_SUI.VPlayer.url = $"{Application.dataPath}/StreamingAssets/Videos/{_name}.mp4";
//#elif UNITY_ANDROID
//		m_SUI.VPlayer.url = $"jar:file://{Application.dataPath}!/assets/Videos/{_name}.mp4";
//#elif UNITY_IOS || UNITY_IPHONE
//		m_SUI.VPlayer.url = $"jar:file://{Application.dataPath}!/assets/Videos/{_name}.mp4";
//#endif
		VPlayer.url = $"{Application.streamingAssetsPath}/Videos/{FileName}.mp4";
//#if UNITY_EDITOR
//		VPlayer.url = $"{Application.dataPath}/0_PatchData/Videos/{FileName}.mp4";
//#else
//		VPlayer.url = $"{UTILE.Get_FilePath()}/Videos/{FileName}.mp4";
//#endif
		VPlayer.Prepare();
//#if USE_STREAMING_VIDEO
//#	if UNITY_EDITOR
//		VPlayer.url = $"{Application.dataPath}/Videos/{FileName}.mp4";
//#	elif NOT_USE_NET
//		Close();
//		yield break;
//#	else
//		VPlayer.url = $"{WEB.GetConfig(EServerConfig.Vedeo_url)}{FileName}.mp4";
//#	endif
//		VPlayer.Prepare();
//#else
//		var clip = UTILE.LoadVideo(FileName);
//		yield return clip;

//		RectTransform rtfMain = (RectTransform)transform;
//		RectTransform rtf = (RectTransform)VPlayer.transform;
//		VPlayer.clip = clip;
//		Img.texture = VPlayer.targetTexture = new RenderTexture((int)VPlayer.clip.width, (int)VPlayer.clip.height, 0, RenderTextureFormat.ARGB32);
//		rtf.sizeDelta = new Vector2(VPlayer.clip.width, VPlayer.clip.height);
//		float scale = Mathf.Min((float)rtfMain.rect.width / (float)VPlayer.clip.width, (float)rtfMain.rect.height / (float)VPlayer.clip.height);
//		rtf.localScale = new Vector3(scale, scale, 1f);
//		VPlayer.Prepare();
//#endif
	}

	void Prepared(VideoPlayer source) {
		SND.AllMute(true);

		RectTransform rtfMain = (RectTransform)transform;
		RectTransform rtf = (RectTransform)VPlayer.transform;
		Img.texture = VPlayer.targetTexture = new RenderTexture((int)VPlayer.width, (int)VPlayer.height, 0, RenderTextureFormat.ARGB32);
		rtf.sizeDelta = new Vector2(VPlayer.width, VPlayer.height);
		float scale = Mathf.Min((float)rtfMain.rect.width / (float)VPlayer.width, (float)rtfMain.rect.height / (float)VPlayer.height);
		rtf.localScale = new Vector3(scale, scale, 1f);

		Img.enabled = true;
		VPlayer.Play();
		PlayerState = State.Play;
	}
	void EndReached(VideoPlayer source) {
		SND.AllMute(false);
		PlayerState = State.End;
		Close();
	}

	void ErrorEventHandler(VideoPlayer source, string message)
	{
		SND.AllMute(false);
		PlayerState = State.End;
		Close();
	}

	public override void Close(int Result = 0) {
		SND.AllMute(false);
		base.Close(Result);
	}
}
/*
using Coffee.UIEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Intro : PopupBase
{
	[Serializable]
	public enum State
	{
		Loading = 0,
		Play,
		End
	}

#pragma warning disable 0649
#pragma warning disable 0414
	[SerializeField] VideoPlayer VPlayer;
	[SerializeField] RawImage Img;
	[SerializeField] string FileName;
	State PlayerState = State.Loading;

#pragma warning restore 0414
#pragma warning restore 0649

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		base.SetData(pos, popup, cb, aobjValue);
		if (VPlayer == null) VPlayer = GetComponent<VideoPlayer>();
		StartCoroutine(ReadyToPlay());
	}

	// 동영상 준비하기.
	IEnumerator ReadyToPlay()
	{
		PlayerState = State.Loading;
		Img.enabled = false;
		var clip = Resources.LoadAsync<VideoClip>(FileName);
		yield return clip;

		RectTransform rtfMain = (RectTransform)transform;
		RectTransform rtf = (RectTransform)VPlayer.transform;
		VPlayer.clip = (VideoClip)clip.asset;
		rtf.sizeDelta = new Vector2(VPlayer.clip.width, VPlayer.clip.height);
		float scale = Mathf.Min((float)rtfMain.rect.width / (float)VPlayer.clip.width, (float)rtfMain.rect.height / (float)VPlayer.clip.height);
		rtf.localScale = new Vector3(scale, scale, 1f);
		VPlayer.Prepare();

		while (!VPlayer.isPrepared)
		{
			yield return null;
		}
		Img.enabled = true;
		VPlayer.Play();
		VPlayer.loopPointReached += EndReached;
		PlayerState = State.Play;
	}

	void EndReached(VideoPlayer source)
	{
		PlayerState = State.End;
		Close();
	}
}
 */