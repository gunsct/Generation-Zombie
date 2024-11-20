using System;
using System.Collections;
using UnityEngine;

public class Item_StatsRadarChart : ObjMng
{
    [SerializeField] private Material radarMaterial;
    private Stats _stats;
    public CanvasRenderer radarMeshCanvasRenderer;
    public Vector2 offset;
    private bool isLineDraw = false;

    // LineDraw
    [SerializeField] private Transform[] arrStatTransform;

    public void SetStats(Stats stats)
    {
        _stats = stats;
        stats.EventStatsChanged += OnStatsChanged;
       
        Refresh();
    }

    private void OnStatsChanged(object sender, EventArgs e)
    {
        Refresh();
    }

    public float bgSize = 200f;

    public void Refresh()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
        StartCoroutine(CoRefreshChart());
    }

    private IEnumerator CoRefreshChart()
    {
        if (isLineDraw)
        {
            arrStatTransform[0].localScale = new Vector3(1, _stats.GetStatAmountNormalized(StatType.HP));
            arrStatTransform[1].localScale = new Vector3(1, _stats.GetStatAmountNormalized(StatType.Atk));
            arrStatTransform[2].localScale = new Vector3(1, _stats.GetStatAmountNormalized(StatType.Def));
            arrStatTransform[3].localScale = new Vector3(1, _stats.GetStatAmountNormalized(StatType.Heal));
            arrStatTransform[4].localScale = new Vector3(1, _stats.GetStatAmountNormalized(StatType.Speed));
        }

        var rectTransform = radarMeshCanvasRenderer.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = offset;

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[6];
        Vector2[] uv = new Vector2[6];
        int[] triangles = new int[3 * 5];

        float angleIncrement = 360f / 5;
        float radarBgSize = bgSize;
        Vector3[] arrVertex = new Vector3[5];
        int[] arrPos = new int[5];

        for (int i = 0; i < arrStatTransform.Length; i++)
        {
            arrStatTransform[i].rotation = Quaternion.Euler(0, 0, -angleIncrement * i);
        }

        var fillRatio = 0f;

        var originPos = radarMeshCanvasRenderer.transform.position;
        originPos.x += 0.5f;
        originPos.y += 4f;
        while (fillRatio < 1f)
        {
            fillRatio += 0.1f;
            if (fillRatio > 1f)
            {
                fillRatio = 1f;
            }
            
            arrVertex[0] = Quaternion.Euler(0, 0, -angleIncrement * 0) * Vector3.up *
                           (radarBgSize * _stats.GetStatAmountNormalized(StatType.HP) * fillRatio);
            arrStatTransform[0].position = originPos + arrVertex[0] * 0.9f;
            arrPos[0] = 1;
            arrVertex[1] = Quaternion.Euler(0, 0, -angleIncrement * 1) * Vector3.up *
                           (radarBgSize * _stats.GetStatAmountNormalized(StatType.Atk) * fillRatio);
            arrStatTransform[1].position = originPos + arrVertex[1] * 0.9f;
            arrPos[1] = 2;
            arrVertex[2] = Quaternion.Euler(0, 0, -angleIncrement * 2) * Vector3.up *
                           (radarBgSize * _stats.GetStatAmountNormalized(StatType.Def) * fillRatio);
            arrStatTransform[2].position = originPos + arrVertex[2] * 0.9f;
            arrPos[2] = 3;
            arrVertex[3] = Quaternion.Euler(0, 0, -angleIncrement * 3) * Vector3.up *
                           (radarBgSize * _stats.GetStatAmountNormalized(StatType.Heal) * fillRatio);
            arrStatTransform[3].position = originPos + arrVertex[3] * 0.9f;
            arrPos[3] = 4;
            arrVertex[4] = Quaternion.Euler(0, 0, -angleIncrement * 4) * Vector3.up *
                           (radarBgSize * _stats.GetStatAmountNormalized(StatType.Speed) * fillRatio);
            arrStatTransform[4].position = originPos + arrVertex[4] * 0.9f;
            arrPos[4] = 5;

            vertices[0] = Vector3.zero;
            vertices[arrPos[0]] = arrVertex[0];
            vertices[arrPos[1]] = arrVertex[1];
            vertices[arrPos[2]] = arrVertex[2];
            vertices[arrPos[3]] = arrVertex[3];
            vertices[arrPos[4]] = arrVertex[4];

            triangles[0] = 0;
            triangles[1] = arrPos[0];
            triangles[2] = arrPos[1];

            triangles[3] = 0;
            triangles[4] = arrPos[1];
            triangles[5] = arrPos[2];

            triangles[6] = 0;
            triangles[7] = arrPos[2];
            triangles[8] = arrPos[3];

            triangles[9] = 0;
            triangles[10] = arrPos[3];
            triangles[11] = arrPos[4];

            triangles[12] = 0;
            triangles[13] = arrPos[4];
            triangles[14] = arrPos[0];

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;

            radarMeshCanvasRenderer.SetMesh(mesh);
            radarMeshCanvasRenderer.SetMaterial(radarMaterial, null);   
            
            yield return new WaitForSeconds(0.03f);
        }
    }
}

[Serializable]
public class Stats
{
    public event EventHandler EventStatsChanged;

    public const int STAT_MIN = 0;
    public const int STAT_MAX = 50;

    public SingleStat m_HP;
    public SingleStat m_Atk;
    public SingleStat m_Def;
    public SingleStat m_Heal;
    public SingleStat m_Speed;

    public Stats()
    {
        m_HP = new SingleStat(0);
        m_Atk = new SingleStat(0);
        m_Def = new SingleStat(0);
        m_Heal = new SingleStat(0);
        m_Speed = new SingleStat(0);
    }

    private SingleStat GetSingleStat(StatType StatType)
    {
        switch (StatType)
        {
            case StatType.HP:
                return m_HP;
            case StatType.Atk:
                return m_Atk;
            case StatType.Def:
                return m_Def;
            case StatType.Heal:
                return m_Heal;
            case StatType.Speed:
                return m_Speed;
        }

        return null;
    }

    public void SetStatAmount(StatType StatType, int value)
    {
        GetSingleStat(StatType).SetStatAmount(value);
        EventStatsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetStatAmount(StatType StatType)
    {
        return GetSingleStat(StatType).GetStatAmount();
    }

    public float GetStatAmountNormalized(StatType StatType)
    {
        return GetSingleStat(StatType).GetStatAmountNormalized();
    }

    [Serializable]
    public class SingleStat
    {
        [SerializeField] private int stat;

        public SingleStat(int value)
        {
            SetStatAmount(value);
        }

        public void SetStatAmount(int value)
        {
            stat = value;
        }

        public int GetStatAmount()
        {
            return stat;
        }

        public float GetStatAmountNormalized()
        {
            if (stat == 0)
                return 0;

            return (float) stat / STAT_MAX;
        }

        public override string ToString()
        {
            return stat.ToString();
        }
    }
}