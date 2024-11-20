using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.U2D;
using UnityEngine.Video;

public partial class Utile_Class
{
	public string Get_FilePath()
	{
		return m_strPath;
	}

	public byte[] GetBytes(string strIn)
	{
		return Encoding.UTF8.GetBytes(strIn);
	}

	public static string GetFileName(string pathinfo)
	{
		return Path.GetFileNameWithoutExtension(pathinfo);
	}

	public bool ISFile(string strDir, string strFileName)
	{
		StringBuilder txt = new StringBuilder(m_strPath);
		if (!string.IsNullOrEmpty(strDir)) txt.Append(strDir).Append("/");
		txt.Append(strFileName);
		return File.Exists(txt.ToString());
	}
	public bool CheckFile(string strFileName, string strExtension, bool bEncodingFileName = true)
	{
		if (bEncodingFileName)
		{
			string name = Get_FileName_Encode(Path.GetFileName(strFileName));
			string dir = Path.GetDirectoryName(strFileName);
			StringBuilder builder = new StringBuilder(512);
			builder.Append(dir);
			builder.Append("/");
			builder.Append(name);
			strFileName = builder.ToString();
		}
		StringBuilder txt = new StringBuilder(m_strPath);
		txt.Append(strFileName);
		txt.Append(strExtension);
		string fullpath = txt.ToString();
		return File.Exists(fullpath);
	}

	public long GetFileSize(string strFileName, string strExtension, bool bEncodingFileName = true)
	{
		try
		{
			if (bEncodingFileName)
			{
				string name = Get_FileName_Encode(Path.GetFileName(strFileName));
				string dir = Path.GetDirectoryName(strFileName);
				StringBuilder builder = new StringBuilder(512);
				builder.Append(dir);
				builder.Append("/");
				builder.Append(name);
				strFileName = builder.ToString();
			}
			StringBuilder txt;
			txt = new StringBuilder(m_strPath);
			txt.Append(strFileName);
			txt.Append(strExtension);
			string fullpath = txt.ToString();

			if (File.Exists(fullpath))
			{
				return (new FileInfo(fullpath)).Length;
			}
			else
			{
				TextAsset txtFile = Resources.Load(strFileName) as TextAsset;
				if (txtFile != null) return txtFile.bytes.Length;
			}
		}
		catch (Exception e)
		{
			UnityEngine.Debug.LogError("GetFileSize Exception: " + e.ToString());
		}
		return 0;
	}
	public string GetData_String(string strFileName, string strExtension)
	{
		string re = "";
		byte[] abyTemp = MainMng.Instance.UTILE.GetData(strFileName, strExtension, true, true);
		if(abyTemp != null)
		{
			// 복호화
			Utile_Class.Decode(abyTemp, abyTemp.Length, 0);
			re = Encoding.UTF8.GetString(abyTemp);
		}
		return re;
	}

	public byte[] GetData(string strFileName, string strExtension, bool bEncodingFileName = true, bool bLoadSave = false)
	{
		try
		{
			if (bEncodingFileName)
			{
				string name = Get_FileName_Encode(Path.GetFileName(strFileName));
				string dir = Path.GetDirectoryName(strFileName);
				StringBuilder builder = new StringBuilder(512);
				builder.Append(dir);
				builder.Append("/");
				builder.Append(name);
				strFileName = builder.ToString();
			}
			StringBuilder txt;
#if UNITY_EDITOR && DATA_PATH_NORMAL
			if (!bLoadSave)
			{
				txt = new StringBuilder(m_strAssetBasePath);
				txt.Append(strFileName);
				txt.Append(strExtension);
				TextAsset obj = AssetDatabase.LoadAssetAtPath<TextAsset>(txt.ToString());
				if (obj != null) return obj.bytes;
			}
#endif
			txt = new StringBuilder(m_strPath);
			txt.Append(strFileName);
			txt.Append(strExtension);
			string fullpath = txt.ToString();

			if (File.Exists(fullpath))
			{
				// 추가다운및 저장으로 인해 생성된 같은 파일이 존재함
				// 안드로이드 게임런처를 통해 플레이시 가끔 로딩중 다른 데이터로 인식되는 경우가 있어 파일 로드 바꿔봄
				byte[] abyBuf = File.ReadAllBytes(fullpath);
				//FileStream fs = new FileStream(fullpath, FileMode.Open, FileAccess.Read);
				//int nBufSize = (int)fs.Length;
				//byte[] abyBuf = new byte[nBufSize];
				//fs.Read(abyBuf, 0, nBufSize);
				//fs.Close();
				return abyBuf;
			}
			else
			{
				TextAsset txtFile = Resources.Load(strFileName) as TextAsset;
				if (txtFile != null) return txtFile.bytes;
			}
		}
		catch (Exception e)
		{
			UnityEngine.Debug.LogError("ReadFile Exception: " + e.ToString());

			TextAsset txtFile = Resources.Load(strFileName) as TextAsset;
			if (txtFile != null) return txtFile.bytes;
		}
		return null;
	}

