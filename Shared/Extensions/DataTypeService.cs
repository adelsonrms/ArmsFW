using System;
using System.Data;

namespace ArmsFW.Services.Shared.Data
{
	public static class DataTypeService
	{

		public static object GetValue(this string value, Type type)
		{
			return type.Name switch
			{
				"Boolean" => Convert.ToBoolean(value), 
				"Int32" => Convert.ToInt32(value), 
				"DateTime" => Convert.ToDateTime(value), 
				"Decimal" => Convert.ToDecimal(value), 
				"Single" => Convert.ToSingle(value), 
				_ => value, 
			};
		}

		public static SqlDbType GetSqlTypeFromType(Type value)
		{
			return value.Name switch
			{
				"Boolean" => SqlDbType.Bit, 
				"Int32" => SqlDbType.Int, 
				"DateTime" => SqlDbType.DateTime, 
				"Decimal" => SqlDbType.Decimal, 
				"Single" => SqlDbType.Float, 
				_ => SqlDbType.VarChar, 
			};
		}

		public static SqlDbType GetSqlTypeFromValue(object value)
		{
			return GetSqlTypeFromType(value.GetType());
		}

		public static object GetValueDbType(object value, SqlDbType dbType)
		{
			if (value == null)
			{
				return DBNull.Value;
			}
			if (value.ToString() == "")
			{
				return DBNull.Value;
			}
			switch (dbType)
			{
			case SqlDbType.NChar:
			case SqlDbType.NText:
			case SqlDbType.NVarChar:
			case SqlDbType.Text:
			case SqlDbType.VarChar:
				return Convert.ToString(value);
			case SqlDbType.Int:
			case SqlDbType.SmallInt:
			case SqlDbType.TinyInt:
				return Convert.ToInt32(value);
			case SqlDbType.DateTime:
			case SqlDbType.SmallDateTime:
			case SqlDbType.Date:
			case SqlDbType.Time:
			case SqlDbType.DateTime2:
			case SqlDbType.DateTimeOffset:
				return Convert.ToDateTime(value);
			case SqlDbType.BigInt:
				return Convert.ToInt64(value);
			case SqlDbType.Bit:
				return Convert.ToBoolean(value);
			case SqlDbType.Char:
				return Convert.ToChar(value);
			case SqlDbType.Decimal:
				return Convert.ToDecimal(value);
			case SqlDbType.Float:
				return Convert.ToSingle(value);
			case SqlDbType.Binary:
			case SqlDbType.Image:
			case SqlDbType.VarBinary:
				return Convert.ToByte(value);
			case SqlDbType.Money:
				return Convert.ToDouble(value);
			case SqlDbType.SmallMoney:
				return Convert.ToDecimal(value);
			default:
				return value;
			}
		}

		public static object GetSqlDataTypeName(Type type, bool notNull = false, string tamanho = null)
		{
			string text = (notNull ? "NOT NULL" : "NULL");
			return type.Name switch
			{
				"Boolean" => "bit " + text, 
				"Int32" => "int " + text, 
				"DateTime" => "datetime " + text, 
				"Decimal" => "decimal" + ((!string.IsNullOrEmpty(tamanho)) ? tamanho : "(18, 2)") + " " + text, 
				"Single" => "float " + text, 
				_ => "varchar" + ((!string.IsNullOrEmpty(tamanho)) ? tamanho : "(255)") + " " + text, 
			};
		}
	}
}
