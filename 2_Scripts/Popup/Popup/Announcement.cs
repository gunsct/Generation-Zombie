using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Announcement : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public WebViewObject m_Webview;
		public GameObject m_LoadingPanel;
	}
	[SerializeField] SUI m_SUI;
	PopupBase m_Buffering;
	string url;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		url = (string)aobjValue[0];

#if !UNITY_EDITOR && (UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID)
		SetWebView(url);
#else
		m_SUI.m_LoadingPanel.SetActive(true);
		m_SUI.m_LoadingPanel.GetComponent<TextMeshProUGUI>().text = "모바일 환경에서만 확인 가능!!";
#endif
	}

	void SetWebView(string url) {
		int wkContentMode = 0;  // 0: recommended, 1: mobile, 2: desktop

		m_SUI.m_Webview.Init(cb: CallFromJS
			, err: Error
			, httpErr: HttpError
			, ld: ID
			, started: Started
#if !UNITY_IOS || UNITY_IPHONE
			, enableWKWebView: true
#endif
			, wkContentMode: wkContentMode);

		m_SUI.m_Webview.LoadURL(url);
		//webViewObject.SetMargins(50, 50, 50, 50);
		RectTransform rectView = (RectTransform)m_SUI.m_Webview.transform;
		float scale = Mathf.Min((float)Screen.width / Canvas_Controller.BASE_SCREEN_WIDTH, (float)Screen.height / Canvas_Controller.BASE_SCREEN_HEIGHT);
		Rect rect = new Rect(rectView.rect.x * scale, rectView.rect.y * scale, rectView.rect.width * scale, rectView.rect.height * scale);
		float fHH = Screen.height * 0.5f;
		Vector2 v2Dis = UTILE.GetMoveFromCanvas(m_SUI.m_Webview.transform) * scale;// Canvas에서 View까지의 Y이동 거리
		float fHW = Screen.width * 0.5f;
		int top = (int)(fHH + rect.y - v2Dis.y);
		int bottom = (int)(Screen.height - (top + rect.height));
		int left = (int)(fHW + rect.x - v2Dis.x);
		int right = (int)(Screen.width - (left + rect.width));

		m_SUI.m_Webview.SetMargins(left, top, right, bottom);
	}

	public override void Close(int Result = 0)
	{
		m_SUI.m_Webview.SetVisibility(false);
		base.Close(Result);
	}

	/// <summary> 서버에서 전송해주는 메세지 </summary>
	void CallFromJS(string msg) {
		Utile_Class.DebugLog(string.Format("WebView CallFromJS[{0}]", msg));
	}

	/// <summary> 에러 </summary>
	void Error(string msg) {
		Utile_Class.DebugLog(string.Format("WebView Error[{0}]", msg));
	}

	/// <summary> Http에러 </summary>
	void HttpError(string msg) {
		Utile_Class.DebugLog(string.Format("WebView HttpError[{0}]", msg));
	}

	/// <summary> 수신 완료 </summary>
	void ID(string msg) {
		m_SUI.m_LoadingPanel.SetActive(false);
		m_SUI.m_Webview.SetVisibility(true);
		Utile_Class.DebugLog(string.Format("WebView ID[{0}]", msg));
		if (msg.IndexOf("intent://") > -1)
		{
			Close(0);
			UTILE.OpenURL(WEB.GetConfig(EServerConfig.Notice_URL));
		}
	}

	/// <summary> 시작 </summary>
	void Started(string msg) {
		m_SUI.m_LoadingPanel.SetActive(true);
		//m_SUI.m_Webview.SetVisibility(false);
		Utile_Class.DebugLog(string.Format("WebView Started[{0}]", msg));
	}
}
