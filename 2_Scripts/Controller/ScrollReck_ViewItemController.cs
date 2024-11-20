using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// ★주의사항
// 1. ScrollRect은 반드시 vertical 이든 horizontal이든 하나만 켜져있어야함 사이즈를 계산해줄 수 없음
// 둘다켜져있을경우 LayoutGroup을 보고 꺼주긴함
// 둘다켜져있을경우 LayoutGroup을 보고 꺼주긴함
// GridLayoutGroup : vertical = false, horizontal = true (둘다 켜져있을 경우만)
// VerticalLayoutGroup : vertical = true, horizontal = false (강제 셋팅)
// HorizontalLayoutGroup : vertical = false, horizontal = true (강제 셋팅)
// 2. 리스트 아이템의 스케일을 할 경우 아이템을 스케일하는 것이 아닌 스크롤 자체를 스케일해줄 것
// 3. StartObj와 EndObj는 content에 있으면 제거되므로 ViewPort에 두고 사용할것 (안에서 content로 이동해줌)

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// ★사용방법
// SetData 를 호출하거나 SetChangeCB, SetPrefab, SetItemCount 순으로 호출해준다.
// SetData 또는 SetItemCount를 호출했다면 리스트 ChangeCB가 호출됨

public class ScrollReck_ViewItemController : ObjMng
{
	enum LayoutMode
	{
		Grid = 0,
		/// <summary> 상하 </summary>
		Vertical,
		/// <summary> 좌우 </summary>
		Horizontal
	}
	public enum RefreshMode
	{
		/// <summary> 그냥 갱신 </summary>
		Normal = 0,
		/// <summary> 스크롤 이동 </summary>
		Move,
		/// <summary> 리스트 추가 </summary>
		Add,
		/// <summary> 리스트 제거 </summary>
		Minus
	}

	ScrollRect _Scroll;
	ScrollRect m_Scroll { get {
			if (_Scroll == null) Init();
			return _Scroll;
	} }
	[SerializeField] RectTransform m_Prefab;
	int _Cnt = -1;
	/// <summary> 스크롤에 셋팅될 리스트의 총 개수 셋팅 호출전 프리팹 등록을 먼저했는지 필요함 </summary>
	public int m_TotalCnt { set { SetItemCount(value); } get { return _Cnt; } }

	[SerializeField] RectTransform m_StartObj;
	[SerializeField] RectTransform m_EndObj;
	[SerializeField] GameObject m_Empty;

	/// <summary> 로드된 아이템의 패널 </summary>
	RectTransform m_ItemPanel;

	Action<RefreshMode> m_OnChangeCB;

