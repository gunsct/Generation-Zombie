
public enum OptionType
{
	/// <summary> ���� </summary>
	None,
	/// <summary> �ش� ĳ���Ͱ� ���ݽ� ����% </summary>
	Vampire,
	/// <summary> ���� : �ش� ĳ���Ͱ� ���ݽ� �߰�������% </summary>
	AttackingDmgAdd,
	/// <summary> �� : �ش� ĳ���Ͱ� ���ݽ� �����°���% </summary>
	AttackingDefDown,
	/// <summary> ����� : �ش� ĳ���Ͱ� ���ݽ� Ȯ������% </summary>
	AttackingStun,
	/// <summary> ��� : �ش� ĳ���Ͱ� ���ݽ� Ȯ�����% </summary>
	AttackingKill,
	/// <summary> ȯ�� : �ش� ĳ���Ͱ� �ǰݽ� ����ȸ��% </summary>
	HitDodge,
	/// <summary> �ź��� : �ش� ĳ���Ͱ� �ǰݽ� ����������% </summary>
	HitDmgDown,
	/// <summary> ����ġ : �ش� ĳ���Ͱ� �ǰݽ� �������ݻ�% </summary>
	HitThorn,
	/// <summary> �Ƹ������� : �ش� ĳ���Ͱ� �ǰݽ� ��������% </summary>
	HitDefUp,
	/// <summary> Ʈ�� : �ش� ĳ���Ͱ� ȸ���� �߰�ȸ��% </summary>
	CureHealAdd,
	/// <summary> ǥ�� : �ش� ĳ���Ͱ� ȸ���� �ൿ��ȸ��% </summary>
	CureApAdd,
	/// <summary> ����� : �ش� ĳ������ �ൿ�¼Ҹ𰨼�% </summary>
	ApConsumDown,
	/// <summary> ���� : �ش� ĳ������ ���ݷ�����% </summary>
	AtkUp,
	/// <summary> �� : �ش� ĳ������ ��������% </summary>
	DefUp,
	/// <summary> ���ݼ��� : �ش� ĳ������ ȸ��������% </summary>
	HealUp,
	/// <summary> ���Ҹ� : �ش� ĳ������ ���� �ִ�ġ����% </summary>
	MenUp,
	/// <summary> ���� : �ش� ĳ������ ���� �ִ�ġ����% </summary>
	HygUp,
	/// <summary> ����� : �ش� ĳ������ ������ �ִ�ġ����% </summary>
	SatUp,
	/// <summary> ������ : �ش� ĳ���� �ǰݽ� ���� ���Ұ���% (���̹� ����������) </summary>
	HitMenDefUp,
	/// <summary> ���ڵ�� : �ش� ĳ���� �ǰݽ� ���� ���Ұ���% </summary>
	HitHygDefUp,
	/// <summary> �巹��ũ : �ش� ĳ���� �ǰݽ� ������ ���Ұ���% </summary>
	HitSatDefUp,
	/// <summary> �׸��� : �ش� ĳ���� ���� �� ũ��Ƽ�� ���� Ȯ�� ���� </summary>
	AttackingCriShoot,
	/// <summary> ���� : �ش� ĳ���Ͱ� ���� �� ��ų ��Ÿ�� �ʱ�ȭ </summary>
	AttackingCoolDown,
	/// <summary> �罺��ġ : �ش� ĳ���Ͱ� �ǰ� �� ��ų ��Ÿ�� �ʱ�ȭ </summary>
	HitCoolDown,
	/// <summary> ��� : �ش� ĳ���Ͱ� ���� �� ������ ��� ȹ�� (������ ���� Ȯ�� �� ���� ����) </summary>
	AttackingMaterialAdd,
	/// <summary> ��ť���� : �ش� ĳ���Ͱ� �ǰ� �� ������ ��� ȹ�� (������ ���� Ȯ�� �� ���� ����) </summary>
	HitMaterialAdd,
	/// <summary> ����� : HP�� ���� ��� �� Ȯ�������� 100%�� ��Ȱ
	/// <para>- ������ ���� %����</para>
	/// <para>- �ߺ� ���� �� HP ȸ������ ������Ű�� HP�� ���� ��� �� ��Ȱ�� 1���������� 1ȸ�� ����</para>
	/// </summary>
	HpResurrection,
	/// <summary> ���� : �ش� ĳ������ ������ ��弦���� ���� �� �ֺ� 9ĭ�� ���� ��� ���
	/// <para>- ��ų���� ����</para>
	/// </summary>
	ExplosiveHeadshot,
	/// <summary> ��	 : �ش� ĳ������ ������ ��弦���� ���� �� ������ ���� ���� ��� ��� 
	/// <para>- ��ų���� ����</para>
	/// </summary>
	PenetratingHeadshot,
	/// <summary> ���Ҹ� : �������� ���� ��� �� �ش� ĳ������ �ִ� ������ @%���� ä�� ��Ȱ
	/// <para>- ������ ���� %����</para>
	/// <para>- �ߺ� ���� �� ���� ȸ������ ������Ű�� ���ſ� ���� ��� �� ��Ȱ�� 1���������� 1ȸ�� ����</para>
	/// </summary>
	MenResurrection,
	/// <summary> ���� : �������� ���� ��� �� �ش� ĳ������ �ִ� ������ @%���� ä�� ��Ȱ
	/// <para>- ������ ���� %����</para>
	/// <para>- �ߺ� ���� �� ���� ȸ������ ������Ű�� ������ ���� ��� �� ��Ȱ�� 1���������� 1ȸ�� ����</para>
	/// </summary>
	HygResurrection,
	/// <summary> ����� : ���������� ���� ��� �� �ش� ĳ������ �ִ� �������� @%���� ä�� ��Ȱ
	/// <para>- ������ ���� %����</para>
	/// <para>- �ߺ� ���� �� ������ ȸ������ ������Ű�� �������� ���� ��� �� ��Ȱ�� 1���������� 1ȸ�� ����</para>
	/// </summary>
	SatResurrection,
	/// <summary> ������ : �������� �������� ������ ��� �� HP, ����,����, ������ ��� ȸ��
	/// <para>- ������ ���� ȸ�� ���� ����</para>
	/// </summary>
	KilltoHeal,
	/// <summary> ���ڵ�� : �������� �������� ������ ��� �� ��ü ��ų �� ����
	/// <para>- ������ ���� ���� �� ����</para>
	/// </summary>
	KilltoCoolDown,
	/// <summary> �巹��ũ : �ش� ĳ���Ͱ� �ǰ� �� �����ڰ� Ȯ�������� ��� </summary>
	DeathThorn,
	/// <summary>  </summary>
	End
}

public enum DNABGType
{
	None = 0,
	Red,
	Blue,
	Green,
	Purple,
	End
}