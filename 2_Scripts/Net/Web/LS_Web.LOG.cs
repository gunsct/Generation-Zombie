using System.Text;
using UnityEngine;

public partial class LS_Web
{
	[System.Diagnostics.Conditional("USE_LOG_MANAGER")]
	static public void Log(string s)
	{
		Debug.Log("[FA] " + s);
	}

	[System.Diagnostics.Conditional("USE_LOG_MANAGER")]
	static public void LogWarning(string s)
	{
		Debug.LogWarning("[FA] " + s);
	}

	[System.Diagnostics.Conditional("USE_LOG_MANAGER")]
	static public void LogError(string s)
	{
		Debug.LogError("[FA] " + s);
	}

	static StringBuilder m_strNetLog_Req;
	static StringBuilder m_strNetLog_Res;

	// 로그 기록용
	[System.Diagnostics.Conditional("NET_DEBUG")]
	static public void Net_LogInit_Res()
	{
		m_strNetLog_Res = new StringBuilder("");
		m_strNetLog_Res.Append("//////////////////////////////////////////////////////////////////////////////////");
	}

	[System.Diagnostics.Conditional("NET_DEBUG")]
	static public void Net_LogInit_Req()
	{
		m_strNetLog_Req = new StringBuilder("");
		m_strNetLog_Req.Append("//////////////////////////////////////////////////////////////////////////////////");
	}

	[System.Diagnostics.Conditional("NET_DEBUG")]
	static public void Net_Log_Res(string strMsg)
	{
		if (m_strNetLog_Res == null) m_strNetLog_Res = new StringBuilder("");
		m_strNetLog_Res.AppendFormat("\r\n//\t{0}", strMsg);
	}

	[System.Diagnostics.Conditional("NET_DEBUG")]
	static public void Net_Log_Req(string strMsg)
	{
		if (m_strNetLog_Req == null) m_strNetLog_Req = new StringBuilder("");
		m_strNetLog_Req.AppendFormat("\r\n//\t{0}", strMsg);
	}

	[System.Diagnostics.Conditional("NET_DEBUG")]
	static public void Net_LogPrint_Req()
	{
		if (m_strNetLog_Req == null) return;
		m_strNetLog_Req.Append("\r\n//////////////////////////////////////////////////////////////////////////////////");
		Log(m_strNetLog_Req.ToString());
		m_strNetLog_Req = null;
	}

	[System.Diagnostics.Conditional("NET_DEBUG")]
	static public void Net_LogPrint_Res()
	{
		if (m_strNetLog_Res == null) return;
		m_strNetLog_Res.Append("\r\n//////////////////////////////////////////////////////////////////////////////////");
		Log(m_strNetLog_Res.ToString());
		m_strNetLog_Res = null;
	}


	[System.Diagnostics.Conditional("NET_DEBUG")]
	static public void Net_LogPrint_All()
	{
		Net_LogPrint_Req();
		Net_LogPrint_Res();
	}

	[System.Diagnostics.Conditional("NET_DEBUG")]
	static public void Web_Log(string strMsg)
	{
		Debug.Log("[FA] " + strMsg);
	}
}
