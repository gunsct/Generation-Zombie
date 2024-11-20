using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(SortingGroup))]
public class DarkLightRenderer : MonoBehaviour
{
    [SerializeField] Sprite LightImage;
    [SerializeField] Color Color = Color.white;
    Material mat;
    MeshRenderer _mr = null;
    MeshRenderer m_MR
    {
        get
        {
            if (_mr == null) _mr = GetComponent<MeshRenderer>();
            return _mr;
        }
    }

    MeshFilter _mf = null;
    MeshFilter m_MF
    {
        get
        {
            if (_mf == null) _mf = GetComponent<MeshFilter>();
            return _mf;
        }
    }

    Mesh _mesh = null;
    Mesh m_Mesh
    {
        get
        {
            if (_mesh == null)
            {
                _mesh = new Mesh();
                int[] tri = new int[6];
                Vector3[] ver = new Vector3[4];
                Vector2[] uv = new Vector2[4];
                Color[] colors = new Color[4];

                colors[3] = colors[2] = colors[1] = colors[0] = Color.white;
                // vertex
                ver[0] = new Vector3(-0.5f, -0.5f, 0);
                ver[1] = new Vector3(0.5f, -0.5f, 0);
                ver[2] = new Vector3(-0.5f, 0.5f, 0);
                ver[3] = new Vector3(0.5f, 0.5f, 0);

                // triangles
                // 1 (0, 2, 1)
                tri[0] = 0;
                tri[1] = 2;
                tri[2] = 1;
                // 2 (2, 3, 1)
                tri[3] = 2;
                tri[4] = 3;
                tri[5] = 1;

                // uv0
                uv[0] = new Vector2(0, 0);
                uv[1] = new Vector2(1, 0);
                uv[2] = new Vector2(0, 1);
                uv[3] = new Vector2(1, 1);

                _mesh.name = "rendermesh";
                m_Mesh.vertices = ver;
                m_Mesh.triangles = tri;
                m_Mesh.colors = colors;
                m_Mesh.uv = uv;

                m_MF.mesh = m_Mesh;
            }
            return _mesh;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        Shader shader = Shader.Find("Shader Graphs/DarkLightRenderer");
        mat = new Material(shader);
        m_MR.sharedMaterial = mat;
    }

    // Update is called once per frame
    void Update()
    {
        SetData();
    }

    void SetData()
    {
        Camera cam = Camera.allCameras[1];

        // 회전 각도 풀어주기
        transform.eulerAngles = Vector3.zero;

        if (LightImage != null) m_MR.sharedMaterial.SetTexture("_MainTex", LightImage.texture);
        m_MR.sharedMaterial.SetTexture("_RawTex", cam.targetTexture);
        m_MR.sharedMaterial.SetColor("_BaseColor", Color);


        float oneuvx = 1f / cam.targetTexture.width;
        float oneuvy = 1f / cam.targetTexture.height;

        float scaleX = transform.lossyScale.x;
        float scaleY = transform.lossyScale.y;
        // 카메라의 좌표 알아내기
        Vector2 LD = cam.WorldToScreenPoint(transform.position + new Vector3(-0.5f * scaleX, -0.5f * scaleY, 0f)); // Left Down
        Vector2 LT = cam.WorldToScreenPoint(transform.position + new Vector3(-0.5f * scaleX, 0.5f * scaleY, 0f)); // Left Top
        Vector2 RD = cam.WorldToScreenPoint(transform.position + new Vector3(0.5f * scaleX, -0.5f * scaleY, 0f)); // Right Down
        Vector2 RT = cam.WorldToScreenPoint(transform.position + new Vector3(0.5f * scaleX, 0.5f * scaleY, 0f)); // Right Top

        Vector2[] uv = new Vector2[4];
        uv[0] = new Vector2(LD.x * oneuvx, LD.y * oneuvy); // Left Down
        uv[1] = new Vector2(RD.x * oneuvx, RD.y * oneuvy); // Right Down
        uv[2] = new Vector2(LT.x * oneuvx, LT.y * oneuvy); // Left Down
        uv[3] = new Vector2(RT.x * oneuvx, RT.y * oneuvy); // Right Top

        m_Mesh.SetUVs(1, uv);


        // 카메라의 해당 지점 사이즈 알아내기
        //CameraViewSizeInfo size = Utile_Class.GetViewWorldSizeInfo(cam, cam.targetTexture.width, cam.targetTexture.height);

        //Vector3 min = size.origin[0] + size.direction[0] / size.direction[0].z * Mathf.Abs(size.origin[0].z);
        //Vector3 max = size.origin[1] + size.direction[1] / size.direction[1].z * Mathf.Abs(size.origin[1].z);

        //float camwidth = Mathf.Abs(min.x) + Mathf.Abs(max.x);
        //float camheight = Mathf.Abs(min.y) + Mathf.Abs(max.y);
        //// 
        //float gapx = mypos.x;
        //float gapy = mypos.y;


        //float lx = (gapx - 0.5f * scaleX) * oneuvx;
        //float rx = (gapx + 0.5f * scaleX) * oneuvx;
        //float dy = (gapy - 0.5f * scaleY) * oneuvy;
        //float uy = (gapy + 0.5f * scaleY) * oneuvy;


        //Vector2[] uv = new Vector2[4];
        //uv[0] = new Vector2(lx, dy);
        //uv[1] = new Vector2(rx, dy);
        //uv[2] = new Vector2(lx, uy);
        //uv[3] = new Vector2(rx, uy);

        // 카메라 영역 디버깅
        //Vector3 Start = size.origin[0];
        //Vector3 End = min;
        //Debug.DrawLine(Start, End, Color.yellow);
        //Start = size.origin[1];
        //End = max;
        //Debug.DrawLine(Start, End, Color.red);
        //Start = new Vector3(size.origin[0].x, size.origin[1].y, size.origin[1].z);
        //End = new Vector3(min.x, max.y, max.z);
        //Debug.DrawLine(Start, End, Color.blue);
        //Start = new Vector3(size.origin[1].x, size.origin[0].y, size.origin[0].z);
        //End = new Vector3(max.x, min.y, min.z);
        //Debug.DrawLine(Start, End, Color.green);
    }
}
