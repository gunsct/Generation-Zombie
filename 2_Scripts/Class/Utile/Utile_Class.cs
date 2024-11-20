using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CameraViewSizeInfo
{
	public Vector3[] origin = new Vector3[3];
	public Vector3[] direction = new Vector3[3];
}

public partial class Utile_Class
{
	private static string m_strPath;

	private string m_strPrefabName;
	private string m_strImageName;
	private string m_strSoundName;
	private string m_strVideoName;
	private const string m_strAssetBasePath = "Assets/0_PatchData/";

	private string m_strPassBundleName;

	private Font m_SystemFont = null;
	public Utile_Class()
	{
		//UnityEngine.Random.seed = (int)System.DateTime.Now.Ticks;
		UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
		//Get_Sync_DateTime(Get_RunTime_Seconds(m_dtUnixEpoch));
#if UNITY_EDITOR
		m_strPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
		m_strPath += "/files";
		DirectoryInfo di = new DirectoryInfo(m_strPath);
		if (di.Exists == false) di.Create();
		m_strPath += "/";
#else
		switch (Application.platform)
		{
		case RuntimePlatform.Android:
		case RuntimePlatform.IPhonePlayer:
			m_strPath = Application.persistentDataPath + "/";
			break;
		default:
			m_strPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
			m_strPath += "/files";
			DirectoryInfo di = new DirectoryInfo(m_strPath);
			if (di.Exists == false) di.Create();
			m_strPath += "/";
			break;
		}
#endif
#if ASSET_BUNDLE_TEST
		m_strPrefabName = Get_FileName_Encode("Prefabs");
		m_strImageName = Get_FileName_Encode("Imgs");
#else
		m_strPrefabName = Get_FileName_Encode("AddRes");
		m_strImageName = Get_FileName_Encode("Imgs");
#endif
		m_strSoundName = Get_FileName_Encode("Snds");
		m_strVideoName = Get_FileName_Encode("Videos");
		//LoadAtlas();
	}
	/////////////////////////////////////////////////////////////////////////////
	/// System
	// 최대 프레임 조절
	public static void SetMaxFrame(int vsync, int nFrame)
	{
		// 수직동기화 Off
		QualitySettings.vSyncCount = vsync;
		Application.targetFrameRate = nFrame;
	}

	public static long GetUniqeID()
	{
		long id = long.Parse(PlayerPrefs.GetString("UniqeNo", "0")) + 1;
		PlayerPrefs.SetString("UniqeNo", id.ToString());
		PlayerPrefs.Save();
		return id;
	}

	/// <summary> 0, 10 -> 0~9</summary>
	public int Get_Random(int nMin, int nMax)
	{
		return Get_RandomStatic(nMin, nMax);
	}

	public float Get_Random(float fMin, float fMax)
	{
		return Get_RandomStatic(fMin, fMax);
	}

	public Vector2 Get_RandomVector2(float fMin, float fMax)
	{
		return Get_RandomVector2Static(fMin, fMax);
	}

	public Vector3 Get_RandomVector3(float fMin, float fMax)
	{
		return Get_RandomVector3Static(fMin, fMax);
	}

	public T Get_Random<T>(T nMin, T nMax) where T : System.Enum
	{
		int min = Convert.ToInt32(nMin);
		int max = Convert.ToInt32(nMax);
		return (T)(object)Get_Random(min, max);
	}

	public static int Get_RandomStatic(int nMin, int nMax)
	{
		return UnityEngine.Random.Range(nMin, nMax);
	}

	public static float Get_RandomStatic(float fMin, float fMax)
	{
		return UnityEngine.Random.Range(fMin, fMax);
	}

	public static Vector3 Get_RandomVector3Static(float fMin, float fMax)
	{
		return new Vector3(Get_RandomStatic(fMin, fMax), Get_RandomStatic(fMin, fMax), Get_RandomStatic(fMin, fMax));
	}
	public static Vector2 Get_RandomVector2Static(float fMin, float fMax)
	{
		return new Vector2(Get_RandomStatic(fMin, fMax), Get_RandomStatic(fMin, fMax));
	}
	public static void Shuffle<T>(T[] array)
	{
		int random1;
		int random2;

		T tmp;

		for (int index = 0; index < array.Length; ++index)
		{
			random1 = UnityEngine.Random.Range(0, array.Length);
			random2 = UnityEngine.Random.Range(0, array.Length);

			tmp = array[random1];
			array[random1] = array[random2];
			array[random2] = tmp;
		}
	}

	public static void Shuffle<T>(List<T> list)
	{
		int random1;
		int random2;

		T tmp;

		for (int index = 0; index < list.Count; ++index)
		{
			random1 = UnityEngine.Random.Range(0, list.Count);
			random2 = UnityEngine.Random.Range(0, list.Count);

			tmp = list[random1];
			list[random1] = list[random2];
			list[random2] = tmp;
		}
	}

	public static float GetLimitValue(float fValue, float fMin, float fMax)
	{
		return Mathf.Max(fMin, Mathf.Min(fMax, fValue));
	}

