using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ItemCardRenderer : ObjMng
{
	[Serializable]
	struct ResetUV
	{
		public bool IsRest;
		// 계산법
		// x = 이미지 width * offset.x
		// y = 이미지 height * offset.y
		// w = 이미지 width * tilling.x
		// h = 이미지 height * (tilling.x + offset.y)
		public Rect Rect;
	}
	[Serializable]
	struct UVINFO
	{
		public string texName;
		public Sprite texture;
		public int uvno;
		public ResetUV Reset;
	}

	public Color m_Color = Color.white;
	[SerializeField] UVINFO[] m_UvTextures;
	[SerializeField] ResetUV m_MainRsetUV;
	Sprite MainTexture;
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
		if (MainTexture != null) SetMainTexture(MainTexture);
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

		SetColor(m_Color);

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
		SetMaterial(new Material(m_MR.sharedMaterial));
	}

	public void SetMaterial(Material mat)
	{
		if (m_MR == null) return;
		m_MR.sharedMaterial = mat;
		SetTexture();
	}

	public void SetMainTexture(Sprite sprite)
	{
		Rect? rect = null;
		MainTexture = sprite;
		if (m_MainRsetUV.IsRest) rect = m_MainRsetUV.Rect;
		SetTexture("_MainTex", 0, sprite, rect);
	}

	public void SetFrameTexture(Sprite _sprite)
	{
		Rect? rect = null;
		if (m_MainRsetUV.IsRest) rect = m_MainRsetUV.Rect;
		m_UvTextures[1].texture = _sprite;
		SetTexture("_Frame", 2, _sprite, rect);
	}
	public void SetMaskTexture(Sprite _sprite)
	{
		Rect? rect = null;
		if (m_MainRsetUV.IsRest) rect = m_MainRsetUV.Rect;
		m_UvTextures[0].texture = _sprite;
		SetTexture("_Mask", 1, _sprite, rect);
	}
	public void SetTexture(string name, int uvno, Sprite img, Rect? rect)
	{
		if (img == null) return;
		m_MR.sharedMaterial.SetTexture(name, img.texture);

		List<Vector2> uvs = Utile_Class.GetMeshUV_From_Sprite_Static(img.uv);
		m_MR.sharedMaterial.SetVector("_Offset" + uvno, Vector2.zero);
		m_MR.sharedMaterial.SetVector("_Tiling" + uvno, Vector2.one);
		// 2	3
		// 0	1
		if (rect != null)
		{
			float oneuv = 1f / img.texture.width;

			float X = rect.Value.x * oneuv;
			float W = rect.Value.width * oneuv;

			float Y = rect.Value.y * oneuv;
			float H = rect.Value.height * oneuv;

			Vector2 start = uvs[0];
			Vector2 value = start;
			value.x += X;
			value.y += Y;
			uvs[0] = value;

			value = start;
			value.x += W;
			value.y += Y;
			uvs[1] = value;

			value = start;
			value.x += X;
			value.y += H;
			uvs[2] = value;

			value = start;
			value.x += W;
			value.y += H;
			uvs[3] = value;
		}
		m_Mesh.SetUVs(uvno, uvs);
	}

	public void SetTexture()
	{
		for (int i = 0; i < m_UvTextures.Length; i++)
		{
			Rect? rect = null;
			if (m_UvTextures[i].Reset.IsRest) rect = m_UvTextures[i].Reset.Rect;
			SetTexture(m_UvTextures[i].texName, m_UvTextures[i].uvno, m_UvTextures[i].texture, rect);
		}
	}

	public void SetFloat(string name, float value)
	{
		if (m_MR == null) return;
		m_MR.sharedMaterial.SetFloat(name, value);
	}
	public float GetFloat(string name)
	{
		if (m_MR == null) return 0;
		return m_MR.sharedMaterial.GetFloat(name);
	}

	public void SetColor(Color value)
	{
		if (m_MR == null) return;
		m_Color = value;
		SetColor("_BaseColor", m_Color);
	}

	public void SetColor(string name, Color value)
	{
		if (m_MR == null) return;
		if (m_MR.sharedMaterial == null) return;
		m_MR.sharedMaterial.SetColor(name, value);
	}
}
