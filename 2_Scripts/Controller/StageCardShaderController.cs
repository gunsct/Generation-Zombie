using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

[System.Serializable] public class DicMaterialPositionData : SerializableDictionary<string, MaterialPositionData> { }
[System.Serializable]
public class MaterialPositionData
{
	// 1f / tiling value
	public Vector2 Scale;
	public Vector2 Position;
}

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class StageCardShaderController : ObjMng
{
	public Color m_Color = Color.white;
	[SerializeField] DicMaterialPositionData MatTransFormData;
	[SerializeField] Sprite StatMask;
	[SerializeField] Sprite StatArrowMask;
	MeshRenderer _mr = null; 
	MeshRenderer m_MR
	{
		get
		{
			if (_mr == null) _mr = GetComponent<MeshRenderer>();
			return _mr;
		}
	}
	Mesh m_Mesh = null;

	private void Awake()
	{
		Init();
	}

	public void Init()
	{
		MeshFilter pMf = GetComponent<MeshFilter>();
		if (pMf == null) return;
#if UNITY_EDITOR
		if (!EditorApplication.isPlaying && m_Mesh != null) return;
#endif
		m_Mesh = new Mesh();
		int[] tri = new int[6];
		Vector3[] ver = new Vector3[4];
		Vector2[] uv = new Vector2[4];
		Color[] colors = new Color[4];


		// vertex
		ver[0] = new Vector3(-0.5f, -0.5f, 0);
		ver[1] = new Vector3(0.5f, -0.5f, 0);
		ver[2] = new Vector3(-0.5f, 0.5f, 0);
		ver[3] = new Vector3(0.5f, 0.5f, 0);

		// uv0
		uv[0] = new Vector2(0, 0);
		uv[1] = new Vector2(1, 0);
		uv[2] = new Vector2(0, 1);
		uv[3] = new Vector2(1, 1);

		// triangles
		// 1 (0, 2, 1)
		tri[0] = 0;
		tri[1] = 2;
		tri[2] = 1;
		// 2 (2, 3, 1)
		tri[3] = 2;
		tri[4] = 3;
		tri[5] = 1;

		colors[0] = m_Color;
		colors[1] = m_Color;
		colors[2] = m_Color;
		colors[3] = m_Color;

		m_Mesh.name = "card";
		m_Mesh.vertices = ver;
		m_Mesh.triangles = tri;
		m_Mesh.uv = uv;
		m_Mesh.colors = colors;

		m_Mesh.SetUVs(3, uv);

		pMf.mesh = m_Mesh;
#if UNITY_EDITOR
		if (!EditorApplication.isPlaying) return;
#endif
		if (m_MR == null) return;
		m_MR.sharedMaterial = new Material(m_MR.sharedMaterial);
	}
	void SetTexture(string TexKey, string TilintKey, string OffsetKey, Sprite img)
	{
		if(img == null)
		{
			m_MR.material.SetTexture(TexKey, null);
			return;
		}
		Texture tex = img.texture;
		m_MR.material.SetTexture(TexKey, tex);
		float ScaleX = 1f, ScaleY = 1f;
		float MoveX = 0f, MoveY = 0f;
		if (MatTransFormData.ContainsKey(TexKey))
		{
			var tr = MatTransFormData[TexKey];
			ScaleX = tr.Scale.x;
			ScaleY = tr.Scale.y;
			float hW = img.textureRect.width * 0.5f;
			float hH = img.textureRect.height * 0.5f;
			// 중간 + 이미지 중앙크기만큼
			MoveX = -hW / ScaleX + hW - tr.Position.x * img.textureRect.width / ScaleX;// hW - (hW + hW * tr.Position.x) / ScaleX;
			MoveY = -hH / ScaleY + hH - tr.Position.y * img.textureRect.height / ScaleY;// hH - (hH / ScaleY - ;   
		}
		if (!string.IsNullOrEmpty(TilintKey))
		{
			Vector2 tiling = Vector2.one;
			tiling.x = img.textureRect.width / tex.width / ScaleX;
			tiling.y = img.textureRect.height / tex.height / ScaleY;
			m_MR.material.SetVector(TilintKey, tiling);
		}

		if (!string.IsNullOrEmpty(OffsetKey))
		{
			// 쉐이더 연산에서 Offset부터한 후 Tiling이 되므로 스케일값 적용 X
			Vector2 offset = Vector2.zero;
			offset.x = (img.textureRect.x + MoveX) / tex.width;
			offset.y = (img.textureRect.y + MoveY) / tex.height;
			m_MR.material.SetVector(OffsetKey, offset);
		}
	}

	public void SetFrame(Sprite img)
	{
		SetTexture("_FrameTex", "_Frame_Tiling", "_Frame_Offset", img);
	}

	public void SetFrameMask(Sprite img)
	{
		SetTexture("_Mask", "_Mask_Tiling", "_Mask_Offset", img);
	}

	public void SetCard(Sprite img)
	{
		SetTexture("_CardTex", "_Card_Tiling", "_Card_Offset", img);
	}

	public void SetStat(bool Active, Sprite Stat, Sprite Arrow = null)
	{
		SetTexture("_StatMark", "_StatMark_Tiling", "_StatMark_Offset", Stat);// stat 위치 잡기
		SetTexture("_StatMarkMask", "_StatMarkMask_Tiling", "_StatMarkMask_Offset", StatMask); // stat 마스크

		if (Arrow != null) {
			m_MR.sharedMaterial.SetInt("_UseStatMarkArrow", 1);// stat 방향 온오프 (0 : 오프, 1 : 온)
			SetTexture("_StatMarkArrow", "_StatMarkArrow_Tiling", "_StatMarkArrow_Offset", Arrow); // stat 위치 잡기
			SetTexture("_StatMarkArrowMask", "_StatMarkArrowMask_Tiling", "_StatMarkArrowMask_Offset", StatArrowMask); // stat 마스크
		}
		else m_MR.sharedMaterial.SetInt("_UseStatMarkArrow", 0);// stat 방향 온오프 (0 : 오프, 1 : 온)
		//if (Active)
		//{
		//	m_MR.sharedMaterial.SetInt("_UseStatMark", 1);// stat 온오프 (0 : 오프, 1 : 온)
			
		//}
		//else m_MR.sharedMaterial.SetInt("_UseStatMark", 0);// stat 온오프 (0 : 오프, 1 : 온)
	}
	public void SetStatActive(bool _active) {
		m_MR.sharedMaterial.SetInt("_UseStatMark", _active ? 1 : 0);// stat 온오프 (0 : 오프, 1 : 온)
	}
	public bool GetStatActive() {
		return Convert.ToBoolean(m_MR.sharedMaterial.GetInt("_UseStatMark"));
	}

	public void Active_HP(bool Active)
	{
		m_MR.sharedMaterial.SetInt("_UseHP", Active ? 1 : 0);// HP 온오프 (0 : 오프, 1 : 온)
	}

	public void SetHPImg(Sprite Frame, Sprite Back, Sprite Now)
	{
		//SetTexture("_HPGaugeGrid", "", "", HPGrid); // HP Grid 텍스쳐 셋팅
		SetTexture("_HPFrame", "", "", Frame); // HP Frame 텍스쳐 셋팅
		//SetTexture("_HPBG", "", "", HPBG); // HP 배경 텍스쳐 셋팅
		SetTexture("_HPBackTex", "", "", Back); // HP 안쪽 게이지 텍스쳐 셋팅
		SetTexture("_HPTex", "", "", Now); // HP 게이지 텍스쳐 셋팅 
	}

	public void SetHPBackAmount(float _amount)
	{
		m_MR.sharedMaterial.SetFloat("_HPBackAmount", _amount); // HP Grid 텍스쳐 셋팅
	}

	public void SetHPAmount(float _amount)
	{
		m_MR.sharedMaterial.SetFloat("_HPAmount", _amount); // HP Grid 텍스쳐 셋팅
	}

	public void ActiveShadow(bool Active)
	{
		m_MR.sharedMaterial.SetInt("_UseShadow", Active ? 1 : 0); // HP Grid 텍스쳐 셋팅
	}
	public void SetShadowColor(Color color)
	{
		m_MR.sharedMaterial.SetColor("_ShadowColor", color); // HP Grid 텍스쳐 셋팅
	}

	public void SetShadowImg(Sprite img)
	{
		SetTexture("_ShadowTex", "_Shadow_Tiling", "_Shadow_Offset", img);
	}



	public void SetFloat(string name, float value) {
		if (m_MR == null) return;
		m_MR.sharedMaterial.SetFloat(name, value);
	}
	public float GetFloat(string name) {
		if (m_MR == null) return 0;
		return m_MR.sharedMaterial.GetFloat(name);
	}
	public void SetColor(string name, Color value) {
		if (m_MR == null) return;
		if (m_MR.sharedMaterial == null) return;
		m_MR.sharedMaterial.SetColor(name, value);
	}
}
