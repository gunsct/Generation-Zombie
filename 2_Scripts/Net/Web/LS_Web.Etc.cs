using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using static AppInfo;
using static hive.AuthV4;

public partial class LS_Web
{
	public class REQ_RE_RES_LSAT_DATA : REQ_BASE
	{
		/// <summary> 마지막 요청 프로토콜 코드 </summary>
		public long ProtocolCode;
	}
}
