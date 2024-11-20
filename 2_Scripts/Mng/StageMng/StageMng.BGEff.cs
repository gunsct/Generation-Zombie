using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class StageMng : ObjMng
{
	void SetBGEff()
	{
		for (int i = 0; i < m_BGEffPanel[0].childCount; i++) Destroy(m_BGEffPanel[0].GetChild(i).gameObject);
		for (int i = 0; i < m_BGEffPanel[1].childCount; i++) Destroy(m_BGEffPanel[1].GetChild(i).gameObject);

		List<BGMaskType> list = STAGEINFO.m_TStage.m_BGs;
		for (int i = 0; i < list.Count; i++)
		{
			BGMaskType type = list[i];
			string prefabname = "";
			switch (type)
			{
			case BGMaskType.SunLight_Left:
				prefabname = "Effect/Stage/BG_SunLight_Left";
				break;
			case BGMaskType.SunLight_Right:
				prefabname = "Effect/Stage/BG_SunLight_Right";
				break;
			case BGMaskType.SunLight_Center:
				prefabname = "Effect/Stage/BG_SunLight_Center";
				break;
			case BGMaskType.Red_01:
				prefabname = "Effect/Stage/BG_PP_Red_01";
				break;
			case BGMaskType.Purple_01:
				prefabname = "Effect/Stage/BG_PP_Purple_01";
				break;
			case BGMaskType.Blue_01:
				prefabname = "Effect/Stage/BG_PP_Blue_01";
				break;
			case BGMaskType.Green_01:
				prefabname = "Effect/Stage/BG_PP_Green_01";
				break;
			case BGMaskType.Gray_01:
				prefabname = "Effect/Stage/BG_PP_Gray_01";
				break;
			case BGMaskType.Red_02:
				prefabname = "Effect/Stage/BG_PP_Red_02";
				break;
			case BGMaskType.Purple_02:
				prefabname = "Effect/Stage/BG_PP_Purple_02";
				break;
			case BGMaskType.Blue_02:
				prefabname = "Effect/Stage/BG_PP_Blue_02";
				break;
			case BGMaskType.Green_02:
				prefabname = "Effect/Stage/BG_PP_Green_02";
				break;
			case BGMaskType.Gray_02:
				prefabname = "Effect/Stage/BG_PP_Gray_02";
				break;
			case BGMaskType.Fog_Left:
				prefabname = "Effect/Stage/BG_PP_Fog_Left";
				break;
			case BGMaskType.Fog_Right:
				prefabname = "Effect/Stage/BG_PP_Fog_Right";
				break;
			case BGMaskType.Wind_Left:
				prefabname = "Effect/Stage/BG_PP_Wind_Left";
				break;
			case BGMaskType.Wind_Right:
				prefabname = "Effect/Stage/BG_PP_Wind_Right";
				break;
			case BGMaskType.Typhoon_Left:
				prefabname = "Effect/Stage/BG_PP_Typhoon_Left";
				break;
			case BGMaskType.Typhoon_Right:
				prefabname = "Effect/Stage/BG_PP_Typhoon_Right";
				break;
			case BGMaskType.Blizzard_Left:
				prefabname = "Effect/Stage/BG_PP_Blizzard_Left";
				break;
			case BGMaskType.Blizzard_Center:
				prefabname = "Effect/Stage/BG_PP_Blizzard_Center";
				break; 
			case BGMaskType.Blizzard_Right:
				prefabname = "Effect/Stage/BG_PP_Blizzard_Right";
				break;
			default:
				return;
			}

			Vector3 pos = Vector3.zero;
			Vector3 scale = Vector3.one;

			CameraViewSizeInfo info = Utile_Class.GetViewWorldSizeInfo(m_MyCam);
			Vector3 min = info.origin[0] + info.direction[0] / info.direction[0].z * Mathf.Abs(info.origin[0].z);
			Vector3 max = info.origin[1] + info.direction[1] / info.direction[1].z * Mathf.Abs(info.origin[1].z);

			float w = Mathf.Abs(min.x) + Mathf.Abs(max.x);
			float h = Mathf.Abs(min.y) + Mathf.Abs(max.y);

			switch (type)
			{
			case BGMaskType.SunLight_Left:
				pos = new Vector3(min.x, max.y, 0f);
				break;
			case BGMaskType.SunLight_Right:
				pos = new Vector3(max.x, max.y, 0f);
				break;
			case BGMaskType.SunLight_Center:
				pos = new Vector3(0, max.y, 0f);
				break;
			}
			GameObject eff = UTILE.LoadPrefab(prefabname, true, m_BGEffPanel[1]);
			eff.transform.localPosition = pos;
			eff.transform.localScale = scale;
		}
	}
}
