using System;
using System.Threading;
using System.Collections;
using UnityEngine;
using System.Text;

public abstract class ToolFile : ClassMng
{
	public static int LINE_SLIP_CNT = 1000;
	public static long LOADED_LINT = 0;
	public string[] m_Path;
	public string m_NowPath;
	Thread m_Thread;
	public ToolFile(string Path)
	{
		m_Path = new string[] { Path };
	}
	public ToolFile(string[] Path)
	{
		m_Path = Path;
	}
	public virtual void Load()
	{
		if (m_Path == null) return;
		DataInit();
		for (int i = m_Path.Length - 1; i > -1; i--)
		{
			m_NowPath = m_Path[i];
			byte[] pData = UTILE.GetData(m_NowPath, ".bytes");
			if (pData != null)
			{
				Utile_Class.Decode(pData, pData.Length, 0);
				CSV_Result csv = UTILE.Get_CsvResult(Encoding.UTF8.GetString(pData));
				int size = csv.Get_LineSize();
				for (int j = 0; j < size; j++, csv.next()) ParsLine(csv);
			}
		}
		CheckData();
	}

	public virtual IEnumerator Load_Async(Action<string, long> FileChange, Action<long> Proc)
	{
		if (m_Path == null) yield break;
		DataInit();
		for (int i = m_Path.Length - 1; i > -1; i--)
		{
			byte[] pData = null;
			string path = m_Path[i];
			yield return UTILE.GetData_Async(path, ".bytes", true, (data) => { pData = data; });
			if (pData == null) continue;
			Utile_Class.Decode(pData, pData.Length, 0);
			CSV_Result csv = UTILE.Get_CsvResult(Encoding.UTF8.GetString(pData));

			m_NowPath = m_Path[i];
			m_Thread = new Thread(() => {
				var curtime = UTILE.Get_Time();
				int size = csv.Get_LineSize();
				FileChange?.Invoke(path, size);
				for (int j = 0; j < size; j++, csv.next())
				{
					ParsLine(csv);
					Proc?.Invoke(j + 1);
					// 특정 간격으로만 슬립 주기
					LOADED_LINT++;
					if(LOADED_LINT % LINE_SLIP_CNT == 0) Thread.Sleep(1);
				}
				m_Thread = null;
				var now = UTILE.Get_Time();
				Utile_Class.DebugLog("[ " + path + "] Loading time : " + (now - curtime).ToString("0.####"));
				curtime = now;
			});
			m_Thread.Start();
			yield return new WaitWhile(() => m_Thread != null);

		}
		CheckData();
	}

	/// <summary> 데이터 로드전 초기화 </summary>
	public abstract void DataInit();
	/// <summary> 데이터 라인 파싱 </summary>
	public abstract void ParsLine(CSV_Result pResult);
	/// <summary> 데이터 로드 종료후 체크 </summary>
	public virtual void CheckData() { }
}