	/// <summary> 전체 라인 개수 </summary>
	int m_TotalListCnt;
	/// <summary> 한라인에 보여지는 개수 </summary>
	int m_OneLineItemCnt;
	/// <summary> Panel에 로드된 아이템 개수 </summary>
	int m_SetItemCnt;
	LayoutGroup m_LayoutGroup;
	LayoutMode m_LayoutMode;
	Action m_ViewAction;
	Func<int> m_ViewLine;
	Vector2 m_CellSize;
	int m_BeforLine;
	/// <summary> 역방향 </summary>
	bool m_IsFlip;
	void Init()
	{
		_Scroll = GetComponent<ScrollRect>();
		if (_Scroll == null) return;
		_Scroll.onValueChanged.RemoveAllListeners();
		_Scroll.onValueChanged.AddListener(OnValueChange);

		//_Scroll.content.pivot = _Scroll.content.anchorMax = _Scroll.content.anchorMin = new Vector2(0f, 1f);

		// 컨텐츠 자식들 저부 제거
		for (int i = _Scroll.content.childCount - 1; i > -1; i--) Destroy(_Scroll.content.GetChild(i).gameObject);
		
		m_ItemPanel = Utile_Class.Instantiate(_Scroll.content.gameObject, _Scroll.viewport).transform as RectTransform;
		m_ItemPanel.name = "ViewItemPanel";

		m_ItemPanel.SetAsFirstSibling();


		// 컨텐츠에 걸려있는 LayoutGroup(GridLayoutGroup, VerticalLayoutGroup, HorizontalLayoutGroup 은 LayoutGroup을 상속 받음) 꺼주기
		m_LayoutGroup = _Scroll.content.GetComponent<LayoutGroup>();
		if(m_LayoutGroup != null) m_LayoutGroup.enabled = false;
		// 컨텐츠에 걸려있는 사이즈 필터 꺼주기
		var sizefilter = _Scroll.content.GetComponent<ContentSizeFitter>();
		if (sizefilter != null) sizefilter.enabled = false;

		sizefilter = m_ItemPanel.GetComponent<ContentSizeFitter>();
		if (sizefilter != null) sizefilter.enabled = false;

		if (m_LayoutGroup is VerticalLayoutGroup)
		{
			m_LayoutMode = LayoutMode.Vertical;
			m_Scroll.vertical = true;
			m_Scroll.horizontal = false;
		}
		else if (m_LayoutGroup is HorizontalLayoutGroup)
		{
			m_LayoutMode = LayoutMode.Horizontal;
			m_Scroll.vertical = false;
			m_Scroll.horizontal = true;
		}
		else
		{
			m_LayoutMode = LayoutMode.Grid;
			if(m_Scroll.vertical && m_Scroll.horizontal)
			{
				m_Scroll.vertical = false;
				m_Scroll.horizontal = true;
			}
		}

		var group = m_ItemPanel.GetComponent<LayoutGroup>();
		if (m_Scroll.vertical)
		{
			m_ItemPanel.anchoredPosition = new Vector2(0f, m_LayoutGroup.padding.top);
			group.padding.top = group.padding.bottom = 0;
			m_ViewAction = SetPanelPosition_Vertical;
			m_ViewLine = GetViewLine_Vertical;
		}
		else
		{
			m_ItemPanel.anchoredPosition = new Vector2(m_LayoutGroup.padding.left, 0f);
			group.padding.left = group.padding.right = 0;
			m_ViewAction = SetPanelPosition_Horizontal;
			m_ViewLine = GetViewLine_Horizontal;
		}

		if (m_StartObj != null) m_StartObj.SetParent(m_Scroll.content);
		if (m_EndObj != null) m_EndObj.SetParent(m_Scroll.content);
	}

	private void OnEnable()
	{
		var scroll = m_Scroll;
	}

	public void InitPosition()
	{
		m_Scroll.horizontalNormalizedPosition = 0f;
		m_Scroll.verticalNormalizedPosition = 1f;
		OnValueChange(Vector2.zero);
	}

	bool IsValid()
	{
		if (m_Scroll == null) return false;
		if (m_Prefab == null) return false;
		return true;
	}

	public void SetData(int Count, RectTransform Prefab, Action<RefreshMode> CB)
	{
		SetChangeCB(CB);
		SetPrefab(Prefab);
		m_TotalCnt = Count;
	}

	/// <summary> 프리팹 등록 (총 개수를 셋팅 전에는 무조건 호출해주어야한다. </summary>
	/// <param name="m_Prefab">등록할 프리팹</param>
	public void SetPrefab(RectTransform Prefab)
	{
		m_Prefab = Prefab;
		LoadPrefab();
	}
#region LoadPrefab

	void LoadPrefab()
	{

		// layout 체크를 해서 content 총 사이즈를 변경해준다.
		switch (m_LayoutMode)
		{
		case LayoutMode.Grid: LoadPrefab_Grid(); break;
		case LayoutMode.Horizontal: LoadPrefab_Horizontal(); break;
		case LayoutMode.Vertical: LoadPrefab_Verticalrid(); break;
		}

		UTILE.Load_Prefab_List(m_SetItemCnt, m_ItemPanel, m_Prefab);

		// 레이아웃이 계속 켜져있으면 스크롤때 앞쪽 오브젝트가 꺼지면 자동으로 땡겨지므로 사이즈에 맞춰 배치가 되었다면 꺼준다.
		var layout = m_ItemPanel.GetComponent<LayoutGroup>();
		if (layout != null) layout.enabled = false;

		for (int i = 0; i < m_ItemPanel.childCount; i++)
		{
			var rtf = m_ItemPanel.GetChild(i) as RectTransform;
			// 애니가 있는 프리팹의경우 스케일을 변경하는게 있다면 락이 걸려 변경되지 않으므로
			// 기본값을 끄고 변경함
			// 다시 켜주는곳은 어짜피 아이템 정보 셋팅해주는데서 해주어야됨
			rtf.gameObject.SetActive(false);
			rtf.localScale = Vector3.one;
			rtf.gameObject.SetActive(true);
		}
		UTILE.SetLayoutGroupReset(m_ItemPanel);

		// 락걸린 스케일 셋팅때문에 켜져있으므
		//for (int i = 0; i < m_ItemPanel.childCount; i++) m_ItemPanel.GetChild(i).gameObject.SetActive(false);

	}