	public void Send_Email(string email, string title, string message)
	{
		//string email = "me@example.com";
		//string subject = MyEscapeURL("My Subject");
		//string body = MyEscapeURL("My Body\r\nFull of non-escaped chars");
		string url = string.Format("mailto:{0}?subject={1}&body={2}", email, EscapeURL(title), EscapeURL(message));
		Application.OpenURL(url);
	}

	public void OpenURL(string URL)
	{
		Application.OpenURL(URL);
	}


	public void OpenReview(string URL)
	{
#if UNITY_ANDROID
		Application.OpenURL("market://details?id=" + Application.identifier);
#elif UNITY_IOS || UNITY_IPHONE
        UnityEngine.iOS.Device.RequestStoreReview();
#endif
	}

	private string EscapeURL(string url)
	{
		return UnityWebRequest.EscapeURL(url).Replace("+", "%20");
	}

	/////////////////////////////////////////////////////////////////////////////
	/// 오브젝트
	public static Vector3 GetWorldPosition(Vector2 v2ScrPostion, float WoroldObjZ = 0f)
	{
		WoroldObjZ = Mathf.Abs(Camera.main.transform.position.z - WoroldObjZ);//Camera.main.farClipPlane;
		return Camera.main.ScreenToWorldPoint(new Vector3(v2ScrPostion.x, v2ScrPostion.y, WoroldObjZ));
	}

	public static Vector3 GetCanvasPosition(Vector3 v3WorldPostion)
	{
		return Camera.main.WorldToScreenPoint(v3WorldPostion);
	}
	public static Vector3 MoveTowords(Vector3 v3Now, Vector3 v3Start, Vector3 v3End, float fTime, float fMaxTime)
	{
		return MoveTowords(v3Now, v3End, Vector3.Distance(v3Start, v3End), fTime, fMaxTime);
	}

	public static Vector3 MoveTowords(Vector3 v3Now, Vector3 v3End, float fTotlaDis, float fTime, float fMaxTime)
	{
		return Vector3.MoveTowards(v3Now, v3End, Mathf.Abs(fTotlaDis) / fMaxTime * fTime);
	}

	public Vector2 GetMoveFromCanvas(Transform check)
	{
		Vector2 v2Pos = check.localPosition;
		if (check.GetComponent<Canvas>() != null) return Vector3.zero;
		else if (check.transform.parent != null) v2Pos += GetMoveFromCanvas(check.parent);
		return v2Pos;
	}
	public static bool IsAniPlay(Animator ani, float _nomal = 1.0f, int _layeridx = 0)
	{
		if (!ani) return false;
		if (ani.IsInTransition(_layeridx)) return true;
		AnimatorStateInfo info = ani.GetCurrentAnimatorStateInfo(_layeridx);
		if (info.loop) return false;
		return info.normalizedTime < _nomal;
	}

