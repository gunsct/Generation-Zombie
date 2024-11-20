using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public partial class LS_Web
{
	public class REQ_APPCHECK : REQ_BASE
	{
		/// <summary> 앱 확인 코드 </summary>
		public string Mode;
	}


	public void SEND_REQ_APPCHECK(Action<RES_BASE> action)
	{
		REQ_APPCHECK _data = new REQ_APPCHECK();
#if THAILAND
		_data.Mode = "THAILAND";
#elif GVI_CBT
		_data.Mode = "GVI_CBT";
#else
		_data.Mode = Application.companyName;
#endif
		SendPost(Protocol.REQ_APPCHECK, JsonConvert.SerializeObject(_data), false, (ushCode, data) =>
		{
			action?.Invoke(JsonConvert.DeserializeObject<RES_BASE>(data));
		});
	}
}
