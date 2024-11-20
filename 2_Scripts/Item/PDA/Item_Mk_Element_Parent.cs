using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Mk_Element_Parent : ObjMng
{
	public enum AnimState
	{
		Active,
		Lock,
		MateLack,
		Complete,
		Process
	}
	public AnimState m_AnimState;
	public Action<Item_Mk_Element_Parent> m_CB;
	public TMakingTable m_Mk_TData;
	public MakingInfo m_Info { get { return USERINFO.m_Making.Find(t => t.m_TData == m_Mk_TData); } }
	public TimeContentState m_State { get { return m_Info == null ? TimeContentState.Idle : m_Info.m_State; } set { if (m_Info != null) m_Info.m_State = value; } }
	public TItemTable m_Item_TData { get { return TDATA.GetItemTable(m_Mk_TData.m_ItemIdx); } }


	public virtual void SetState(TimeContentState _advstate) {
	}
}
