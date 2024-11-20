using System.Collections.Generic;
using UnityEngine;


public class CameraArea2D : ClassMng
{
	Camera m_MyCam;
	bool[] GapCheck = new bool[2];
	CameraViewSizeInfo m_Size;
	Vector3 m_SizeMin, m_SizeMax;
	Vector2 m_AreaSize;
	Vector2 m_AreaMin, m_AreaMax;
	public void Init(Camera cam, float SizeX, float SizeY)
	{
		m_MyCam = cam;
		m_Size = Utile_Class.GetViewWorldSizeInfo(m_MyCam);
		m_AreaSize = new Vector2(SizeX, SizeY);
		m_AreaMin = m_AreaSize * -0.5f;
		m_AreaMax = m_AreaSize * 0.5f;
		m_SizeMin = m_Size.origin[0] + m_Size.direction[0] / m_Size.direction[0].z * Mathf.Abs(m_Size.origin[0].z);
		m_SizeMax = m_Size.origin[1] + m_Size.direction[1] / m_Size.direction[1].z * Mathf.Abs(m_Size.origin[1].z);

		SizeX = Mathf.Abs(m_SizeMin.x) + Mathf.Abs(m_SizeMax.x);
		SizeY = Mathf.Abs(m_SizeMin.y) + Mathf.Abs(m_SizeMax.y);
		GapCheck[0] = SizeX < m_AreaSize.x;
		GapCheck[1] = SizeY < m_AreaSize.y;
	}

	public bool IS_UseArea()
	{
		return GapCheck[0] || GapCheck[1];
	}

	public void DrawArea()
	{
		Vector3 Start = m_Size.origin[0];
		Vector3 End = m_SizeMin;
		Debug.DrawLine(Start, End, Color.yellow);
		Start = m_Size.origin[1];
		End = m_SizeMax;
		Debug.DrawLine(Start, End, Color.red);
		Start = new Vector3(m_Size.origin[0].x, m_Size.origin[1].y, m_Size.origin[1].z);
		End = new Vector3(m_SizeMin.x, m_SizeMax.y, m_SizeMax.z);
		Debug.DrawLine(Start, End, Color.blue);
		Start = new Vector3(m_Size.origin[1].x, m_Size.origin[0].y, m_Size.origin[0].z);
		End = new Vector3(m_SizeMax.x, m_SizeMin.y, m_SizeMin.z);
		Debug.DrawLine(Start, End, Color.green);
	}

	public Vector3 GetOverGap()
	{
		Vector3 posmin = m_MyCam.transform.position + m_SizeMin;
		Vector3 posmax = m_MyCam.transform.position + m_SizeMax;
		float GapX = 0f, GapY = 0f;
		if(GapCheck[0])
		{
			if (posmin.x < m_AreaMin.x) GapX = m_AreaMin.x - posmin.x;
			else if (posmax.x > m_AreaMax.x) GapX = m_AreaMax.x - posmax.x;
		}

		if(GapCheck[1])
		{
			if (posmin.y < m_AreaMin.y) GapY = m_AreaMin.y - posmin.y;
			else if (posmax.y > m_AreaMax.y) GapY = m_AreaMax.y - posmax.y;
		}

		return new Vector3(GapX, GapY, 0f);
	}
}
