public enum UpTierType
{
	NONE,		//�±� �Ұ�
	LEAGUERANK, //Ư�� ���� �̻� �±�
	POINT		//Ư�� ����Ʈ �̻� �±�
}
public enum DownTierType
{
	NONE,		//���� �Ұ�
	LEAGUERANK, //Ư�� ���� �̻� ����
}
public enum UserPos
{
	My = 0,
	Target,
	Max
}
public enum PVPDeckPos
{
	Atk,
	Def
}
public enum PVPPosType
{
	None,
	/// <summary> ������ </summary>
	Combat,
	/// <summary> ������ </summary>
	Supporter
}
public enum PVPDmgType
{
	Bad,
	Normal,
	Good,
	Heal,
	Miss
}
public enum PVPArmorType
{
	/// <summary> ����. </summary>
	None,
	/// <summary> �氩 ��</summary>
	LightArmor,
	/// <summary> ���� ��</summary>
	Leather,
	/// <summary>õ �� </summary>
	Fabric,
	Max
}
public enum PVPSkillType
{
	/// <summary>  </summary>
	None,
	/// <summary> ����Ŀ ��ų </summary>
	Combat,
	/// <summary> ���� ���� ����, ���� ���� ���� ���� ��ġ ���� (���ŷ�, û�ᵵ, ������) , val1 ����, val2 ���� val3 �� </summary>
	LowStatus,
	/// <summary> ���� ���� ����, (HP, ���ŷ�, û�ᵵ, ������) , val1 ����, val2 ���� val3 �� </summary>
	Status,
	/// <summary> �������ͽ� ����, (���ǵ�, ���߷�, ġ��Ÿ, ġ��Ÿ ������ ���) , val1 ����, val2 ���� val3 ��, �ۼ�Ʈ�� ���밪�̶� ���� ������ ����� �����������   </summary>
	StatusBuff,
	/// <summary> ���� ���� ����, val1,2 ���� �ѹ�, 3�� �� </summary>
	TwoStat,
	/// <summary> �ൿ ������ �� ���� ȸ�� val2 ���(1 or 5), val3 �� </summary>
	Ap,

}
public enum PVPEquipAtkType
{
	/// <summary>  </summary>
	None,
	/// <summary> ���� ���� Ÿ�� : �氩 Ÿ�� ���� �߰� ���� </summary>
	Through,
	/// <summary> ���� ���� Ÿ�� : õ Ÿ�� ���� �߰� ���� </summary>
	Cut,
	/// <summary> ��� ���� Ÿ�� : ���� Ÿ�� ���� �߰� ���� </summary>
	Shock
}
public enum PVPAtkTargetType
{
	/// <summary> </summary>
	None,
	/// <summary> ���� ��� ���� </summary>
	Random,
	/// <summary> "�������� ���� ���� ���� ���� or �������� ���� ���� ���� �߽����� ���� ����" </summary>
	Stronger,
	/// <summary>��� ���� ���� </summary>
	All,
	/// <summary>"���� HP�� ���� ���� ���� ���� or ���� HP�� ���� ���� ���� �߽����� ���� ����" </summary>
	LowHP,
	/// <summary> "�����Ϸ��� �Ʊ��� ���� ����� ��ġ�� �ִ� �� ���� or �����Ϸ��� �Ʊ��� ���� ����� ��ġ�� �ִ� ���� �߽����� ���� ����"</summary>
	Near,
	/// <summary> �������� �ڸ� ���� �� �¿� </summary>
	RandomNear
}
public enum PVPTurnType
{
	/// <summary> </summary>
	None,
	/// <summary> "�Ʊ� Ÿ��" </summary>
	Team,
	/// <summary> "�� Ÿ��" </summary>	
	Enemy
}
public enum PVPRewardRankType
{
	RANK,
	RATIO
}
public enum PVPFailCause
{
	Men,
	Sat,
	Hyg,
	HP,
	Turn
}