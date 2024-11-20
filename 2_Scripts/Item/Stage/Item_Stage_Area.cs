using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_Stage_Area : ObjMng
{
	[SerializeField] Color inColor = Color.yellow;
	[SerializeField] Color[] outColor = new Color[2];
	[SerializeField] Material m_Material;
	[SerializeField] float m_OutSize = 0.2f;
	float m_CardW = 5.12f;
	float m_CardH = 6.50f;
	List<Vector3> m_OutList = new List<Vector3>();
	List<Vector3> m_InList = new List<Vector3>();
	List<Item_Stage> m_Cards = new List<Item_Stage>();

	void CreateLineMaterial()
	{
		if (!m_Material)
		{
			// Unity has a built-in shader that is useful for drawing
			// simple colored things.
			Shader shader = Shader.Find("Hidden/Internal-Colored");
			//Shader shader = Shader.Find ("Custom/GridColored");
			m_Material = new Material(shader);
			m_Material.hideFlags = HideFlags.HideAndDontSave;

			// Turn on alpha blending
			m_Material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			m_Material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);//기본 
			//m_pLineMaterial.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);//이펙트 같은 블렌딩
			// Turn backface culling off
			m_Material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
			// Turn off depth writes
			m_Material.SetInt("_ZWrite", 0);
		}
	}

	public void Clear()
	{
		gameObject.SetActive(false);
		m_OutList.Clear();
		m_InList.Clear();
	}

	public void Show()
	{
		gameObject.SetActive(true);
	}

	Item_Stage ISArea(EDIR dir, Item_Stage card, List<Item_Stage> AreaCards)
	{
		int line = card.m_Line;
		int pos = card.m_Pos;
		switch(dir)
		{
		case EDIR.UP:
			line++;
			pos++;
			break;
		case EDIR.LEFT:
			pos--;
			break;
		case EDIR.DOWN:
			line--;
			pos--;
			break;
		case EDIR.RIGHT:
			pos++;
			break;
		case EDIR.UP_LEFT:
			line++;
			break;
		case EDIR.DOWN_LEFT:
			line--;
			pos-=2;
			break;
		case EDIR.DOWN_RIGHT:
			line--;
			break;
		case EDIR.UP_RIGHT:
			line++;
			pos+=2;
			break;
		default: return null;
		}

		for (int i = AreaCards.Count - 1; i > -1; i--)
		{
			Item_Stage temp = AreaCards[i];
			if (temp.m_Line == line && temp.m_Pos == pos) return temp;
		}
		return null;
	}

	public Vector3[] GetVertex(Item_Stage card)
	{
		Vector3[] av3Ver = new Vector3[4];
		float SX = card.transform.localScale.x;
		float SY = card.transform.localScale.y;
		float SZ = card.transform.localScale.z;
		float X = card.transform.localPosition.x;
		float Y = card.transform.localPosition.y;
		float Z = card.transform.localPosition.z;
		float GX = m_CardW * SX * 0.5f;
		float GY = m_CardH * SY * 0.5f;
		av3Ver[0] = new Vector3(X - GX, Y - GY, Z); // DOWN, LEFT
		av3Ver[1] = new Vector3(X - GX, Y + GY, Z); // UP, LEFT
		av3Ver[2] = new Vector3(X + GX, Y + GY, Z); // UP, RIGHT
		av3Ver[3] = new Vector3(X + GX, Y - GY, Z); // DOWN, RIGHT
		return av3Ver;
	}


	float GetValueX(float Gap, float Angle)
	{
		return Mathf.Cos(Angle * Mathf.Deg2Rad) * Gap;
	}
	float GetValueY(float Gap, float Angle)
	{
		return Mathf.Sin(Angle * Mathf.Deg2Rad) * Gap;
	}

	public void AddCard(List<Item_Stage> AreaCards)
	{
		Item_Stage UpCard = null;
		Item_Stage LeftCard = null;
		Item_Stage DownCard = null;
		Item_Stage RightCard = null;

		int Cnt = 5;
		float Angle = 90f / Cnt;

		float OG = m_OutSize;
		float XOG, YOG;

		for (int i = AreaCards.Count - 1; i > -1; i--)
		{
			Item_Stage card = AreaCards[i];

			// 해당 위치에서 상하좌우 카드가 있는지 확인
			UpCard = ISArea(EDIR.UP, card, AreaCards);
			LeftCard = ISArea(EDIR.LEFT, card, AreaCards);
			DownCard = ISArea(EDIR.DOWN, card, AreaCards);
			RightCard = ISArea(EDIR.RIGHT, card, AreaCards);
			//	1	2
			//	0	3
			Vector3[] vertex = GetVertex(card);

			// 안쪽 사각 만들기
			m_InList.Add(vertex[0]);
			m_InList.Add(vertex[1]);
			m_InList.Add(vertex[2]);
			m_InList.Add(vertex[3]);

			// 아웃 라인 만들기
			// 위쪽
			if (!UpCard)
			{
				Vector3[] line = new Vector3[4];
				// 왼쪽에 카드가 없을때
				// 왼쪽 세로
				Item_Stage ULCard = ISArea(EDIR.UP_LEFT, card, AreaCards);
				if (ULCard)
				{
					// 둘중 하나라도 있으면 YOG값이 정해져야한다.
					// 위 아래의 사이즈르 다시 측정해서 빈 간격을 알아낸다.
					// 하나의 연결된 영역으로 측정해야하므로
					Vector3[] Temp_vertex = GetVertex(ULCard);
					// 회전이 있는게 아니므로
					XOG = vertex[1].x - Temp_vertex[3].x;
				}
				else XOG = OG;
				float GapW = (OG - XOG);
				line[0] = vertex[1];
				line[0].x = vertex[1].x + GapW;
				line[0].y = vertex[1].y + OG;

				line[3] = vertex[1];
				line[3].x = vertex[1].x + GapW;

				// 갭만큼 올려주기
				// 0	1
				// 3	2

				Item_Stage URCard = ISArea(EDIR.UP_RIGHT, card, AreaCards);
				if (URCard)
				{
					// 둘중 하나라도 있으면 YOG값이 정해져야한다.
					// 위 아래의 사이즈르 다시 측정해서 빈 간격을 알아낸다.
					// 하나의 연결된 영역으로 측정해야하므로
					Vector3[] Temp_vertex = GetVertex(URCard);
					// 회전이 있는게 아니므로
					XOG = Temp_vertex[0].x - vertex[2].x;
				}
				else XOG = OG;
				GapW = (OG - XOG);
				line[1] = vertex[2];
				line[1].x = vertex[2].x - GapW;
				line[1].y = vertex[2].y + OG;

				line[2] = vertex[2];
				line[2].x = vertex[2].x - GapW;

				// 1 - G, 1			1, 1
				// 0 - G, 0			0, 0
				// 1				2
				// 0				3
				m_OutList.Add(line[0]);
				m_OutList.Add(line[1]);
				m_OutList.Add(line[2]);
				m_OutList.Add(line[3]);
				if (ULCard)
				{
					// 해당 지점에 카드가 있으면 라운드를 반대로
					float a = 0f;
					for (int j = 0; j < Cnt; j++)
					{
						m_OutList.Add(line[0]);
						m_OutList.Add(line[0]);
						m_OutList.Add(new Vector3(line[0].x - GetValueX(OG, a), line[0].y - GetValueY(OG, a), line[0].z));
						a += Angle;
						m_OutList.Add(new Vector3(line[0].x - GetValueX(OG, a), line[0].y - GetValueY(OG, a), line[0].z));
					}
				}
				else if (!LeftCard)
				{
					float a = 0f;
					for (int j = 0; j < Cnt; j++)
					{
						m_OutList.Add(new Vector3(line[3].x - GetValueY(OG, a), line[3].y + GetValueX(OG, a), line[3].z));
						a += Angle;
						m_OutList.Add(new Vector3(line[3].x - GetValueY(OG, a), line[3].y + GetValueX(OG, a), line[3].z));
						m_OutList.Add(line[3]);
						m_OutList.Add(line[3]);
					}
				}
				else
				{
					Vector3[] tempvertex = GetVertex(LeftCard);
					float gapW = Mathf.Abs(vertex[1].x - tempvertex[2].x);

					m_OutList.Add(new Vector3(line[0].x - gapW, line[0].y, line[0].z));
					m_OutList.Add(line[0]);
					m_OutList.Add(line[3]);
					m_OutList.Add(new Vector3(line[3].x - gapW, line[3].y, line[3].z));
				}
			}

			// 왼쪽
			if (!LeftCard)
			{
				// 1	2
				// 0	3

				// 왼쪽에 카드가 없을때
				// 왼쪽아래 모서리
				Item_Stage DLCard = ISArea(EDIR.DOWN_LEFT, card, AreaCards);
				if (DLCard)
				{
					// 둘중 하나라도 있으면 YOG값이 정해져야한다.
					// 위 아래의 사이즈르 다시 측정해서 빈 간격을 알아낸다.
					// 하나의 연결된 영역으로 측정해야하므로
					Vector3[] Temp_vertex = GetVertex(DLCard);
					// 회전이 있는게 아니므로
					YOG = vertex[0].y - Temp_vertex[2].y;
				}
				else YOG = OG;
				float GapH = (OG - YOG);
				// 갭만큼 올려주기
				Vector3[] line = new Vector3[4];
				line[0] = vertex[0];
				line[0].x = vertex[0].x - OG;
				line[0].y = vertex[0].y + GapH;

				line[3] = vertex[0];
				line[3].y = vertex[0].y + GapH;

				Item_Stage ULCard = ISArea(EDIR.UP_LEFT, card, AreaCards);
				if (ULCard)
				{
					// 둘중 하나라도 있으면 YOG값이 정해져야한다.
					// 위 아래의 사이즈르 다시 측정해서 빈 간격을 알아낸다.
					// 하나의 연결된 영역으로 측정해야하므로
					Vector3[] Temp_vertex = GetVertex(ULCard);
					// 회전이 있는게 아니므로
					YOG = Temp_vertex[3].y - vertex[1].y;
				}
				else YOG = OG;

				GapH = (OG - YOG);
				line[1] = vertex[1];
				line[1].x = vertex[1].x - OG;
				line[1].y = vertex[1].y - GapH;

				line[2] = vertex[1];
				line[2].y = vertex[1].y - GapH;

				// 1 - G, 1			1, 1
				// 0 - G, 0			0, 0
				// 1				2
				// 0				3
				m_OutList.Add(line[0]);
				m_OutList.Add(line[1]);
				m_OutList.Add(line[2]);
				m_OutList.Add(line[3]);
				if (DLCard)
				{
					// 해당 지점에 카드가 있으면 라운드를 반대로

					float a = 0f;
					for (int j = 0; j < Cnt; j++)
					{
						m_OutList.Add(line[0]);
						m_OutList.Add(line[0]);
						m_OutList.Add(new Vector3(line[0].x + GetValueX(OG, a), line[0].y - GetValueY(OG, a), line[0].z));
						a += Angle;
						m_OutList.Add(new Vector3(line[0].x + GetValueX(OG, a), line[0].y - GetValueY(OG, a), line[0].z));
					}
				}
				else if (!DownCard)
				{
					float a = 90f;
					for (int j = 0; j < Cnt; j++)
					{
						m_OutList.Add(new Vector3(line[3].x - GetValueX(OG, a), line[3].y - GetValueY(OG, a), line[3].z));
						a -= Angle;
						m_OutList.Add(new Vector3(line[3].x - GetValueX(OG, a), line[3].y - GetValueY(OG, a), line[3].z));
						m_OutList.Add(line[3]);
						m_OutList.Add(line[3]);
					}
				}
				else
				{
					Vector3[] tempvertex = GetVertex(DownCard);
					float gapH = Mathf.Abs(vertex[0].y - tempvertex[2].y);

					m_OutList.Add(new Vector3(line[0].x, line[0].y - gapH, line[0].z));
					m_OutList.Add(line[0]);
					m_OutList.Add(line[3]);
					m_OutList.Add(new Vector3(line[3].x, line[3].y - gapH, line[3].z));

				}
			}
			// 아래
			if (!DownCard)
			{
				Vector3[] line = new Vector3[4];
				// 왼쪽에 카드가 없을때
				// 왼쪽 세로
				// 갭만큼 올려주기
				// 3	2
				// 0	1


				Item_Stage DLCard = ISArea(EDIR.DOWN_LEFT, card, AreaCards);
				if (DLCard)
				{
					// 둘중 하나라도 있으면 YOG값이 정해져야한다.
					// 위 아래의 사이즈르 다시 측정해서 빈 간격을 알아낸다.
					// 하나의 연결된 영역으로 측정해야하므로
					Vector3[] Temp_vertex = GetVertex(DLCard);
					// 회전이 있는게 아니므로
					XOG = vertex[0].x - Temp_vertex[2].x;
				}
				else XOG = OG;
				float GapW = (OG - XOG);

				line[0] = vertex[0];
				line[0].x = vertex[0].x + GapW;
				line[0].y = vertex[0].y - OG;

				line[3] = vertex[0];
				line[3].x = vertex[0].x + GapW;

				Item_Stage DRCard = ISArea(EDIR.DOWN_RIGHT, card, AreaCards);
				if (DRCard)
				{
					// 둘중 하나라도 있으면 YOG값이 정해져야한다.
					// 위 아래의 사이즈르 다시 측정해서 빈 간격을 알아낸다.
					// 하나의 연결된 영역으로 측정해야하므로
					Vector3[] Temp_vertex = GetVertex(DRCard);
					// 회전이 있는게 아니므로
					XOG = Temp_vertex[1].x - vertex[3].x;
				}
				else XOG = OG;
				GapW = (OG - XOG);

				line[1] = vertex[3];
				line[1].x = vertex[3].x - GapW;
				line[1].y = vertex[3].y - OG;

				line[2] = vertex[3];
				line[2].x = vertex[3].x - GapW;

				m_OutList.Add(line[0]);
				m_OutList.Add(line[1]);
				m_OutList.Add(line[2]);
				m_OutList.Add(line[3]);
				if (DRCard)
				{
					// 해당 지점에 카드가 있으면 라운드를 반대로
					float a = 0f;
					for (int j = 0; j < Cnt; j++)
					{
						m_OutList.Add(line[1]);
						m_OutList.Add(line[1]);
						m_OutList.Add(new Vector3(line[1].x + GetValueX(OG, a), line[1].y + GetValueY(OG, a), line[1].z));
						a += Angle;
						m_OutList.Add(new Vector3(line[1].x + GetValueX(OG, a), line[1].y + GetValueY(OG, a), line[1].z));
					}
				}
				else if (!RightCard)
				{
					float a = 0f;
					for (int j = 0; j < Cnt; j++)
					{
						m_OutList.Add(new Vector3(line[2].x + GetValueX(OG, a), line[2].y - GetValueY(OG, a), line[2].z));
						a += Angle;
						m_OutList.Add(new Vector3(line[2].x + GetValueX(OG, a), line[2].y - GetValueY(OG, a), line[2].z));
						m_OutList.Add(line[2]);
						m_OutList.Add(line[2]);
					}
				}
				else
				{
					Vector3[] tempvertex = GetVertex(RightCard);
					float gapW = Mathf.Abs(vertex[3].x - tempvertex[0].x);

					m_OutList.Add(line[1]);
					m_OutList.Add(new Vector3(line[1].x + gapW, line[1].y, line[1].z));
					m_OutList.Add(new Vector3(line[2].x + gapW, line[2].y, line[2].z));
					m_OutList.Add(line[2]);
				}
			}
			// 왼쪽
			if (!RightCard)
			{
				// 2	1
				// 3	0
				// 왼쪽에 카드가 없을때
				// 왼쪽아래 모서리
				Item_Stage DRCard = ISArea(EDIR.DOWN_RIGHT, card, AreaCards);
				if (DRCard)
				{
					// 둘중 하나라도 있으면 YOG값이 정해져야한다.
					// 위 아래의 사이즈르 다시 측정해서 빈 간격을 알아낸다.
					// 하나의 연결된 영역으로 측정해야하므로
					Vector3[] Temp_vertex = GetVertex(DRCard);
					// 회전이 있는게 아니므로
					YOG = vertex[3].y - Temp_vertex[1].y;
				}
				else YOG = OG;
				float GapH = (OG - YOG);
				// 갭만큼 올려주기
				Vector3[] line = new Vector3[4];
				line[0] = vertex[3];
				line[0].x = vertex[3].x + OG;
				line[0].y = vertex[3].y + GapH;

				line[3] = vertex[3];
				line[3].y = vertex[3].y + GapH;

				Item_Stage URCard = ISArea(EDIR.UP_RIGHT, card, AreaCards);
				if (URCard)
				{
					// 둘중 하나라도 있으면 YOG값이 정해져야한다.
					// 위 아래의 사이즈르 다시 측정해서 빈 간격을 알아낸다.
					// 하나의 연결된 영역으로 측정해야하므로
					Vector3[] Temp_vertex = GetVertex(URCard);
					// 회전이 있는게 아니므로
					YOG = Temp_vertex[0].y - vertex[2].y;
				}
				else YOG = OG;

				GapH = (OG - YOG);
				line[1] = vertex[2];
				line[1].x = vertex[2].x + OG;
				line[1].y = vertex[2].y - GapH;

				line[2] = vertex[2];
				line[2].y = vertex[2].y - GapH;

				m_OutList.Add(line[0]);
				m_OutList.Add(line[1]);
				m_OutList.Add(line[2]);
				m_OutList.Add(line[3]);

				if (URCard)
				{
					// 해당 지점에 카드가 있으면 라운드를 반대로

					float a = 0f;
					for (int j = 0; j < Cnt; j++)
					{
						m_OutList.Add(line[1]);
						m_OutList.Add(line[1]);
						m_OutList.Add(new Vector3(line[1].x - GetValueX(OG, a), line[1].y + GetValueY(OG, a), line[1].z));
						a += Angle;
						m_OutList.Add(new Vector3(line[1].x - GetValueX(OG, a), line[1].y + GetValueY(OG, a), line[1].z));
					}
				}
				else if (!UpCard)
				{
					float a = 90f;
					for (int j = 0; j < Cnt; j++)
					{
						m_OutList.Add(new Vector3(line[2].x + GetValueX(OG, a), line[2].y + GetValueY(OG, a), line[2].z));
						a -= Angle;
						m_OutList.Add(new Vector3(line[2].x + GetValueX(OG, a), line[2].y + GetValueY(OG, a), line[2].z));
						m_OutList.Add(line[2]);
						m_OutList.Add(line[2]);
					}
				}
				else
				{
					Vector3[] tempvertex = GetVertex(UpCard);
					float gapH = tempvertex[3].y - vertex[2].y;

					m_OutList.Add(line[1]);
					m_OutList.Add(new Vector3(line[1].x, line[1].y + gapH, line[1].z));
					m_OutList.Add(new Vector3(line[2].x, line[2].y + gapH, line[2].z));
					m_OutList.Add(line[2]);
				}
			}

		}
	}


	public void OnRenderObject()
	{
		CreateLineMaterial();
		m_Material.SetPass(0);

		GL.PushMatrix();
		GL.MultMatrix(transform.localToWorldMatrix);

		GL.Begin(GL.QUADS);

		GL.Color(inColor);
		int nMax = m_InList.Count;
		for (int i = 0; i < nMax; i++) GL.Vertex(m_InList[i]);

		nMax = m_OutList.Count;
		for (int i = 0; i < nMax; i += 4)
		{
			GL.Color(outColor[1]);
			GL.Vertex(m_OutList[i]);
			GL.Vertex(m_OutList[i + 1]);
			GL.Color(outColor[0]);
			GL.Vertex(m_OutList[i + 2]);
			GL.Vertex(m_OutList[i + 3]);
		}

		GL.Color(Color.white);

		GL.End();
		GL.PopMatrix();
	}
}
