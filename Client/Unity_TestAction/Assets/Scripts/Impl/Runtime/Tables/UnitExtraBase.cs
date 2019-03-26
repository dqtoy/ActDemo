//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class UnitExtraBase : JW_Table.Binary, JW_Table.IKey
{
	public partial class SkillIDList : JW_Table.Binary
	{
		private System.Collections.Generic.List<int> m_data = new System.Collections.Generic.List<int>();

		public System.Collections.Generic.List<int> data
		{
			get { return m_data; }
		}

		public override void Read(JW_Table.Reader reader)
		{
			m_data = reader.ReadRepeatedInt32(m_data);
		}
	}

	private int m_ID;
	private SkillIDList m_Skills;
	private int m_SkillEx;

	public int ID
	{
		get { return m_ID; }
		set { m_ID = value; }
	}

	public SkillIDList Skills
	{
		get { return m_Skills; }
		set { m_Skills = value; }
	}

	public int SkillEx
	{
		get { return m_SkillEx; }
		set { m_SkillEx = value; }
	}

	public long Key()
	{
		return m_ID;
	}

	public override void Read(JW_Table.Reader reader)
	{
		m_ID = reader.ReadInt32();
		m_Skills = reader.ReadItem<SkillIDList>();
		m_SkillEx = reader.ReadInt32();
	}
}

//UnitExtraBase.xlsx
public sealed class UnitExtraBaseManager : JW_Table.TableManager<UnitExtraBase>
{
	public const uint VERSION = 463626794;

	private UnitExtraBaseManager()
	{
	}

	private static readonly UnitExtraBaseManager ms_instance = new UnitExtraBaseManager();

	public static UnitExtraBaseManager instance
	{
		get { return ms_instance; }
	}

	public string source
	{
		get { return "UnitExtraBase.tbl"; }
	}

	public bool Load(string path)
	{
		return Load(path, source, VERSION);
	}

	public bool Load(byte[] buffer)
	{
		return Load(buffer, VERSION, source);
	}

	public UnitExtraBase Find(int key)
	{
		return FindInternal(key);
	}
}
