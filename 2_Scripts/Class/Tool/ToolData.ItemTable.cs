using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;
using static LS_Web;

public class ItemStat : ClassMng
{
	public StatType m_Stat; //스텟 종류
	public float m_Val;       //스텟 값
	public float m_ValUp;	//증가율

	//기본 옵션타입만 해당 추가옵션이랑 세트옵션은 제외
	public float GetValue(int LV)
	{
		return m_Val + (LV - 1) * m_ValUp;
	}

	public override string ToString()
	{
		return TDATA.GetStatString(m_Stat, m_Val);
	}

	public LS_Web.RES_ITEM_STAT GetRES()
	{
		return new RES_ITEM_STAT() {
			Type = m_Stat,
			Value = m_Val,
			ValueUp = m_ValUp
		};
	}

	public string GetStatString()
	{
		return string.Format("{0} +{1:0.##}%", TDATA.GetStatString(m_Stat), m_Val * 0.01f);
	}
}

public class TItemTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 이름 </summary>
	public int m_Name;
	/// <summary> 분류 </summary>
	public ItemType m_Type;

	// TODO DEPRECATED 제거 예정 / 09/16 ps. 제거 후 다른쪽에서 가져올 예정 
	/// <summary> 등급 </summary>
	public int m_Grade;
	
	/// <summary> 인벤토리에 나올지 여부 </summary>
	public bool m_Hide;
	/// <summary> 설명 </summary>
	public int m_Info;
	/// <summary> 판매가격(구매는 상점데이터로만 가능) </summary>
	public int m_Price;
	/// <summary> 이미지 이름 </summary>
	public string m_ImgName;
	/// <summary> 장비 소모성 아이템의 경험치량 </summary>
	public int m_Value;
	/// <summary> 스텟 </summary>
	public List<ItemStat> m_Stat = new List<ItemStat>();
	/// <summary> 재조립 등장 확률 </summary>
	public int m_ReassemblyProb;
	/// <summary> 즉시 구매용 상점 인덱스 </summary>
	public int m_ShopIdx;
	/// <summary> GetGuideTable 그룹 인덱스 </summary>
	public int m_GetGuideGid;
	/// <summary> 랜덤 스텟 테이블 참조용 그룹 </summary>
	public int m_RandStatGroup;

	public TItemTable(CSV_Result pResult)
	{
		m_Idx = pResult.Get_Int32();
		m_Name = pResult.Get_Int32();
		m_Type = pResult.Get_Enum<ItemType>();
		m_Grade = pResult.Get_Int32();
		m_Hide = pResult.Get_Boolean();
		m_Info = pResult.Get_Int32();
		m_Price = pResult.Get_Int32();
		m_ImgName = pResult.Get_String();
#if USE_LOG_MANAGER
		if (!string.IsNullOrEmpty(m_ImgName) && !m_ImgName.Contains("/"))
			Debug.LogError($"[ ItemTable ({m_Idx}) ] m_ImgName 패스 체크할것");
#endif
		m_Value = pResult.Get_Int32();
		for (int i = 0; i < 2; i++)
		{
			StatType stat = pResult.Get_Enum<StatType>();
			if (stat == StatType.None)
			{
				pResult.NextReadPos();
				pResult.NextReadPos();
				continue;
			}
			ItemStat itemstat = new ItemStat()
			{
				m_Stat = stat,
				m_Val = pResult.Get_Float(),
				m_ValUp = pResult.Get_Float()
			};
			m_Stat.Add(itemstat);
		}
		m_ReassemblyProb = pResult.Get_Int32();
		m_ShopIdx = pResult.Get_Int32();
		m_GetGuideGid = pResult.Get_Int32();
		m_RandStatGroup = pResult.Get_Int32();
	}
	public int GetPrice() {
		return Mathf.RoundToInt(m_Price * (1f + USERINFO.GetSkillValue(SkillKind.GetDoller) + USERINFO.ResearchValue(ResearchEff.DollarUp)));
	}
	public string GetName()
	{
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Name);
	}

	public string GetGradeGroupName(int grade)
	{
		if (GetInvenGroupType() == ItemInvenGroupType.CharaterPiece) return GetInvenGroupName();
		return string.Format("{0} {1}", BaseValue.GradeName(grade), GetInvenGroupName());
	}

	public string GetInfo()
	{
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Info);
	}

	public EquipType GetEquipType()
	{
		switch (m_Type)
		{
		case ItemType.None://주먹
		case ItemType.Blunt:
		case ItemType.Blade:
		case ItemType.Axe:
		case ItemType.Bow:
		case ItemType.Pistol:
		case ItemType.Shotgun:
		case ItemType.Rifle:
			return EquipType.Weapon;
		case ItemType.Helmet:
			return EquipType.Helmet;
		case ItemType.Costume:
			return EquipType.Costume;
		case ItemType.Shoes:
			return EquipType.Shoes;
		case ItemType.Accessory:
			return EquipType.Accessory;
		}
		return EquipType.End;
	}
	public ItemType GetGroupType() {
		switch (m_Type) {
			case ItemType.None://주먹
			case ItemType.Blunt:
			case ItemType.Blade:
			case ItemType.Axe:
			case ItemType.Bow:
			case ItemType.Pistol:
			case ItemType.Shotgun:
			case ItemType.Rifle:
				return ItemType.Weapon;
			default:return m_Type;
		}
	}
	public ItemInvenGroupType GetInvenGroupType()
	{
		switch (m_Type)
		{
		// 무기
		case ItemType.None:
		case ItemType.Blunt:
		case ItemType.Blade:
		case ItemType.Axe:
		case ItemType.Bow:
		case ItemType.Pistol:
		case ItemType.Shotgun:
		case ItemType.Rifle:
			// 방어구
		case ItemType.Helmet:
		case ItemType.Costume:
		case ItemType.Shoes:
		case ItemType.Accessory:
			return ItemInvenGroupType.Equipment;
		case ItemType.CharaterPiece:
			return ItemInvenGroupType.CharaterPiece;
		}
		return ItemInvenGroupType.Etc;
	}
	public string GetInvenGroupName()
	{
		ItemInvenGroupType type = GetInvenGroupType();
		switch (type)
		{
		case ItemInvenGroupType.Equipment: return TDATA.GetString(112);
		case ItemInvenGroupType.CharaterPiece: return TDATA.GetString(113);
		}
		return TDATA.GetString(237);
	}

	public UnityEngine.Sprite GetItemImg()
	{
		return UTILE.LoadImg(m_ImgName, "png");
	}


	/// <summary> 사용하여 소모하는 아이템 인지 체크 </summary>
	public bool Is_UseItem()
	{
		switch (m_Type)
		{
		// 무기
		case ItemType.GoldTeethBundle:
		case ItemType.DollarBundle:
		case ItemType.ExpBundle:
		case ItemType.BulletBundle:
			return true;
		}
		return false;
	}
}

public class TItemTableMng : ToolFile
{
	public Dictionary<int, TItemTable> DIC_Idx = new Dictionary<int, TItemTable>();
	public Dictionary<ItemType, List<TItemTable>> DIC_Type = new Dictionary<ItemType, List<TItemTable>>();

	public TItemTableMng() : base("Datas/ItemTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
		DIC_Type.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TItemTable data = new TItemTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
		if (!DIC_Type.ContainsKey(data.m_Type)) DIC_Type.Add(data.m_Type, new List<TItemTable>());
		DIC_Type[data.m_Type].Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// ItemTable
	TItemTableMng m_Item = new TItemTableMng();

	public TItemTable GetItemTable(int idx)
	{
		if (!m_Item.DIC_Idx.ContainsKey(idx)) return null;
		return m_Item.DIC_Idx[idx];
	}
	public List<TItemTable> GetItemTypeGroupTable(ItemType _type) {
		if (!m_Item.DIC_Type.ContainsKey(_type)) return null;
		return m_Item.DIC_Type[_type];
	}
	public List<TItemTable> GetAllItemIdxs()
	{
		List<TItemTable> datas = new List<TItemTable>(m_Item.DIC_Idx.Values);
		return datas;
	}
}
