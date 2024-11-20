using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using System.Globalization;
#endif
using UnityEngine;
using UnityEngine.Networking;

public enum BGMaskType
{
	None = 0,
	SunLight_Left,
	SunLight_Right,
	SunLight_Center,
	Red_01,
	Purple_01,
	Blue_01,
	Green_01,
	Gray_01,
	Red_02,
	Purple_02,
	Blue_02,
	Green_02,
	Gray_02,
	Fog_Left,
	Fog_Right,
	Fog_Center,
	Wind_Left,
	Wind_Right,
	Wind_Center,
	Typhoon_Left,
	Typhoon_Right,
	Typhoon_Center,
	Blizzard_Left,
	Blizzard_Center,
	Blizzard_Right
}