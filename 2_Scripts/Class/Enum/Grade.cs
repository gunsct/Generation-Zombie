using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using System.Globalization;
#endif
using UnityEngine;
using UnityEngine.Networking;

public enum Grade
{
	/// <summary> 일반 </summary>
	Normal = 0,
	/// <summary> 쓸만한 </summary>
	Useful,
	/// <summary> 뛰어난 </summary>
	Elite,
	/// <summary> 명품 </summary>
	Luxury,
	/// <summary> 걸작 </summary>
	Masterpiece,
	/// <summary>  </summary>
	Max
}
