using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public partial class Utile_Class
{
	/// <summary> 시그마 구간 구하기 </summary>
	/// <param name="Start"> 시작 </param>
	/// <param name="End"> 끝 </param>
	public static int Sigma(int Start, int End)
	{
		return Sigma(End) - Sigma(Start);
	}
	/// <summary> 시그마 계산식  </summary>
	public static int Sigma(int Cnt)
	{
		return Cnt * (Cnt + 1) / 2;
	}
	/// <summary>
	/// 두점 사이의 각도 구하기
	/// </summary>
	/// <param name="Start"> 시작 지점 </param>
	/// <param name="End"> 종료 지점 </param>
	/// <returns>구점 사이의 각도</returns>
	public static float GetAngle(Vector2 Start, Vector2 End)
	{
		Vector2 v2 = End - Start;
		return Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg + 180f;
	}

	// <summary>
	/// 직선/원 교차점 찾기
	/// </summary>
	/// <param name="circleCenter">원 중심점</param>
	/// <param name="circleRadius">원 반경</param>
	/// <param name="linePoint1">직선 포인트 1</param>
	/// <param name="linePoint2">직선 포인트 2</param>
	/// <param name="intersectPoint1">교차점 1</param>
	/// <param name="intersectPoint2">교차점 2</param>
	/// <returns>교차점 수</returns>
	public int FindLineCircleIntersection(Vector2 circleCenter, float circleRadius, Vector2 linePoint1, Vector2 linePoint2, out Vector2 intersectPoint1, out Vector2 intersectPoint2)
	{

		float deltaX;
		float deltaY;
		float a;
		float b;
		float c;
		float det;
		float t;

		deltaX = linePoint2.x - linePoint1.x;
		deltaY = linePoint2.y - linePoint1.y;

		a = deltaX * deltaX + deltaY * deltaY;
		b = 2 * (deltaX * (linePoint1.x - circleCenter.x) + deltaY * (linePoint1.y - circleCenter.y));
		c = (linePoint1.x - circleCenter.x) * (linePoint1.x - circleCenter.x) + (linePoint1.y - circleCenter.y) * (linePoint1.y - circleCenter.y) - circleRadius * circleRadius;

		det = b * b - 4 * a * c;
		if ((a <= 0.0000001) || (det < 0))
		{
			intersectPoint1 = new Vector2(float.NaN, float.NaN);
			intersectPoint2 = new Vector2(float.NaN, float.NaN);
			return 0;
		}
		else if (det == 0)
		{
			t = -b / (2 * a);
			intersectPoint1 = new Vector2(linePoint1.x + t * deltaX, linePoint1.y + t * deltaY);
			intersectPoint2 = new Vector2(float.NaN, float.NaN);
			return 1;
		}
		else
		{
			t = (float)((-b + Mathf.Sqrt(det)) / (2 * a));
			intersectPoint1 = new Vector2(linePoint1.x + t * deltaX, linePoint1.y + t * deltaY);

			t = (float)((-b - Mathf.Sqrt(det)) / (2 * a));
			intersectPoint2 = new Vector2(linePoint1.x + t * deltaX, linePoint1.y + t * deltaY);
			return 2;
		}

	}

	public static int NeedCnt(int need, int gap)
	{
		if (need == 0 || gap == 0) return 0;
		return (need + gap - 1) / gap;
	}

	public List<Vector2> GetMeshUV_From_Sprite(Vector2[] spriteuvs)
	{
		return GetMeshUV_From_Sprite_Static(spriteuvs);
	}

	public static List<Vector2> GetMeshUV_From_Sprite_Static(Vector2[] spriteuvs)
	{
		// 2	3
		// 0	1
		List<Vector2> uv = new List<Vector2>();
		float minX = 1f;
		float maxX = 0f;
		float minY = 1f;
		float maxY = 0f;
		for (int i = 0; i < spriteuvs.Length; i++)
		{
			float x = spriteuvs[i].x;
			float y = spriteuvs[i].y;
			minX = Mathf.Min(x, minX);
			maxX = Mathf.Max(x, maxX);
			minY = Mathf.Min(y, minY);
			maxY = Mathf.Max(y, maxY);
		}
		uv.Add(new Vector2(minX, minY));
		uv.Add(new Vector2(maxX, minY));
		uv.Add(new Vector2(minX, maxY));
		uv.Add(new Vector2(maxX, maxY));
		//uv.Add(spriteuvs[2]);
		//uv.Add(spriteuvs[3]);
		//uv.Add(spriteuvs[0]);
		//uv.Add(spriteuvs[1]);

		return uv;
	}

	public void SetLayoutGroupReset(RectTransform rtf)
	{
		var layout = rtf.GetComponent<LayoutGroup>();
		if (layout == null) return;
		var isEnable = layout.enabled;
		layout.enabled = true;
		LayoutRebuilder.ForceRebuildLayoutImmediate(rtf);
		rtf.ForceUpdateRectTransforms();

		layout.enabled = isEnable;
	}
}
