using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public enum CDN_RESULT
{
	NET_ERROR = -2
	, HTTP_ERROR = -1
	, COMPLATE = 0
	, DOWN_START
}

public partial class LS_Web
{
	[System.Serializable]
	public class patch_data
	{
		public long size;
		public List<file_data> crcs = new List<file_data>();

		public List<string> imgBundles = new List<string>();
	}

	[System.Serializable]
	public class file_data
	{
		public string path;
		public string file;
		public long crc;
		public long size;
		public file_data(string path, string file, long crc, long size)
		{
			this.path = path;
			this.file = file;
			this.crc = crc;
			this.size = size;
		}
	}

	string m_strFileName { get { return string.Format("{0}.txt", Utile_Class.Get_FileName_Encode("patch")); } }
	
	static patch_data m_pLocal;
	patch_data m_pDown;


	string Get_PatchFileFullPath()
	{
		return string.Format("{0}{1}", UTILE.Get_FilePath(), m_strFileName);
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// CDN Check
	public void CDN_Check(Action<CDN_RESULT, List<file_data>> cb)
	{
		// 파일 검사
		MAIN.StartCoroutine(CDN_Checker(string.Format("{0}{1}", GetConfig(EServerConfig.CDN_url), m_strFileName), cb));
	}

	public void LoadLocalData()
	{
		string filepath = Get_PatchFileFullPath();
		FileInfo info = new FileInfo(filepath);
		m_pLocal = null;
		List<file_data> downfiles = new List<file_data>();
		if (info.Exists)
		{
			byte[] abyData = File.ReadAllBytes(filepath);
			Utile_Class.Decode(abyData, abyData.Length, 0);
			string result = Encoding.UTF8.GetString(abyData);
			m_pLocal = JsonUtility.FromJson<patch_data>(result);
			string imgBaseName = UTILE.GetImgFileName();
			m_pLocal.imgBundles = m_pLocal.crcs.FindAll(o => o.file.IndexOf(imgBaseName) > -1).Select(o => o.file).ToList();
		}
		else m_pLocal = new patch_data();
	}

	public List<string> GetImgBundleDatas()
	{
		if (m_pLocal == null) LoadLocalData();
		return m_pLocal.imgBundles;
	}

	public static List<file_data> GetDownFileDatas()
	{
		if (m_pLocal == null) return new List<file_data>();
		return m_pLocal.crcs;
	}

	public static uint GetCrc(string filename)
	{
		var data = m_pLocal.crcs.Find(o => o.file.Equals(filename));
		if (data == null) return 0;
		return (uint)data.crc;
	}

	IEnumerator CDN_Checker(string patch_url, Action<CDN_RESULT, List<file_data>> action)
	{
		Web_Log("patch URL : " + patch_url);
		LoadLocalData();
		UnityWebRequest www = new UnityWebRequest(patch_url);
		www.downloadHandler = new DownloadHandlerBuffer();
		yield return www.SendWebRequest();
		if (www.isDone && www.result != UnityWebRequest.Result.ConnectionError && www.result != UnityWebRequest.Result.ProtocolError)
		{
			byte[] abyData = www.downloadHandler.data;
			Utile_Class.Decode(abyData, abyData.Length, 0);
			// {"size":35338097,"crcs":[{"path":"","file":"73646e53","crc":2551069853,"size":35338097}]}
			string json = Encoding.UTF8.GetString(abyData);
			Web_Log(json);
			m_pDown = JsonUtility.FromJson<patch_data>(json);

			//m_pDown = JsonUtility.FromJson<patch_data>(www.downloadHandler.text);   // 서버 패치데이터

			List<file_data> downfiles = new List<file_data>();

			if (m_pLocal.crcs.Count == 0)
			{
				downfiles = m_pDown.crcs;
			}
			else
			{
				for (int i = m_pLocal.crcs.Count - 1; i > -1; i--)
				{
					file_data ldata = m_pLocal.crcs[i];
					// 이전 다운로드에 등록되어있다가 제거된 파일 찾기
					if (m_pDown.crcs.Find(d => d.file.Equals(ldata.file)) == null)
					{
						// 번들 파일 언로드(타이틀 사운드같이 시작부터 플레이되는 놈들도 있으므로 에러를 막기위해 언로드해줌)
						UTILE.UnloadAsset(ldata.file);

						// 파일 제거
						string path = string.IsNullOrEmpty(ldata.path) ? $"{ldata.file}" : $"{ldata.path}/{ldata.file}";
						Utile_Class.DebugLog($"파일 제거 : {path}");
						string file = $"{UTILE.Get_FilePath()}{path}";
						if (File.Exists(file)) File.Delete(file);

						m_pLocal.crcs.Remove(ldata);
					}
				}


				file_data[] list = m_pDown.crcs.ToArray();
				for (int i = list.Length - 1; i > -1; i--)
				{
					file_data data = list[i];
					file_data d = m_pLocal.crcs.Find(I => I.file == data.file && I.crc == data.crc);
					string localfile = UTILE.Get_FilePath() + (string.IsNullOrEmpty(data.path) ? data.file : data.path + "/" + data.file);
					FileInfo fileinfo = new FileInfo(localfile);
#if UNITY_EDITOR
					// 유니티에서 작업은 테스트로 파일을 변경하면서도 하므로 사이즈 체크는 제거
					if (!fileinfo.Exists || d == null || d.size != fileinfo.Length) downfiles.Add(data);
#else

					if(!fileinfo.Exists || d == null || d.size != fileinfo.Length) downfiles.Add(data);
#endif
				}
			}
			action(CDN_RESULT.COMPLATE, downfiles);
		}
		else
		{
			action(CDN_RESULT.HTTP_ERROR, null);
		}

		yield break;
	}

	public bool CheckFile(string filename)
	{
		List<file_data> re = new List<file_data>();
		file_data d = m_pLocal.crcs.Find(I => I.file == filename);
		if (d == null) return false;
		string localfile = UTILE.Get_FilePath() + (string.IsNullOrEmpty(d.path) ? d.file : d.path + "/" + d.file);
		FileInfo fileinfo = new FileInfo(localfile);
		return fileinfo.Exists && d.size == fileinfo.Length;
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// CDN Check
	/// <summary> 파일 다운로드 시작하기 </summary>
	/// <param name="URL">cdn 기본 URL</param>
	/// <param name="files">CDN_Check로 전달받은 파일 리스트</param>
	/// <param name="startCB">다운로드 시작된 파일 알림 콜백</param>
	/// <param name="actionProgress">startCB 진행상태 알림 콜백</param>
	public void StartDownLoad(List<file_data> files, Action<file_data> startCB, Action<float> actionProgress, Action<ushort> ErrorCB, Action EndCB)
	{
		//MAIN.StartCoroutine(RunThrowingIterator(DownLoad(GetConfig(EServerConfig.CDN_url), files, startCB, actionProgress, ErrorCB, EndCB), (ex) => {
		//	ErrorCB?.Invoke(EStateError.ERROR_NETERROR);
		//}));

		MAIN.StartCoroutine(DownLoad(GetConfig(EServerConfig.CDN_url), files, startCB, actionProgress, ErrorCB, EndCB));
	}


	IEnumerator DownLoad(string URL, List<file_data> files, Action<file_data> startCB, Action<float> actionProgress, Action<ushort> ErrorCB, Action EndCB)
	{
		// 화면이 자동으로 잠기면 넷 에러가 나므로
		// 잠기지 않도록 변경
		Utile_Class.Set_AutoScreenSleep(true);
		HIVE.Analytics_StartCDN();
		files.Sort((befor, after) => befor.size.CompareTo(after.size));
		for (int i = files.Count - 1; i > -1; i--)
		{
			file_data data = files[i];
			startCB?.Invoke(data);
			UTILE.NoneCheckBundle(data.file);

			// 번들 파일 언로드(타이틀 사운드같이 시작부터 플레이되는 놈들도 있으므로 에러를 막기위해 언로드해줌)
			UTILE.UnloadAsset(data.file);

			string path = string.IsNullOrEmpty(data.path) ? $"{data.file}" : $"{data.path}/{data.file}";
			Utile_Class.DebugLog(path);
			string url = $"{URL}{path}";
			string file = $"{UTILE.Get_FilePath()}{path}";

			if (File.Exists(file)) File.Delete(file);

			UnityWebRequest www = new UnityWebRequest(url);
			www.downloadHandler = new DownloadHandlerFile(file);
			www.SendWebRequest();
			float progress = 0;
			float time = Time.unscaledTime + WAIT_TIME;
			while (!www.isDone)
			{
				actionProgress?.Invoke(www.downloadProgress);
				if(progress != www.downloadProgress)
				{
					time = Time.unscaledTime + WAIT_TIME;
					progress = www.downloadProgress;
				}
				if (time < Time.unscaledTime)
				{
					ErrorCB?.Invoke(EStateError.ERROR_TIMEOUT);
					yield break;
				}
				yield return null;
			}
			yield return null;

			if (www.result == UnityWebRequest.Result.ConnectionError)
			{
				ErrorCB?.Invoke(EStateError.ERROR_NETERROR);
				yield break;
			}
			else if (www.result == UnityWebRequest.Result.ProtocolError)
			{
				ErrorCB?.Invoke(EStateError.ERROR_SERVER_EXCEPTION);
				yield break;
			}
			else if (www.result == UnityWebRequest.Result.DataProcessingError)
			{
				ErrorCB?.Invoke(EStateError.ERROR_DATA_PROCESS);
				yield break;
			} 
			else
			{
				// 다음 에 다시 받지 않도록 
				// 다운로드 완료된 내용 적용
				file_data local = m_pLocal.crcs.Find(I => I.file == data.file);

				if (local != null)
				{
					local.crc = data.crc;
					local.size = data.size;
				}
				else
				{
					m_pLocal.crcs.Add(data);
				}

				m_pLocal.size = m_pLocal.crcs.Sum(o => o.size);

				string json = JsonUtility.ToJson(m_pLocal);
				byte[] abyData = Encoding.UTF8.GetBytes(json);
				Utile_Class.Encode(abyData, abyData.Length, 0);
				File.WriteAllBytes(Get_PatchFileFullPath(), abyData);
			}
			yield return new WaitForEndOfFrame();
		}
		HIVE.Analytics_EndCDN();
		// 로컬 데이터는 이미지 로드등에서 다시사용 되므로 저장된 데이터 다시 로드 해둔다.
		LoadLocalData();
		Utile_Class.Set_AutoScreenSleep(false);
		UTILE.NoneCheckBundle(null);
		UTILE.Init_Assets();

		EndCB?.Invoke();
		yield break;
	}
}
