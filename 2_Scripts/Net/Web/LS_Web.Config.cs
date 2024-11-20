using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public partial class LS_Web
{
	public class REQ_CONFIG : REQ_BASE
	{
		/// <summary> 요청 Config 리스트 </summary>
		public List<string> Names;
		public string Nation;
	}

	public class RES_CONFIG : RES_BASE
	{
		/// <summary> 요청 받은 Config 리스트 결과</summary>
		public List<ConfigData> Configs;
	}

	public class ConfigData
	{
		/// <summary> 정보 이름 </summary>
		public string Name;
		/// <summary> 정보 값 </summary>
		public string Value;
	}

	public void SEND_REQ_CONFIG(Action<RES_CONFIG> action, params EServerConfig[] args)
	{
		REQ_CONFIG _data = new REQ_CONFIG();
		_data.Names = new List<string>();
		_data.Nation = APPINFO.CountryCode;
		for (int i = args.Length - 1; i > -1; i--) _data.Names.Add(GetConfigKeyString(args[i]));
		SendPost(Protocol.REQ_CONFIG, JsonConvert.SerializeObject(_data), false, (ushCode, data) =>
		{
			action?.Invoke(ParsResData<RES_CONFIG>(data));
		});
	}

	public void SEND_REQ_SERVERTIME()
	{
		REQ_BASE _data = new REQ_BASE();
		SendPost(Protocol.REQ_SERVERTIME, JsonConvert.SerializeObject(_data), false, (ushCode, data) => {
			RES_BASE res = ParsResData<RES_CONFIG>(data);// JsonConvert.DeserializeObject<RES_BASE>(data);
			MainMng.Instance.m_Utile.SetServerTime(res.servertime);
		});
	}
	
}