	public IEnumerator GetData_Async(string strFileName, string strExtension, bool bEncodingFileName = true, Action<byte[]> cb = null)
	{
		if (bEncodingFileName)
		{
			string name = Get_FileName_Encode(Path.GetFileName(strFileName));
			string dir = Path.GetDirectoryName(strFileName);
			StringBuilder builder = new StringBuilder(512);
			builder.Append(dir);
			builder.Append("/");
			builder.Append(name);
			strFileName = builder.ToString();
		}

		StringBuilder txt;
#if UNITY_EDITOR && DATA_PATH_NORMAL
		txt = new StringBuilder(m_strAssetBasePath);
		txt.Append(strFileName);
		txt.Append(strExtension);
		TextAsset obj = AssetDatabase.LoadAssetAtPath<TextAsset>(txt.ToString());
		if (obj != null)
		{
			cb?.Invoke(obj.bytes);
			yield break;
		}
#endif
		txt = new StringBuilder(m_strPath);
		txt.Append(strFileName);
		txt.Append(strExtension);
		string fullpath = txt.ToString();

		if (File.Exists(fullpath))
		{
			// 추가다운및 저장으로 인해 생성된 같은 파일이 존재함
			byte[] abyBuf = File.ReadAllBytes(fullpath);
			//FileStream fs = new FileStream(fullpath, FileMode.Open, FileAccess.Read);
			//byte[] abyBuf = null;
			//if (fs != null)
			//{
			//	int nBufSize = (int)fs.Length;
			//	abyBuf = new byte[nBufSize];
			//	yield return fs.ReadAsync(abyBuf, 0, nBufSize);
			//	fs.Close();
			//}
			cb?.Invoke(abyBuf);
		}
		else
		{
			ResourceRequest data = Resources.LoadAsync<TextAsset>(strFileName);
			yield return data;
			TextAsset txtFile = (TextAsset)data.asset;
			cb?.Invoke(txtFile?.bytes);
		}
	}

	public void CreateDirectory(string strPath)
	{
		StringBuilder txt = new StringBuilder(m_strPath);
		txt.Append(strPath);
		string fullpath = txt.ToString();
		if (!Directory.Exists(fullpath)) Directory.CreateDirectory(fullpath);
	}

	public void DeleteData(string strFileName, string strExtension, bool bEncodingFileName = true)
	{
		try
		{
			StringBuilder txt = new StringBuilder(m_strPath);
			if (bEncodingFileName)
			{
				string name = Get_FileName_Encode(Path.GetFileName(strFileName));
				string dir = Path.GetDirectoryName(strFileName);
				StringBuilder builder = new StringBuilder(512);
				builder.Append(dir);
				builder.Append("/");
				builder.Append(name);
				strFileName = builder.ToString();
			}
			txt.Append(strFileName);
			txt.Append(strExtension);
			string fullpath = txt.ToString();
			if (File.Exists(fullpath)) File.Delete(fullpath);
		}
		catch (Exception e)
		{
			UnityEngine.Debug.LogError("SaveData Exception: " + e.ToString());
		}
	}

	public void SaveData_String(string strFileName, string strExtension, string Data, bool IsEncoding = true)
	{
		byte[] abyData = Encoding.UTF8.GetBytes(Data);
		int nLen = abyData.Length;
		if(IsEncoding) Encode(abyData, nLen, 0);
		SaveData(strFileName, strExtension, abyData, IsEncoding);
	}

