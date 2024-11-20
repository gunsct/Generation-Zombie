#define USE_CAM_RENDER
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class RenderAlpha_Controller : MonoBehaviour
{
	[Range(0f, 1f)]
	public float Alpha = 1f;
	// 기존 값들
	Dictionary<Component, Color> m_Colors = new Dictionary<Component, Color>();
	Dictionary<Component, Material> m_Mats = new Dictionary<Component, Material>();
	[HideInInspector] public List<SpriteRenderer> m_Sprites;
	[HideInInspector] public List<MeshRenderer> m_Meshs;
	[HideInInspector] public List<TextMeshPro> m_TextPros;
	[HideInInspector] public List<ParticleSystem> m_Particles;
#if !USE_CAM_RENDER
	float m_NowAlpha = 0;
#endif


	RenderAlpha_Controller[] m_Parents;

	// Start is called before the first frame update
	//void Awake()
	//{
	//	SetList();
	//}
	void OnEnable()
	{
		SetList();
#if USE_CAM_RENDER
#  if UNITY_2019_1_OR_NEWER
#    if UNITY_2019_3_OR_NEWER
		if (GraphicsSettings.currentRenderPipeline == null)
#    else
		if (GraphicsSettings.renderPipelineAsset == null)
#    endif
		{
			// Built-in Render Pipeline
			Camera.onPreRender += OnCamPreRender;
			//if (!Application.isPlaying) Camera.onPostRender += OnCamPostRender;
		}
		else
		{
			// URP
			RenderPipelineManager.beginCameraRendering += OnPreRender_URP;
			//if (!Application.isPlaying) RenderPipelineManager.endCameraRendering += OnPostRender_URP;
		}
#  else
			Camera.onPreRender += OnCamPreRender;
			//if (!Application.isPlaying) Camera.onPostRender += OnCamPostRender;
#  endif
#endif
	}

	void OnDisable()
	{
#if USE_CAM_RENDER
#  if UNITY_2019_1_OR_NEWER
#    if UNITY_2019_3_OR_NEWER
		if (GraphicsSettings.currentRenderPipeline == null)
#    else
		if (GraphicsSettings.renderPipelineAsset == null)
#    endif
		{
			// Built-in Render Pipeline
			Camera.onPreRender -= OnCamPreRender;
			Camera.onPostRender -= OnCamPostRender;
		}
		else
		{
			// URP
			RenderPipelineManager.beginCameraRendering -= OnPreRender_URP;
			RenderPipelineManager.endCameraRendering -= OnPostRender_URP;
		}
#  else
		Camera.onPreRender -= OnCamPreRender;
		Camera.onPostRender -= OnCamPostRender;
#  endif
#elif UNITY_EDITOR
#  if UNITY_2019_1_OR_NEWER
#    if UNITY_2019_3_OR_NEWER
		if (GraphicsSettings.currentRenderPipeline == null)
#    else
		if (GraphicsSettings.renderPipelineAsset == null)
#    endif
		{
			// Built-in Render Pipeline
			Camera.onPreRender -= OnCamPreRender;
			if (!Application.isPlaying)Camera.onPostRender -= OnCamPostRender;
		}
		else
		{
			// URP
			RenderPipelineManager.beginCameraRendering -= OnPreRender_URP;
			if (!Application.isPlaying)RenderPipelineManager.endCameraRendering -= OnPostRender_URP;
		}
#  else
		Camera.onPreRender -= OnCamPreRender;
		if (!Application.isPlaying)Camera.onPostRender -= OnCamPostRender;
#  endif
#endif
	}

	public void SetAlpha(float alpha)
	{
		Alpha = alpha;
	}

	public void SetList()
	{
#if !USE_CAM_RENDER
		m_NowAlpha = -1f;
#endif
		m_Colors.Clear();
		m_Mats.Clear();
		m_Sprites = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>(true));
		m_Meshs = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>(true));
		m_Meshs.RemoveAll(o => o.GetComponent<TextMeshPro>() != null || o.GetComponent<TMP_SubMesh>() != null);
		m_TextPros = new List<TextMeshPro>(GetComponentsInChildren<TextMeshPro>(true));
		m_Particles = new List<ParticleSystem>(GetComponentsInChildren<ParticleSystem>(true));

		m_Parents = GetComponentsInParent<RenderAlpha_Controller>(true);

		// 호출되는 순서에의해 꼬여있을 수 있으므로 부모와 자식을 검사해서 중복되지 않도록함
		for (int i = 0; i < m_Parents.Length; i++)
		{
			if (m_Parents[i] == this) continue;
			m_Parents[i].m_Sprites = m_Parents[i].m_Sprites.Except(m_Sprites).ToList();
			m_Parents[i].m_Meshs = m_Parents[i].m_Meshs.Except(m_Meshs).ToList();
			m_Parents[i].m_TextPros = m_Parents[i].m_TextPros.Except(m_TextPros).ToList();
			m_Parents[i].m_Particles = m_Parents[i].m_Particles.Except(m_Particles).ToList();
			//for (int j = m_Sprites.Count - 1; j > -1; j--) m_Parents[i].m_Sprites.Remove(m_Sprites[j]);
			//for (int j = m_Meshs.Count - 1; j > -1; j--) m_Parents[i].m_Meshs.Remove(m_Meshs[j]);
			//for (int j = m_TextPros.Count - 1; j > -1; j--) m_Parents[i].m_TextPros.Remove(m_TextPros[j]);
		}

		RenderAlpha_Controller[] Child = GetComponentsInChildren<RenderAlpha_Controller>(true);
		for (int i = 0; i < Child.Length; i++)
		{
			RenderAlpha_Controller render = Child[i];
			if (render == this) continue;
			m_Sprites = m_Sprites.Except(render.m_Sprites).ToList();
			m_Meshs = m_Meshs.Except(render.m_Meshs).ToList();
			m_TextPros = m_TextPros.Except(render.m_TextPros).ToList();
			m_Particles = m_Particles.Except(render.m_Particles).ToList();
			//for (int j = render.m_Sprites.Count - 1; j > -1; j--) m_Sprites.Remove(render.m_Sprites[j]);
			//for (int j = render.m_Meshs.Count - 1; j > -1; j--) m_Meshs.Remove(render.m_Meshs[j]);
			//for (int j = render.m_TextPros.Count - 1; j > -1; j--) m_TextPros.Remove(render.m_TextPros[j]);
		}
	}

