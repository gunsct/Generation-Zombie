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

public partial class Utile_Class
{
	public Vector3 GetGyroValue()
	{
#if USE_ACCELERATION
		return Input.acceleration;
#else
		return Input.mousePosition;
#endif
	}
}