	public void SaveData(string strFileName, string strExtension, byte[] inBuf, bool bEncodingFileName = true)
	{
		try
		{
			StringBuilder txt = new StringBuilder(m_strPath);
			if (bEncodingFileName)
			{
				string name = Get_FileName_Encode(Path.GetFileName(strFileName));
				string dir = Path.GetDirectoryName(strFileName);
				StringBuilder builder = new StringBuilder(512);
				builder.Append(dir);
				builder.Append("/");
				builder.Append(name);
				strFileName = builder.ToString();
			}
			txt.Append(strFileName);
			txt.Append(strExtension);
			string fullpath = txt.ToString();
			if (File.Exists(fullpath)) File.Delete(fullpath);
			FileStream fs = new FileStream(fullpath, FileMode.Create, FileAccess.Write);
			fs.Write(inBuf, 0, inBuf.Length);
			fs.Close();
		}
		catch (Exception e)
		{
			UnityEngine.Debug.LogError("SaveData Exception: " + e.ToString());
		}
	}

	static public void SaveData_static(string strFileName, string strExtension, byte[] inBuf, bool bEncodingFileName = true)
	{
		try
		{
			StringBuilder txt = new StringBuilder(m_strPath);
			if (bEncodingFileName)
			{
				string name = Get_FileName_Encode(Path.GetFileName(strFileName));
				string dir = Path.GetDirectoryName(strFileName);
				StringBuilder builder = new StringBuilder(512);
				builder.Append(dir);
				builder.Append("/");
				builder.Append(name);
				strFileName = builder.ToString();
			}
			txt.Append(strFileName);
			txt.Append(strExtension);
			string fullpath = txt.ToString();
			if (File.Exists(fullpath)) File.Delete(fullpath);
			FileStream fs = new FileStream(fullpath, FileMode.Create, FileAccess.Write);
			fs.Write(inBuf, 0, inBuf.Length);
			fs.Close();
		}
		catch (Exception e)
		{
			UnityEngine.Debug.LogError("SaveData Exception: " + e.ToString());
		}
	}

	public void NoneCheckBundle(string BundleName)
	{
		m_strPassBundleName = BundleName;
	}

	void CheckLoadImg()
	{
		var list = MainMng.Instance.m_Web.GetImgBundleDatas();
		for (int i = 0; i < list.Count; i++)
		{
			var strName = list[i];
#if !(NOT_USE_NET || (UNITY_EDITOR && !DATA_PATH_CDN))
			if (!MainMng.Instance.m_Web.CheckFile(strName)) continue;
#endif
			LoadAsset(strName);
		}
	}

	T GetImgAsset<T>(string Path, string Name, string Extention, bool AutoRemove = true) where T : UnityEngine.Object
	{
		if (!string.IsNullOrEmpty(m_strPassBundleName) && m_strPassBundleName.Equals(m_strImageName)) return null;
		//m_strImageName, "Imgs", atlas_name.ToString(), "spriteatlas", false);
		T result = null;
		try
		{
			string fileName = $"{Name}.{Extention}";
			AssetBundle pBundle = null;
			var list = MainMng.Instance.m_Web.GetImgBundleDatas();
			for(int i = 0; i < list.Count; i++)
			{
				var strName = list[i];
#if !(NOT_USE_NET || (UNITY_EDITOR && !DATA_PATH_CDN))
				if (!MainMng.Instance.m_Web.CheckFile(strName)) continue;
#endif
				pBundle = LoadAsset(strName);
				if (pBundle != null)
				{
					result = pBundle.LoadAsset<T>($"{m_strAssetBasePath}{Path}/{fileName}");
					if(result != null)
					{
#if ASSET_BUNDLE_TEST
						if (AutoRemove) UnloadAsset(loadBundle);
#endif
						break;
					}
				}
			}

		}
#pragma warning disable 0168
		catch (Exception e)
		{
			//Debug.LogError(e);
			result = null;
		}
		return result;
	}