	void LoadPrefab_Grid()
	{
		var layoutgroup = m_LayoutGroup as GridLayoutGroup;

		if (m_Scroll.horizontal)
		{
			// 좌우이동이므로 Width가 길어짐
			SetBaseSizeData_Horizontal(layoutgroup.padding, layoutgroup.cellSize, layoutgroup.spacing.x, layoutgroup.spacing.y);
		}
		else
		{
			// 상하 이동이므로 Height가 길어짐
			SetBaseSizeData_Vertical(layoutgroup.padding, layoutgroup.cellSize, layoutgroup.spacing.x, layoutgroup.spacing.y);
		}
	}
	void LoadPrefab_Horizontal()
	{
		var layoutgroup = m_LayoutGroup as HorizontalLayoutGroup;
		m_IsFlip = layoutgroup.reverseArrangement;
		SetBaseSizeData_Horizontal(layoutgroup.padding, m_Prefab.sizeDelta, layoutgroup.spacing, 0f);
	}
	void LoadPrefab_Verticalrid()
	{
		var layoutgroup = m_LayoutGroup as VerticalLayoutGroup;
		m_IsFlip = layoutgroup.reverseArrangement;
		SetBaseSizeData_Vertical(layoutgroup.padding, m_Prefab.sizeDelta, 0f, layoutgroup.spacing);
	}

	void SetBaseSizeData_Horizontal(RectOffset padding, Vector2 cellsize, float spacingX, float spacingY)
	{
		// 좌우이동이므로 Width가 길어짐
		// content size 계산
		var viewrect = m_Scroll.viewport.rect;

		m_CellSize = cellsize;
		m_CellSize.x += spacingX;
		m_CellSize.y += spacingY;

		m_OneLineItemCnt = 1;
		if (m_LayoutMode == LayoutMode.Grid)
		{
			// 한 줄에 몇개가 셋팅 되는지를 계산하는 것이므로
			float gapH = padding.top + padding.bottom;
			float realsize = viewrect.height - gapH;
			m_OneLineItemCnt = (int)(realsize / cellsize.y);
		}

		// Panel size 계산 및 프리팹 로드
		float onesize = cellsize.x + spacingX;
		float SizeH = viewrect.height;
		int cnt = Utile_Class.NeedCnt((int)viewrect.width, (int)onesize + 1) + 1;
		//// 오른쪽으 한줄 더 늘림
		m_SetItemCnt = m_OneLineItemCnt * cnt;
		float SizeW = onesize * cnt;

		m_ItemPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, SizeW);
		m_ItemPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, SizeH);
	}

	void SetBaseSizeData_Vertical(RectOffset padding, Vector2 cellsize, float spacingX, float spacingY)
	{
		// 상하 이동이므로 Height가 길어짐
		// content size 계산
		var viewrect = m_Scroll.viewport.rect;

		m_CellSize = cellsize;
		m_CellSize.x += spacingX;
		m_CellSize.y += spacingY;

		m_OneLineItemCnt = 1;
		if (m_LayoutMode == LayoutMode.Grid)
		{
			// 한 줄에 몇개가 셋팅 되는지를 계산하는 것이므로
			float gapW = padding.left + padding.left;
			float realsize = viewrect.width - gapW;
			m_OneLineItemCnt = (int)(realsize / cellsize.x);
		}

		// Panel size 계산 및 프리팹 로드
		float onesize = cellsize.y + spacingY;
		float SizeW = viewrect.width;
		int cnt = Utile_Class.NeedCnt((int)viewrect.height, (int)onesize + 1) + 1;
		// 오른쪽으 한줄 더 늘림
		m_SetItemCnt = m_OneLineItemCnt * cnt;
		float SizeH = onesize * cnt;

		m_ItemPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, SizeW);
		m_ItemPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, SizeH);
	}