#if UNITY_2019_1_OR_NEWER
	void OnPreRender_URP(ScriptableRenderContext context, Camera cam)
	{
		OnCamPreRender(cam);
	}
	void OnPostRender_URP(ScriptableRenderContext context, Camera cam)
	{
		OnCamPostRender(cam);
	}
#endif
	float GetRealAlpha()
	{
		float realalpah = 1f;
		for (int i = 0; i < m_Parents.Length; i++) realalpah *= m_Parents[i].Alpha;
		return realalpah;
	}

	private void OnCamPreRender(Camera cam)
	{
		UpdateColor();
	}

	private void LateUpdate()
	{
#if !USE_CAM_RENDER
		UpdateColor();
#endif
		float realalpah = GetRealAlpha();
		for (int i = m_TextPros.Count - 1; i > -1; i--)
		{
			TextMeshPro txtpro = m_TextPros[i];
			if (!txtpro.enabled || !txtpro.gameObject.activeSelf) continue;
			Utile_Class.TextMeshProAlphaChange(txtpro, realalpah);
			////txtpro.GetComponent<MeshRenderer>().material.color = color;
			////for(int j = txtpro.transform.childCount - 1; j > -1; j--)
			////{
			////	txtpro.transform.GetChild(i).GetComponent<MeshRenderer>().material.color = color;
			////}
			//var mats = txtpro.fontMaterials;
			//if (mats != null || mats.Length > 0)
			//{
			//	for (int j = mats.Length - 1; j > -1; j--)
			//	{
			//		mats[j].color = color;
			//		//txtpro.fontMaterials[j].SetColor("_Color", color);
			//		//subs[j].material?.SetColor("_Color", color);
			//	}
			//}
		}
	}

	void UpdateColor()
	{
		float realalpah = GetRealAlpha();
		m_Colors.Clear();
		Color color = Color.white;
		color.a = realalpah;
#if USE_CAM_RENDER
		for (int i = m_Meshs.Count - 1; i > -1; i--)
		{
			MeshRenderer mesh = m_Meshs[i];
			if (mesh == null) continue;
			if (!mesh.enabled) continue;
			if (!mesh.gameObject.activeInHierarchy) continue;
			if (!mesh.gameObject.activeSelf) continue;
			mesh.sharedMaterial.color = color;
			//mesh.material?.SetColor("_Color", color);
		}
		for(int i = m_Particles.Count - 1; i > -1; i--) {
			ParticleSystem particle = m_Particles[i];
			if (particle == null) continue;
			ParticleSystem.MainModule main = particle.main;
			ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
			if (renderer == null) continue;
			if(renderer.sharedMaterial == null) continue;
			renderer.sharedMaterial.color = color;
			//main.startColor = color;
		}
#  if UNITY_EDITOR
		for (int i = m_Sprites.Count - 1; i > -1; i--)
		{
			SpriteRenderer sprite = m_Sprites[i];
			if (sprite == null) continue;
			if (!sprite.enabled) continue;
			if (!sprite.gameObject.activeInHierarchy) continue;
			if (!sprite.gameObject.activeSelf) continue;
			if (Application.isPlaying)
			{
				if (!m_Mats.ContainsKey(sprite)) m_Mats.Add(sprite, sprite.material);
				if (m_Mats[sprite] != sprite.sharedMaterial)
				{
					Debug.LogWarning("Sprite Material changed !!!!!");
					m_Mats[sprite] = sprite.material;
				}
			}
			sprite.sharedMaterial.color = color;
		}
#  else
		for (int i = m_Sprites.Count - 1; i > -1; i--)
		{
			SpriteRenderer sprite = m_Sprites[i];
			if (sprite == null) continue;
			if (!sprite.enabled) continue;
			if (!sprite.gameObject.activeInHierarchy) continue;
			if (!sprite.gameObject.activeSelf) continue;
			if (!m_Mats.ContainsKey(sprite)) m_Mats.Add(sprite, sprite.material);
			if (m_Mats[sprite] != sprite.sharedMaterial)
			{
				Debug.LogWarning("Sprite Material changed !!!!!");
				m_Mats[sprite] = sprite.material;
			}
			sprite.sharedMaterial.color = color;
			//color = sprite.color;
			//m_Colors.Add(sprite, color);
			//color.a *= realalpah;
			//sprite.color = color;
		}
#  endif
#else
#  if UNITY_EDITOR
		if (!Application.isPlaying)
		{
			for (int i = m_Sprites.Count - 1; i > -1; i--)
			{
				SpriteRenderer sprite = m_Sprites[i];
			if (sprite == null) continue;
				if (!sprite.gameObject.activeInHierarchy) continue;
				color = sprite.color;
				m_Colors.Add(sprite, color);
				color.a *= realalpah;
				sprite.color = color;
			}
		}
#  endif

		if (Mathf.Abs(m_NowAlpha - realalpah) < 0.01f) return;
		m_NowAlpha = realalpah;
#  if UNITY_EDITOR
		if (Application.isPlaying)
		{
#  endif
			for (int i = m_Sprites.Count - 1; i > -1; i--)
			{
				SpriteRenderer sprite = m_Sprites[i];
				//if (!sprite.enabled || !sprite.gameObject.activeSelf) continue;
				if (!m_Mats.ContainsKey(sprite)) m_Mats.Add(sprite, sprite.material);
				if(m_Mats[sprite] != sprite.sharedMaterial)
				{
					Debug.LogWarning("Sprite Material changed !!!!!");
					m_Mats[sprite] = sprite.material;
				}
				sprite.sharedMaterial.color = color;
				//mesh.material?.SetColor("_Color", color);
			}
#  if UNITY_EDITOR
		}
#  endif

		for (int i = m_Meshs.Count - 1; i > -1; i--)
		{
			MeshRenderer mesh = m_Meshs[i];
			if (!mesh.enabled || !mesh.gameObject.activeSelf) continue;
			mesh.sharedMaterial.color = color;
			//mesh.material?.SetColor("_Color", color);
		}

		//for (int i = m_TextPros.Count - 1; i > -1; i--)
		//{
		//	TextMeshPro txtpro = m_TextPros[i];
		//	if (!txtpro.enabled || !txtpro.gameObject.activeSelf) continue;
		//	Utile_Class.TextMeshProAlphaChange
		//	//txtpro.GetComponent<MeshRenderer>().material.color = color;
		//	//for(int j = txtpro.transform.childCount - 1; j > -1; j--)
		//	//{
		//	//	txtpro.transform.GetChild(i).GetComponent<MeshRenderer>().material.color = color;
		//	//}
		//	var mats = txtpro.fontMaterials;
		//	if (mats != null || mats.Length > 0)
		//	{
		//		for (int j = mats.Length - 1; j > -1; j--)
		//		{
		//			mats[j].color = color;
		//			//txtpro.fontMaterials[j].SetColor("_Color", color);
		//			//subs[j].material?.SetColor("_Color", color);
		//		}
		//	}
		//}

		//for (int i = m_TextPros.Count - 1; i > -1; i--)
		//{
		//	TextMeshPro txtpro = m_TextPros[i];
		//	if (!txtpro.enabled || !txtpro.gameObject.activeSelf) continue;
		//	Utile_Class.TextMeshProAlphaChange(txtpro, realalpah);
		//	//color = txtpro.color;
		//	//m_Colors.Add(txtpro, color);
		//	//color.a *= realalpah;
		//	//txtpro.faceColor = color;

		//	//if (txtpro.fontMaterials != null || txtpro.fontMaterials.Length > 0)
		//	//{
		//	//	for (int j = txtpro.fontMaterials.Length - 1; j > -1; j--)
		//	//	{
		//	//		//txtpro.fontMaterials[j].color = color;
		//	//		txtpro.fontMaterials[j].SetColor("_Color", color);
		//	//		//subs[j].material?.SetColor("_Color", color);
		//	//	}
		//	//}
		//}
#endif
	}

	private void OnCamPostRender(Camera cam)
	{
		//if (m_Colors.Count < 1) return;
		//var keys = new List<Component>(m_Colors.Keys).ToArray();
		//for (int i = keys.Length - 1; i > -1; i--)
		//{
		//	Component key = keys[i];
		//	((SpriteRenderer)key).color = m_Colors[key];
		//}
		//m_Colors.Clear();
		//for (int i = m_TextPros.Count - 1; i > -1; i--)
		//{
		//	TextMeshPro txtpro = m_TextPros[i];
		//	if (!m_Colors.ContainsKey(txtpro)) continue;
		//	txtpro.faceColor = m_Colors[txtpro];
		//}
	}
}