	T GetAsset<T>(string Bundle, string Path, string Name, string Extention, bool AutoRemove = true) where T : UnityEngine.Object
	{
		if (!string.IsNullOrEmpty(m_strPassBundleName) && m_strPassBundleName.Equals(Bundle)) return null;
		// 프리팹의경우 리소스들이 캐싱되어있어야지만되므로 오픈해둔다.
		//if (Bundle.Equals(m_strPrefabName)) CheckLoadImg();
		T result = null;
		try
		{
			string fileName = $"{Name}.{Extention}";
			string loadBundle = Get_FileName_Encode(fileName);
			AssetBundle pBundle = LoadAsset(loadBundle);
			if (pBundle == null)
			{
				loadBundle = Bundle;
				pBundle = LoadAsset(loadBundle);
			}

			if (pBundle != null)
			{
				result = pBundle.LoadAsset<T>($"{m_strAssetBasePath}{Path}/{fileName}");
#if ASSET_BUNDLE_TEST
				if (AutoRemove) UnloadAsset(loadBundle);
#endif
			}
		}
#pragma warning disable 0168
		catch (Exception e)
		{
			//Debug.LogError(e);
			result = null;
		}
#pragma warning restore 0168
		return result;
	}

	UnityEngine.Object GetAsset(string Bundle, string Path, string Name, string Extention, bool AutoRemove = true)
	{
		if (!string.IsNullOrEmpty(m_strPassBundleName) && m_strPassBundleName.Equals(Bundle)) return null;
		UnityEngine.Object result = null;
		try
		{
			string fileName = $"{Name}.{Extention}";
			string loadBundle = Get_FileName_Encode(fileName);
			AssetBundle pBundle = LoadAsset(loadBundle);
			if (pBundle == null)
			{
				loadBundle = Bundle;
				pBundle = LoadAsset(loadBundle);
			}

			if (pBundle != null)
			{
				result = pBundle.LoadAsset($"{m_strAssetBasePath}{Path}/{fileName}");
#if ASSET_BUNDLE_TEST
				if (AutoRemove) UnloadAsset(loadBundle);
#endif
			}
		}
#pragma warning disable 0168
		catch (Exception e)
		{
			//Debug.LogError(e);
			result = null;
		}
#pragma warning restore 0168
		return result;
	}


	AsyncOperation GetAssetAsync(string Bundle, string Path, string Name, string Extention)
	{
		AsyncOperation result = null;
		string fileName = $"{Name}.{Extention}";
		string loadBundle = Get_FileName_Encode(fileName);
		AssetBundle pBundle = LoadAsset(loadBundle);
		if (pBundle == null)
		{
			loadBundle = Bundle;
			pBundle = LoadAsset(loadBundle);
		}

		if (pBundle != null)
		{
			result = pBundle.LoadAssetAsync($"{m_strAssetBasePath}{Path}/{fileName}");
		}
		return result;
	}

	/// <summary>Prefab Load</summary>
	public GameObject LoadPrefab(string strFileName, bool bInstantiate, Transform tfParent = null)
	{
		// Instantiate의경우 Awake, Update같이 랜더링부분에서 해야한다.
		GameObject prefab = null;
#if UNITY_EDITOR
		string path = string.Format("{0}Prefabs/{1}.prefab", m_strAssetBasePath, strFileName);
		if (strFileName.Contains("7_Sample")) path = string.Format("Assets/{0}.prefab", strFileName);
		// 변경사항을 바로 확인해야하므로 
		prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
#else   // 패치 에셋은 각 플렛폼별 포멧이 되므로 이미지등 나오지 않는다.
		prefab = GetAsset<GameObject>(m_strPrefabName, "Prefabs", strFileName, "prefab", false);
#endif
		if (prefab == null) prefab = Resources.Load(string.Format("Prefabs/{0}", strFileName)) as GameObject;
		if (prefab == null) return null;
		if (bInstantiate)
		{
			if (tfParent != null) prefab = GameObject.Instantiate(prefab, tfParent) as GameObject;
			else prefab = GameObject.Instantiate(prefab) as GameObject;
			prefab.name = Path.GetFileName(strFileName);
		}
		return prefab;
	}

#if UNITY_EDITOR
	public class UtileResourceRequest : ResourceRequest
	{
		public new bool isDone;
		public new System.Object asset;
	}
#endif

