//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class CameraActionBase : JW_Table.Binary, JW_Table.IKey
{
	public partial class Param : JW_Table.Binary
	{
		private System.Collections.Generic.List<int> m_values = new System.Collections.Generic.List<int>();

		public System.Collections.Generic.List<int> values
		{
			get { return m_values; }
		}

		public override void Read(JW_Table.Reader reader)
		{
			m_values = reader.ReadRepeatedInt32(m_values);
		}
	}

	private int m_id;
	private int m_type;
	private int m_time;
	private Param m_closeup;
	private Param m_shake;
	private Param m_Lighteness;

	public int id
	{
		get { return m_id; }
		set { m_id = value; }
	}

	public int type
	{
		get { return m_type; }
		set { m_type = value; }
	}

	public int time
	{
		get { return m_time; }
		set { m_time = value; }
	}

	public Param closeup
	{
		get { return m_closeup; }
		set { m_closeup = value; }
	}

	public Param shake
	{
		get { return m_shake; }
		set { m_shake = value; }
	}

	public Param Lighteness
	{
		get { return m_Lighteness; }
		set { m_Lighteness = value; }
	}

	public long Key()
	{
		return m_id;
	}

	public override void Read(JW_Table.Reader reader)
	{
		m_id = reader.ReadInt32();
		m_type = reader.ReadInt32();
		m_time = reader.ReadInt32();
		m_closeup = reader.ReadItem<Param>();
		m_shake = reader.ReadItem<Param>();
		m_Lighteness = reader.ReadItem<Param>();
	}
}

//CameraAction.xlsx
public sealed class CameraActionBaseManager : JW_Table.TableManager<CameraActionBase>
{
	public const uint VERSION = 3644501696;

	private CameraActionBaseManager()
	{
	}

	private static readonly CameraActionBaseManager ms_instance = new CameraActionBaseManager();

	public static CameraActionBaseManager instance
	{
		get { return ms_instance; }
	}

	public string source
	{
		get { return "CameraActionBase.tbl"; }
	}

	public bool Load(string path)
	{
		return Load(path, source, VERSION);
	}

	public bool Load(byte[] buffer)
	{
		return Load(buffer, VERSION, source);
	}

	public CameraActionBase Find(int key)
	{
		return FindInternal(key);
	}
}
