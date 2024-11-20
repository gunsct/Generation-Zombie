using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TowerMng : ObjMng
{
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Value
#pragma warning disable 0649
	[SerializeField] Transform m_MapPanel;
	public Camera m_MyCam;
	public SpriteRenderer BG;
	Item_TowerBG m_Map;
	IEnumerator m_PlayAction;
	ETouchState m_TouchState;
	Vector3 m_TouchPoint;
	public Main_Stage m_MainUI;


	public StageCheck m_Check { get { return STAGEINFO.m_Check; } }
	public StageUser m_User { get { return STAGEINFO.m_User; } }
#pragma warning restore 0649
}