	/// <summary>Prefab Load</summary>
	public AsyncOperation LoadPrefab_Async(string strFileName, out bool bResLoad)
	{
		// Instantiate의경우 Awake, Update같이 랜더링부분에서 해야한다.
		AsyncOperation prefab = null;
		bResLoad = false;
#if UNITY_EDITOR
		// 변경사항을 바로 확인해야하므로 
		GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(string.Format("{0}Prefabs/{1}.prefab", m_strAssetBasePath, strFileName));
		if(obj != null) prefab = new UtileResourceRequest() { isDone = true, asset = obj };
#else   // 패치 에셋은 각 플렛폼별 포멧이 되므로 이미지등 나오지 않는다.
		prefab = GetAssetAsync(m_strPrefabName, "Prefabs", strFileName, "prefab");
#endif
		if (prefab == null)
		{
			prefab = Resources.LoadAsync(string.Format("Prefabs/{0}", strFileName));
			bResLoad = true;
		}
		if (prefab == null) return null;
		return prefab;
	}

	public static GameObject Instantiate(GameObject prefab, Transform tfParent = null)
	{
		GameObject objRe = null;
		if (tfParent != null) objRe = GameObject.Instantiate(prefab, tfParent) as GameObject;
		else objRe = GameObject.Instantiate(prefab) as GameObject;

		return objRe;
	}

	public void Load_Prefab_List(int nCnt, RectTransform rtfList, RectTransform rtfPrefab)
	{
		Load_Prefab_List(nCnt, (Transform)rtfList, (Transform)rtfPrefab);
	}

	public void Load_Prefab_List(int nCnt, Transform tfList, Transform tfPrefab)
	{
		int i;
		int nChildCnt = tfList.childCount;
		int nGap = nCnt - nChildCnt;
		if (nGap < 0)
		{
			// 차이 개수 만큼 차감
			// 처음 위치부터 바로 셋팅해야하는데 바로 삭제가 되지않아 셋팅이 잘못되는경우가 있어 뒤에서부터 삭제한다.
			for (i = nChildCnt - 1; i >= nChildCnt + nGap; i--) GameObject.Destroy(tfList.GetChild(i).gameObject, 0f);
		}
		else
		{
			// 차이 개수만큼 생성
			for (i = 0; i < nGap; i++) Instantiate(tfPrefab.gameObject, tfList);
		}
	}
	public enum AtlasName
	{
		UI = 0,
		Card,
		Font,
		End
	}

	Dictionary<AtlasName, SpriteAtlas> m_pAtlas = new Dictionary<AtlasName, SpriteAtlas>();
	public string GetImgFileName()
	{
		return m_strImageName;
	}
	public void Init_ImgLoadData()
	{
		m_pAtlas.Clear();
	}

	/// <summary>Atlas Load</summary>
	public SpriteAtlas LoadImg(AtlasName atlas_name)
	{
		// Instantiate의경우 Awake, Update같이 랜더링부분에서 해야한다.
		if (m_pAtlas.ContainsKey(atlas_name)) return m_pAtlas[atlas_name];
		SpriteAtlas atlas = null;
#if UNITY_EDITOR && !DATA_PATH_CDN
		atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(string.Format("{0}Imgs/{1}.spriteatlas", m_strAssetBasePath, atlas_name.ToString()));
#else   // 패치 에셋은 각 플렛폼별 포멧이 되므로 이미지등 나오지 않는다.
		// 캐시에서 삭제하면 프리팹 로드에 문제가 되므로 제거하지 않는다.
		atlas = GetImgAsset<SpriteAtlas>("Imgs", atlas_name.ToString(), "spriteatlas", false);
#endif
		if (atlas == null) atlas = Resources.Load(string.Format("Imgs/{0}", atlas_name.ToString())) as SpriteAtlas;
		if (atlas == null) return null;
		m_pAtlas.Add(atlas_name, atlas);
		return atlas;
	}

	/// <summary>Atlas Image Load</summary>
	public Sprite LoadImg(AtlasName atlas_name, string strImgName)
	{
		if (strImgName.Length < 1) return null;
		// Instantiate의경우 Awake, Update같이 랜더링부분에서 해야한다.
		SpriteAtlas atlas = LoadImg(atlas_name);
		//Debug.Log(strImgName);
		return atlas == null ? null : atlas.GetSprite(strImgName);
	}