#endregion

	/// <summary> 스크롤에 셋팅될 리스트의 총 개수 셋팅 호출전 프리팹 등록을 먼저했는지 필요함 </summary>
	/// <param name="Count">리스트 총 개수</param>
	public void SetItemCount(int Count)
	{
		if (_Cnt == Count) {
			Refresh(RefreshMode.Normal);
			return;
		}
		RefreshMode mode = _Cnt > Count ? RefreshMode.Minus : RefreshMode.Add;
		_Cnt = Count;
		m_TotalListCnt = Utile_Class.NeedCnt(m_TotalCnt, m_OneLineItemCnt);
		Refresh(mode);

		if(m_Empty != null)
		{
			bool Active = Count < 1;
			if(Active != m_Empty.activeSelf) m_Empty.SetActive(Active);
		}
	}
	/// <summary> 스크롤에 셋팅될 리스트의 총 개수 및 프리팹 등록 </summary>
	/// <param name="Count">리스트 총 개수</param>
	/// <param name="Prefab">생성할 프리팹</param>
	public void SetItemCount(int Count, RectTransform Prefab)
	{
		if (_Cnt == Count) return;
		RefreshMode mode = _Cnt > Count ? RefreshMode.Minus : RefreshMode.Add;
		SetPrefab(Prefab);
		_Cnt = Count;
		m_TotalListCnt = Utile_Class.NeedCnt(m_TotalCnt, m_OneLineItemCnt);
		Refresh(mode);
	}

	public void SetChangeCB(Action<RefreshMode> CB)
	{
		m_OnChangeCB = CB;
	}

	/// <summary> 스크롤 정보 다시 셋팅 </summary>
	public void Refresh(RefreshMode mode = RefreshMode.Normal)
	{
		if (!IsValid()) return;

		// layout 체크를 해서 content 총 사이즈를 변경해준다.
		switch(m_LayoutMode)
		{
		case LayoutMode.Grid: SetContentSize_Grid(); break;
		case LayoutMode.Horizontal: SetContentSize_Horizontal(); break;
		case LayoutMode.Vertical: SetContentSize_Vertical(); break;
		}

		m_BeforLine = -1;
		// 배치된 리스트에 정보가 셋팅 되도록 콜백 호출
		m_OnChangeCB?.Invoke(mode);
	}

	void SetContentSize_Grid()
	{
		var layoutgroup = m_LayoutGroup as GridLayoutGroup;

		if(m_Scroll.horizontal)
		{
			// 좌우이동이므로 Width가 길어짐
			SetSize_Horizontal(layoutgroup.padding, layoutgroup.cellSize, layoutgroup.spacing.x, layoutgroup.spacing.y);
		}
		else
		{
			// 상하 이동이므로 Height가 길어짐
			SetSize_Vertical(layoutgroup.padding, layoutgroup.cellSize, layoutgroup.spacing.x, layoutgroup.spacing.y);
		}
	}

	void SetContentSize_Horizontal()
	{
		var layoutgroup = m_LayoutGroup as HorizontalLayoutGroup;
		SetSize_Horizontal(layoutgroup.padding, m_Prefab.sizeDelta, layoutgroup.spacing, 0f);
	}

	void SetContentSize_Vertical()
	{
		var layoutgroup = m_LayoutGroup as VerticalLayoutGroup;
		SetSize_Vertical(layoutgroup.padding, m_Prefab.sizeDelta, 0f, layoutgroup.spacing);
	}

	void SetSize_Horizontal(RectOffset padding, Vector2 cellsize, float spacingX, float spacingY)
	{
		// 좌우이동이므로 Width가 길어짐
		// content size 계산
		var viewrect = m_Scroll.viewport.rect;

		float prefabsize = cellsize.x;
		float SizeH = viewrect.height;
		float SizeW = padding.left + padding.right + m_TotalListCnt * prefabsize;
		if (m_StartObj != null)
		{
			SizeW += m_StartObj.rect.width;
			m_StartObj.anchoredPosition = new Vector2(padding.left + (m_StartObj.rect.width + m_StartObj.rect.x), 0f);
		}

		if (m_EndObj != null)
		{
			SizeW += m_EndObj.rect.width;
			m_EndObj.anchoredPosition = new Vector2(SizeW - (padding.right + (m_EndObj.rect.width + m_EndObj.rect.x)), 0f);
		}
		if (m_TotalListCnt > 1) SizeW += (m_TotalListCnt - 1) * spacingX;

		m_Scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, SizeW);
		m_Scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, SizeH);
	}

	void SetSize_Vertical(RectOffset padding, Vector2 cellsize, float spacingX, float spacingY)
	{
		// 상하 이동이므로 Height가 길어짐
		// content size 계산
		var viewrect = m_Scroll.viewport.rect;

		float prefabsize = cellsize.y;
		float SizeW = viewrect.width;
		float SizeH = padding.top + padding.bottom + m_TotalListCnt * prefabsize;
		if (m_TotalListCnt > 1) SizeH += (m_TotalListCnt - 1) * spacingY;

		if (m_StartObj != null)
		{
			SizeH += m_StartObj.rect.height;
			m_StartObj.anchoredPosition = new Vector2(0f, - (padding.top + (m_StartObj.rect.height + m_StartObj.rect.y)));
		}

		if (m_EndObj != null)
		{
			SizeH += m_EndObj.rect.height;
			m_EndObj.anchoredPosition = new Vector2(-SizeH + (padding.bottom + (m_EndObj.rect.height + m_EndObj.rect.y)), 0f);
		}

		m_Scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, SizeW);
		m_Scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, SizeH);
	}

	/// <summary> 현재 보여줘야할 아이템의 시작 위치 </summary>
	public int GetViewLine()
	{
		if (!IsValid()) return 0;
		return m_ViewLine.Invoke();
	}

	/// <summary> 현재 보여줘야할 아이템의 시작 위치 </summary>
	int GetViewLine_Horizontal()
	{
		// 왼쪽으로 갈수록 음수
		// 컨텐츠 시작지점으 뺀좌표가 아이템의 시작 위치
		var posX = m_Scroll.content.anchoredPosition.x + m_LayoutGroup.padding.left;
		// 스크롤 된 좌표에서 사이즈 만큼 나누면 보이는 위치
		return (int)(posX * -1f / m_CellSize.x);
	}

	/// <summary> 현재 보여줘야할 아이템의 시작 위치 </summary>
	int GetViewLine_Vertical()
	{
		// 위로 갈수록 양수
		// 컨텐츠 시작지점으 뺀좌표가 아이템의 시작 위치
		var posY = m_Scroll.content.anchoredPosition.y - m_LayoutGroup.padding.top;
		// 스크롤 된 좌표에서 사이즈 만큼 나누면 보이는 위치
		return (int)(posY / m_CellSize.y);
	}
	/// <summary> 보이는 아이템의 개수 </summary>
	public int GetViewCnt()
	{
		if (!IsValid()) return 0;
		return m_SetItemCnt;
	}

	/// <summary> 한줄에 보이는 아이템의 개수 </summary>
	public int GetOneLineItemCnt()
	{
		if (!IsValid()) return 1;
		return m_OneLineItemCnt;
	}

	/// <summary> 프리팹으로 로드된 아이템 가져오기 </summary>
	/// <typeparam name="T">반환될 컨포넌트</typeparam>
	/// <param name="offset">셋팅할 위치(0부터 시작)</param>
	/// <returns>컨포넌트 데이터, null 일경우 오프셋 값 오버거나 컨포넌트 없음</returns>
	public T GetItem<T>(int offset) where T : MonoBehaviour
	{
		//if (m_IsFlip) offset = GetViewCnt() - offset;
		if (offset < 0) return null;
		if (m_ItemPanel.childCount <= offset) return null;
		return m_ItemPanel.GetChild(offset).GetComponent<T>();
	}

	void OnValueChange(Vector2 pos)
	{
		// pos는 이동한 값인듯
		m_ViewAction?.Invoke();
	}


	void SetPanelPosition_Horizontal()
	{
		var line = GetViewLine();
		
		var posX = m_Scroll.content.anchoredPosition.x + (line * m_CellSize.x + m_LayoutGroup.padding.left);
		m_ItemPanel.anchoredPosition = new Vector2(posX, 0f);
		if (m_BeforLine != line) m_OnChangeCB?.Invoke(RefreshMode.Move);
		m_BeforLine = line;
	}
	void SetPanelPosition_Vertical()
	{
		var line = GetViewLine();

		var posY = m_Scroll.content.anchoredPosition.y - (line * m_CellSize.y + m_LayoutGroup.padding.top);
		m_ItemPanel.anchoredPosition = new Vector2(0f, posY);

		if (m_BeforLine != line) m_OnChangeCB?.Invoke(RefreshMode.Move);
		m_BeforLine = line;
	}
}