	public static IEnumerator CheckAniPlay(Animator ani)
	{
		if (ani == null) yield break;
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => IsAniPlay(ani));
	}
	public static IEnumerator CheckAni(Animator _ani, float _normal, Action _cb) {
		if (_ani == null) yield break;
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => IsAniPlay(_ani, _normal));
		_cb?.Invoke();
	}
	public static void AniSkip(Animator ani, float? PlayFrame = null)
	{
		AnimatorStateInfo info = ani.GetCurrentAnimatorStateInfo(0);
		float offset = Mathf.Clamp(PlayFrame == null ? 1f : PlayFrame.Value / 60f / info.length, 0, 1f);
		if (offset <= info.normalizedTime) return;
		// 해당 시간부터 시작하기
		ani.Play(0, 0, offset);
	}
	public static void AniResetAllTriggers(Animator ani)
	{
		if (ani == null) return;
		for (int i = 0; i < ani.parameters.Length; i++)
		{
			var param = ani.parameters[i];
			if (param.type == AnimatorControllerParameterType.Trigger) ani.ResetTrigger(param.name);
		}
	}

	public static bool IsPlayiTween(GameObject obj, string _name = "")
	{
		if (_name == "")
			return obj.GetComponent<iTween>() != null;
		else
			return obj.GetComponent<iTween>() != null && obj.GetComponent<iTween>()._name != null && obj.GetComponent<iTween>()._name.Equals(_name);
	}

	public static CameraViewSizeInfo GetViewWorldSizeInfo(Camera cam = null, float? width = null, float? height = null)
	{
		if (cam == null) cam = Camera.main;
		if (width == null) width = Screen.width;
		if (height == null) height = Screen.height;
		CameraViewSizeInfo info = new CameraViewSizeInfo();
		Ray[] arayViewPort = new Ray[3];
		arayViewPort[0] = cam.ScreenPointToRay(Vector3.zero);
		arayViewPort[1] = cam.ScreenPointToRay(new Vector2(width.Value, height.Value));
		arayViewPort[2] = cam.ScreenPointToRay(new Vector2(width.Value, height.Value) * 0.5f);
		for (int i = 0; i < 3; i++)
		{
			info.origin[i] = arayViewPort[i].origin;
			info.direction[i] = arayViewPort[i].direction;
		}
		return info;
	}

	/// <summary> TextMeshPro용 알파 셋팅하기
	/// <para> LateUpdate 에서만 사용할것 </para>
	/// </summary>
	public static void TextMeshProAlphaChange(TextMeshPro text, float alpha, bool reset = true)
	{
		if (!text.enabled || !text.gameObject.activeSelf) return;
		//List<float> alphas = new List<float>();
		//for (int i = 0; i < info.meshInfo.Length; i++)
		//{
		//	TMP_MeshInfo mesh = info.meshInfo[i];
		//	if (mesh.colors32 == null) continue;
		//	for (int j = 0; j < mesh.colors32.Length; j++)
		//	{
		//		Color color = mesh.colors32[j];
		//		//alphas.Add(color.a);
		//		color.a = alpha;
		//		mesh.colors32[j] = color;
		//	}
		//}
		Color color = text.color;
		color.a = alpha;
		text.color = color;

		//TMP_TextInfo txtinfo = text.textInfo;
		//int characterCount = txtinfo.characterCount;
		//for (int k = 0; k < characterCount; k++)
		//{
		//	TMP_CharacterInfo ch = txtinfo.characterInfo[k];
		//	//if (ch.isVisible)
		//	//{
		//		int materialReferenceIndex = ch.materialReferenceIndex;
		//		int idx = ch.vertexIndex;
		//		Color32[] colors = txtinfo.meshInfo[materialReferenceIndex].colors32;
		//		for (int j = ch.vertexIndex, jMax = ch.vertexIndex + 4; j < jMax; j++)
		//		{
		//			Color color = colors[j];
		//			//alphas.Add(color.a);
		//			color.a = alpha;
		//			txtinfo.meshInfo[materialReferenceIndex].colors32[j] = color;
		//		}
		//	//}
		//}

		//text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

		//if(reset)
		//{
		//	int offset = 0;
		//	for (int i = 0; i < info.meshInfo.Length; i++)
		//	{
		//		TMP_MeshInfo mesh = info.meshInfo[i];
		//		if (mesh.colors32 == null) continue;
		//		for (int j = 0; j < mesh.colors32.Length; j++, offset++)
		//		{
		//			Color color = mesh.colors32[j];
		//			color.a = alphas[offset];
		//			mesh.colors32[j] = color;
		//		}
		//	}
		//}
	}

	/// <summary> MeshFilter용 색상 만들기
	/// </summary>
	public static void MeshColor(MeshFilter mf, Color color)
	{
		Mesh mesh = mf.mesh;
		for (int j = 0; j < mesh.colors.Length; j++) mesh.colors[j] = color;
	}

	/// <summary> MeshFilter용 알파 셋팅하기
	/// <para> LateUpdate 에서만 사용할것 </para>
	/// </summary>
	public static void MeshAlphaChange(Mesh mesh, float alpha)
	{
		if (mesh == null) return;
		for (int j = 0; j < mesh.colors.Length; j++)
		{
			Color color = mesh.colors[j];
			color.a *= alpha;
			mesh.colors[j] = color;
		}
	}

	[System.Diagnostics.Conditional("USE_LOG_MANAGER")]
	public static void DebugLog(object _log) {
		Debug.Log($"PZW!!! {_log}");
	}
	[System.Diagnostics.Conditional("STAT_VAL_DEBUG")]
	public static void DebugLog_Value(object _log) {
		Debug.Log(_log);
	}
	public Font GetSystemFont(int size = 32)
	{
		if(m_SystemFont == null)
		{
			var fonts = Font.GetOSInstalledFontNames();
			if (fonts.Length < 1) return null;
			m_SystemFont = Font.CreateDynamicFontFromOSFont(fonts[0], size);
		}
		return m_SystemFont;
	}

	/////////////////////////////////////////////////////////////////////////////
	/// 클립보드
	// 클립보드 복사
	public static void Copy_Clipboard(string strMsg)
	{
		GUIUtility.systemCopyBuffer = strMsg;
	}

	// 클립보드 내용 알아내기
	public static string Paste_Clipboard()
	{
		return GUIUtility.systemCopyBuffer;
	}

	public static void ChangeLayer(Transform _obj, int _layer) {
		_obj.gameObject.layer = _layer;
		for (int i = 0; i < _obj.childCount; i++) {
			Transform child = _obj.GetChild(i);
			child.gameObject.layer = _layer;
			ChangeLayer(child, _layer);
		}
	}
//	public static void Copy_Clipboard(string data)
//	{
//#if UNITY_EDITOR
//		GUIUtility.systemCopyBuffer = data;
//#else
//		TextEditor _textEditor = new TextEditor{ text = data };

//		_textEditor.OnFocus();
//		_textEditor.Copy();
//#endif
//	}
}