	/// <summary>Sprite Load</summary>
	public Sprite LoadImg(string strFileName, string strExtention)
	{
		// Instantiate의경우 Awake, Update같이 랜더링부분에서 해야한다.
		Sprite img = null;
#if UNITY_EDITOR && !DATA_PATH_CDN
		img = AssetDatabase.LoadAssetAtPath<Sprite>(string.Format("{0}Imgs/{1}.{2}", m_strAssetBasePath, strFileName, strExtention));
#else   // 패치 에셋은 각 플렛폼별 포멧이 되므로 이미지등 나오지 않는다.
		img = GetImgAsset<Sprite>("Imgs", strFileName, strExtention, true);
#endif
		if (img == null) img = Resources.Load<Sprite>(string.Format("Imgs/{0}", strFileName));
		return img;
	}
	public static T Load_StreamingAsset<T>(string path) where T : class
	{
		//TextAsset _xml = Resources.Load<TextAsset>(path);
		string _xml = File.ReadAllText(Application.streamingAssetsPath + path);

		XmlSerializer serializer = new XmlSerializer(typeof(T));

		StringReader reader = new StringReader(_xml);

		T items = serializer.Deserialize(reader) as T;
		reader.Close();
		return items;
	}

	string m_strLoadFontCode = "";
	SpriteAtlas m_pFontAtlas = null;
	public SpriteAtlas LoadFontAtlas(string strLanguageCode)
	{
		if (m_strLoadFontCode.Equals(strLanguageCode)) return m_pFontAtlas;
		m_pFontAtlas = null;
		m_strLoadFontCode = strLanguageCode;
#if UNITY_EDITOR && !DATA_PATH_CDN
		m_pFontAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(string.Format("{0}Imgs/Font_{1}.spriteatlas", m_strAssetBasePath, strLanguageCode));
#else   // 패치 에셋은 각 플렛폼별 포멧이 되므로 이미지등 나오지 않는다.
		m_pFontAtlas = GetImgAsset<SpriteAtlas>("Fonts", $"Font_{strLanguageCode}", "spriteatlas", true);
#endif
		if (m_pFontAtlas == null) m_pFontAtlas = Resources.Load(string.Format("Imgs/Font_{0}", strLanguageCode)) as SpriteAtlas;
		return m_pFontAtlas;
	}
	/// <summary>Atlas Image Load</summary>
	public Sprite LoadFontImg(string strLanguageCode, string strImgName)
	{
		if (m_pFontAtlas == null || strImgName.Length < 1) return null;
		// Instantiate의경우 Awake, Update같이 랜더링부분에서 해야한다.
		SpriteAtlas atlas = LoadFontAtlas(strLanguageCode);
		return atlas.GetSprite(strImgName);
	}

	/// <summary>Sound Load</summary>
	public AudioClip LoadSnd(string strFileName)
	{
		string[] exts = new string[] { "mp3", "wav" };
		AudioClip snd = null;
#if UNITY_EDITOR && !DATA_PATH_CDN
		for (int i = 0; i< exts.Length; i++) {
			snd = AssetDatabase.LoadAssetAtPath<AudioClip>(string.Format("{0}Snds/{1}.{2}", m_strAssetBasePath, strFileName, exts[i]));
			if (snd != null) break;
		}
#else   // 패치 에셋은 각 플렛폼별 포멧이 되므로 이미지등 나오지 않는다.
		//string strPath = m_strPath + m_strSoundName;
		for (int i = 0; i < exts.Length; i++)
		{
			snd = GetAsset(m_strSoundName, "Snds", strFileName, exts[i], false) as AudioClip;
			if (snd != null) break;
		}
#endif

		if (snd == null) snd = Resources.Load(string.Format("Snds/{0}", strFileName)) as AudioClip;
		return snd;
	}

	public VideoClip LoadVideo(string strFileName) {
		string[] exts = new string[] { "mp4" };
		VideoClip video = null;
#if UNITY_EDITOR && !DATA_PATH_CDN
		for (int i = 0; i < exts.Length; i++) {
			video = AssetDatabase.LoadAssetAtPath<VideoClip>(string.Format("{0}Videos/{1}.{2}", m_strAssetBasePath, strFileName, exts[i]));
			if (video != null) break;
		}
#else
		for (int i = 0; i < exts.Length; i++)
		{
			video = GetAsset(m_strVideoName, "Videos", strFileName, exts[i], true) as VideoClip;
			if (video != null) break;
		}
#endif
		if (video == null) video = Resources.Load(string.Format("Videos/{0}", strFileName)) as VideoClip;
		return video;
	}
	Dictionary<string, AssetBundle> m_pAssets = new Dictionary<string, AssetBundle>();


