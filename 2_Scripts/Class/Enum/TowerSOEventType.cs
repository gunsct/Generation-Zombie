public enum TowerEventType
{
	/// <summary> ���� </summary>
	None,
	/// <summary> �Ա� </summary>    
	Entrance,
	/// <summary> �Ϲ� ���ʹ� </summary>    
	NormalEnemy,
	/// <summary> ���� ���ʹ� </summary> 
	EliteEnemy,
	/// <summary> ���� ���ʹ� (��ǥ) </summary>  
	Boss,
	/// <summary> ���� �̺�Ʈ </summary>    
	OpenEvent,
	/// <summary> ����� �̺�Ʈ </summary>   
	SecrectEvent
}
public enum TowerSOEventType
{
	/// <summary> ���� </summary>
	None,
	/// <summary> ���� ���� (�Ϲ�), �̷ο� ī�� �� �ϳ��� ���� </summary>
	NormalSupplyBox,
	/// <summary> ���� ���� (���), �̷ο� ī�� �� �ϳ��� ���� </summary>
	EpicSupplyBox,
	/// <summary> �ǳ��� </summary>
	Refugee,
	/// <summary> �޽� </summary>
	Rest,
	/// <summary> �طο� ī�� �� �ϳ��� ���� </summary>
	BadSupplyBox,
	/// <summary> �������ͽ� ����, ����� ī�� (HP 30% ȸ�� or ����, ���ŷ�, ������, û�� 30 ���� or ����) </summary>
	StatusBuffEvent,
	/// <summary> �ش� ���������� ��ġ �� ���ʹ� �� (���� ���ʹ� ����) �ϳ��� ���, ��� �� EnemyEvent�� ���� ���μ����� ���� </summary>
	SuddenAttack
}
public enum TowerSOType
{
	Open,
	Secret,
	All
}