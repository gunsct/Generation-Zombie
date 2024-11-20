using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Msg_WebView : MsgBoxBase
{
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public TextMeshProUGUI Title;
		public WebViewObject m_Webview;
		public GameObject m_LoadingPanel;
	}

	[SerializeField] SUI m_sUI;
#pragma warning restore 0649
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		base.SetData(pos, popup, cb, aobjValue);
	}

	public override void SetMsg(string Title, string Msg)
	{
		base.SetMsg(Title, Msg);
		m_sUI.Title.text = Title;
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID)
		SetWebView(Msg);
#else
		m_sUI.m_Webview.gameObject.SetActive(false);

		m_sUI.m_LoadingPanel.SetActive(true);
		m_sUI.m_LoadingPanel.GetComponent<TextMeshProUGUI>().text = "모바일 환경에서만 확인 가능!!";
#endif
	}

	void SetWebView(string url)
	{
		int wkContentMode = 0;  // 0: recommended, 1: mobile, 2: desktop
		//Caching.ClearCache();
		//// 캐시 데이터 제거
		//m_sUI.m_Webview.ClearCache(true);
		//// 쿠키 제거
		//m_sUI.m_Webview.ClearCookies();
		
		m_sUI.m_Webview.Init(cb:CallFromJS
			, err : Error
			, httpErr : HttpError
			, ld : ID
			, started : Started
#if !UNITY_IOS || UNITY_IPHONE
			, enableWKWebView: true
#endif
			, wkContentMode: wkContentMode);


		RectTransform rectView = (RectTransform)m_sUI.m_Webview.transform;
		float scale = Mathf.Min((float)Screen.width / Canvas_Controller.BASE_SCREEN_WIDTH, (float)Screen.height / Canvas_Controller.BASE_SCREEN_HEIGHT);
		Rect rect = new Rect(rectView.rect.x * scale, rectView.rect.y * scale, rectView.rect.width * scale, rectView.rect.height * scale);
		float fHH = Screen.height * 0.5f;
		Vector2 v2Dis = UTILE.GetMoveFromCanvas(m_sUI.m_Webview.transform) * scale;// Canvas에서 View까지의 Y이동 거리
		float fHW = Screen.width * 0.5f;
		int top = (int)(fHH + rect.y - v2Dis.y);
		int bottom = (int)(Screen.height - (top + rect.height));
		int left = (int)(fHW + rect.x - v2Dis.x);
		int right = (int)(Screen.width - (left + rect.width));

		m_sUI.m_Webview.SetMargins(left, top, right, bottom);

		m_sUI.m_Webview.LoadURL(url);
	}

	public override void Close(int Result = 0)
	{
		m_sUI.m_Webview.SetVisibility(false);
		base.Close(Result);
	}

	/// <summary> 서버에서 전송해주는 메세지 </summary>
	void CallFromJS(string msg)
	{
		Utile_Class.DebugLog(string.Format("WebView CallFromJS[{0}]", msg));
	}

	/// <summary> 에러 </summary>
	void Error(string msg)
	{
		Utile_Class.DebugLog(string.Format("WebView Error[{0}]", msg));
	}

	/// <summary> Http에러 </summary>
	void HttpError(string msg)
	{
		Utile_Class.DebugLog(string.Format("WebView HttpError[{0}]", msg));
	}

	/// <summary> 수신 완료 </summary>
	void ID(string msg)
	{
		m_sUI.m_LoadingPanel.SetActive(false);
		m_sUI.m_Webview.SetVisibility(true);
		Utile_Class.DebugLog(string.Format("WebView ID[{0}]", msg));
		if(msg.IndexOf("intent://") > -1)
		{
			Close(0);
			UTILE.OpenURL(WEB.GetConfig(EServerConfig.Notice_URL));
		}
	}

	/// <summary> 시작 </summary>
	void Started(string msg)
	{
		m_sUI.m_LoadingPanel.SetActive(true);
		//m_sUI.m_Webview.SetVisibility(false);
		Utile_Class.DebugLog(string.Format("WebView Started[{0}]", msg));
	}

	/// <summary> ???? </summary>
	void Hooked(string msg)
	{
		//m_sUI.m_LoadingPanel.SetActive(true);
		//m_sUI.m_Webview.SetVisibility(false);
		Utile_Class.DebugLog(string.Format("WebView hooked[{0}]", msg));
	}
}