	public void Init_Assets()
	{
		Init_ImgLoadData();
		AssetBundle.UnloadAllAssetBundles(false);
		//List<string> kyes = new List<string>(m_pAssets.Keys);
		////kyes.RemoveAll(o => o.Equals(m_strImageName));
		//for (int i = kyes.Count - 1; i > -1; i--) m_pAssets[kyes[i]].Unload(false);
		m_pAssets.Clear();
		Resources.UnloadUnusedAssets();

		CheckLoadImg();
	}

	public void UnloadAsset(string key)
	{
		if (!m_pAssets.ContainsKey(key)) return;
		m_pAssets[key].Unload(false);
		m_pAssets.Remove(key);
		Resources.UnloadUnusedAssets();
	}

	public IEnumerator AllLoadAsset()
	{
		Init_Assets();
#if ASSET_BUNDLE_TEST
		List<string> names = new List<string>();
		names.AddRange(MainMng.Instance.m_Web.GetImgBundleDatas());
		names.Add(m_strSoundName);
		for (int i = 0; i < names.Count; i++)
		{
			if (m_pAssets.ContainsKey(names[i])) continue;
			string path = $"{m_strPath}{names[i]}";
			FileInfo info = new FileInfo(path);
			if (!info.Exists) continue;
			UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle($"file://{path}", 1, LS_Web.GetCrc(names[i]));
			yield return uwr.SendWebRequest();

			m_pAssets.Add(names[i], DownloadHandlerAssetBundle.GetContent(uwr));

			if(!names[i].Equals(m_strSoundName))
			{
				// 아틀라스 이미지들 로드
				for (AtlasName j = 0; j < AtlasName.End; j++)
				{
					if (j == AtlasName.Card) continue;
					LoadImg(j);
					Debug.Log($"!!!! Img Load {j}");
				}
			}
		}
#else
		yield break;
#endif
	}

	public AssetBundle LoadAsset(string strName)
	{
#if !(NOT_USE_NET || (UNITY_EDITOR && !DATA_PATH_CDN))
		if (!MainMng.Instance.m_Web.CheckFile(strName)) return null;
#endif
		if (m_pAssets.ContainsKey(strName)) return m_pAssets[strName];
		string path = $"{m_strPath}{strName}";
		try
		{
			if (File.Exists(path))
			{
				AssetBundle pBundle = AssetBundle.LoadFromFile(path);
				if (pBundle != null) m_pAssets.Add(strName, pBundle);
				return pBundle;
			}
		}
		catch(Exception e)
		{
			Debug.LogError(e);
		}
		return null;
	}
	public IEnumerator LoadAssetAsync(string strName)
	{
		if (m_pAssets.ContainsKey(strName))
		{
			m_pAssets[strName].Unload(true);
			m_pAssets.Remove(strName);
		}
		string strPath = string.Format("{0}{1}", m_strPath, strName);
		if (File.Exists(strPath))
		{
			AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(strPath);
			yield return req;
			if(req.assetBundle != null) m_pAssets.Add(strName, req.assetBundle);
		}
	}

	public static UnityEngine.Object SetInstance(Type type)
	{
		UnityEngine.Object[] aobj = Resources.FindObjectsOfTypeAll(type);//GameObject.FindObjectsOfType(type);
		UnityEngine.Object obj = null;
		if (aobj.Length > 0) obj = aobj[0];
		if (!obj)
		{
			GameObject m_inst = new GameObject();
			m_inst.name = type.Name;
			m_inst.AddComponent(type);
			obj = m_inst.GetComponent(type);
		}
		return obj;
	}

	public CSV_Result Get_CsvResult(string strData)
	{
		return new CSV_Result(strData);
	}

	public static CSV_Result Get_CsvResult_static(string strData)
	{
		return new CSV_Result(strData);
	}
}
