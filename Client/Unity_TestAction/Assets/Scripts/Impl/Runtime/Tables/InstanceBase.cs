//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class InstanceBase : JW_Table.Binary, JW_Table.IKey
{
	public partial class TriggerName2IDDef : JW_Table.Binary
	{
		private string m_TriggerName;
		private int m_TriggerID;

		public string TriggerName
		{
			get { return m_TriggerName; }
			set { m_TriggerName = value; }
		}

		public int TriggerID
		{
			get { return m_TriggerID; }
			set { m_TriggerID = value; }
		}

		public override void Read(JW_Table.Reader reader)
		{
			m_TriggerName = reader.ReadString();
			m_TriggerID = reader.ReadInt32();
		}
	}

	public partial class MultiMonsterTriggerDef : JW_Table.Binary
	{
		private System.Collections.Generic.List<TriggerName2IDDef> m_data = new System.Collections.Generic.List<TriggerName2IDDef>();

		public System.Collections.Generic.List<TriggerName2IDDef> data
		{
			get { return m_data; }
		}

		public override void Read(JW_Table.Reader reader)
		{
			m_data = reader.ReadRepeatedItem(m_data);
		}
	}

	public partial class Trigger2InstanceDef : JW_Table.Binary
	{
		private string m_TriggerName;
		private string m_InstanceName;

		public string TriggerName
		{
			get { return m_TriggerName; }
			set { m_TriggerName = value; }
		}

		public string InstanceName
		{
			get { return m_InstanceName; }
			set { m_InstanceName = value; }
		}

		public override void Read(JW_Table.Reader reader)
		{
			m_TriggerName = reader.ReadString();
			m_InstanceName = reader.ReadString();
		}
	}

	public partial class MultiTransferTriggerDef : JW_Table.Binary
	{
		private System.Collections.Generic.List<Trigger2InstanceDef> m_data = new System.Collections.Generic.List<Trigger2InstanceDef>();

		public System.Collections.Generic.List<Trigger2InstanceDef> data
		{
			get { return m_data; }
		}

		public override void Read(JW_Table.Reader reader)
		{
			m_data = reader.ReadRepeatedItem(m_data);
		}
	}

	private int m_ID;
	private string m_Name;
	private string m_Desc;
	private string m_SceneName;
	private MultiMonsterTriggerDef m_MonsterTriggerInfo;
	private MultiTransferTriggerDef m_TransferTriggerInfo;

	public int ID
	{
		get { return m_ID; }
		set { m_ID = value; }
	}

	public string Name
	{
		get { return m_Name; }
		set { m_Name = value; }
	}

	public string Desc
	{
		get { return m_Desc; }
		set { m_Desc = value; }
	}

	public string SceneName
	{
		get { return m_SceneName; }
		set { m_SceneName = value; }
	}

	public MultiMonsterTriggerDef MonsterTriggerInfo
	{
		get { return m_MonsterTriggerInfo; }
		set { m_MonsterTriggerInfo = value; }
	}

	public MultiTransferTriggerDef TransferTriggerInfo
	{
		get { return m_TransferTriggerInfo; }
		set { m_TransferTriggerInfo = value; }
	}

	public long Key()
	{
		return m_ID;
	}

	public override void Read(JW_Table.Reader reader)
	{
		m_ID = reader.ReadInt32();
		m_Name = reader.ReadString();
		m_Desc = reader.ReadString();
		m_SceneName = reader.ReadString();
		m_MonsterTriggerInfo = reader.ReadItem<MultiMonsterTriggerDef>();
		m_TransferTriggerInfo = reader.ReadItem<MultiTransferTriggerDef>();
	}
}

//InstanceBase.xlsx
public sealed class InstanceBaseManager : JW_Table.TableManager<InstanceBase>
{
	public const uint VERSION = 3474955301;

	private InstanceBaseManager()
	{
	}

	private static readonly InstanceBaseManager ms_instance = new InstanceBaseManager();

	public static InstanceBaseManager instance
	{
		get { return ms_instance; }
	}

	public string source
	{
		get { return "InstanceBase.tbl"; }
	}

	public bool Load(string path)
	{
		return Load(path, source, VERSION);
	}

	public bool Load(byte[] buffer)
	{
		return Load(buffer, VERSION, source);
	}

	public InstanceBase Find(int key)
	{
		return FindInternal(key);
	}
}
