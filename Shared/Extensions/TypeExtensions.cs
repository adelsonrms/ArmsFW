using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ArmsFW.Services.Extensions
{
	public static class TypeExtensions
    {

		public static T Merge<T>(this T origem, T destino)
        {

			origem.GetType()
				.GetProperties()
				.Where(x => x.CanRead && x.CanWrite)
				.ToList()
				.ForEach(p => 
			{
				//Valor da propriedade de origem
				var valor_origem = GetMemberValue(origem, p);

				//Seta na propriedade de destino de mesmo nome
				SetMemberValue(destino, p, valor_origem);
			});

			return destino;

		}

		/// <summary>
		/// Dado uma propriedade qualquer, define o valor
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="origem"></param>
		/// <param name="propriedade"></param>
		/// <param name="valor"></param>
		public static void DefinirValor<T>(this T origem, string propriedade, object valor = null)
		{
            try
            {
				var p = origem.GetType().GetProperty(propriedade);

                if (p!=null)
                {
					SetMemberValue(origem, p, valor);
				}
			}
            catch 
            {
            }
		}

		public static ObjetoCopiado<T, TDest> CopiarPara<T, TDest>(this T origem, TDest destino, params string[] exceto)
		{
			exceto = exceto ?? new string[0];
			int qtd_alterados = 0;
			List<ValorAlterado> _valorAlterado = new List<ValorAlterado>();


			origem.GetType()
				.GetProperties()
				.Where(x => x.CanRead && x.CanWrite)
				.Where(a => exceto.Length==0 || !exceto.Any(b => b.ToLower() == a.Name.ToLower()))
				.ToList()
				.ForEach(p =>
				{
					//Valor da propriedade de origem
					var valor_origem = GetMemberValue(origem, p) ?? "";

					var pDestino = destino.GetType().GetProperties().FirstOrDefault(pd => pd.Name.ToLower() == p.Name.ToLower());

                    if (pDestino!=null)
                    {
						var valor_destino = GetMemberValue(destino, pDestino) ?? "";

						if (!valor_origem.Equals(valor_destino))
						{
							_valorAlterado.Add(new ValorAlterado { Item = p.Name, valor_origem = valor_origem, valor_destino = valor_destino });
							//Seta na propriedade de destino de mesmo nome
							SetMemberValue(destino, pDestino, valor_origem);
							qtd_alterados += 1;
						}
					}
				});

			return new ObjetoCopiado<T, TDest> {

				Origem = origem, Destino = destino, QtdAlterados = qtd_alterados, ItemAlterados = _valorAlterado
			};
		}

		public static string GetAtributo(this Type type, string nome)
		{
			var att = getCustomAtt(type.GetCustomAttributes().ToList(), nome + "Attribute");
			
			if (att != null)
			{
				var p = ((Type)att.GetType()).GetProperties().ToList().Find(x => x.Name == "Name");

				return GetMemberValue((Attribute)att, p).ToString();
			}

			return "";
		}


		public static List<Propriedade> Propriedades(this Type tipo)
        {
			List<Propriedade> _properties = new List<Propriedade>();

			tipo.GetProperties().ToList().ForEach(delegate (PropertyInfo p)
			{
				if (p.ReflectedType.Name == tipo.Name)
				{
					Propriedade property = new Propriedade
					{
						Name = GetPropertyName(p),
						Value = GetMemberValue(tipo, p),
						DataType = p.PropertyType.Name
					};

					if (!string.IsNullOrEmpty(property.Name)) _properties.Add(property);
				}
			});


			return _properties;
		}

		public static  string GetPropertyName(PropertyInfo p)
		{
			string pName = p.Name;
			dynamic columnAtt = getCustomAtt(p.GetCustomAttributes().ToList(), "IgnoreAttribute");
			if (columnAtt != null)
			{
				return null;
			}
			columnAtt = getCustomAtt(p.GetCustomAttributes().ToList(), "ColumnAttribute");
			if (columnAtt != null)
			{
				return columnAtt?.Name;
			}
			columnAtt = getCustomAtt(p.GetCustomAttributes().ToList(), "KeyAttribute");

			if (columnAtt != null)
			{
				return p.Name;
			}
			return pName;
		}

		public static  dynamic getCustomAtt(dynamic obj, string type)
		{
			List<Attribute> attL = obj;
			if (attL.Count > 0)
			{
				return attL.Find((Attribute x) => ((Type)x.TypeId).Name == type);
			}
			return null;
		}

		private static void SetMemberValue(object objeto, PropertyInfo membro, object valor)
		{
			try
			{

				if (membro == null) return;

				if (membro.GetGetMethod().IsStatic)
				{
					membro.SetValue(null, valor, null);
				}
				else
				{
					membro.SetValue(objeto, valor, null);
				}
			}
			catch
			{
			}
		}

		public static  object GetValue(this Type tipo, string pName)
		{
			PropertyInfo propMember = tipo.GetProperty(pName);
			return GetMemberValue(tipo, propMember);
		}

		public static object GetMemberValue(object objeto, PropertyInfo membro)
		{
			if (membro.GetGetMethod().IsStatic)
			{
				return membro.GetValue(null, null);
			}
			return membro.GetValue(objeto, null);
		}
	}

    internal class ValorAlterado
    {
        public string Item { get; set; }
        public object valor_origem { get; set; }
        public object valor_destino { get; set; }
    }

    public class ObjetoCopiado<TOrigem, TDestino>
    {
        public TOrigem Origem { get; set; }
		public TDestino Destino { get; set; }
        public int QtdAlterados { get; internal set; }
		internal List<ValorAlterado> ItemAlterados { get; set; } = new List<ValorAlterado>();
    }

    public class TypeBase<TObject>
	{
		private List<Propriedade> _properties;

		private object _thisEntityClass;

		private string _table;

		private StringBuilder sbCampo = new StringBuilder();

		public TObject Instance => (TObject)Activator.CreateInstance(typeof(TObject));

		public string IdProperty { get; set; }

		[Key]
		public int Id { get; set; }

		public List<Propriedade> Properties
		{
			get
			{
				if (_properties == null)
				{
					_properties = RefreshProperties();
				}
				return _properties;
			}
		}

		private string SelectFields
		{
			get
			{
				sbCampo = new StringBuilder();
				Properties.ForEach(delegate (Propriedade p)
				{
					if (p.Name == "Id")
					{
						sbCampo.Append("," + TrataCampo(p.Name) + " AS Id");
					}
					else
					{
						sbCampo.Append("," + TrataCampo(p.Name) + " ");
					}
				});
				return sbCampo.Remove(0, 1).ToString().Trim();
			}
		}

		private string InsertFields
		{
			get
			{
				sbCampo = new StringBuilder();
				Properties.Where((Propriedade pr) => !pr.Name.ToLower().Contains("ordem")).ToList().ForEach(delegate (Propriedade p)
				{
					sbCampo.Append("," + TrataCampo(p.Name) + " ");
				});
				return sbCampo.Remove(0, 1).ToString().Trim();
			}
		}

		private string InsertValues
		{
			get
			{
				sbCampo = new StringBuilder();
				Properties.Where((Propriedade pr) => !pr.Name.ToLower().Contains("ordem")).ToList().ForEach(delegate (Propriedade p)
				{
					sbCampo.Append($",{GetTSQLValue(p.Name)} ");
				});
				return sbCampo.Remove(0, 1).ToString();
			}
		}

		private string UpdateFields
		{
			get
			{
				sbCampo = new StringBuilder();
				Properties.Where((Propriedade pr) => !pr.Name.ToLower().Contains("ordem")).ToList().ForEach(delegate (Propriedade p)
				{
					sbCampo.Append($",{TrataCampo(p.Name)} = {GetTSQLValue(p.Name)} ");
				});
				return sbCampo.Remove(0, 1).ToString().Trim();
			}
		}

		private string SelectAll => "SELECT " + SelectFields + " FROM " + ToTable;

		public string ToTable
		{
			get
			{
				if (_table != null)
				{
					return _table;
				}
				return _thisEntityClass.GetType().Name;
			}
			set
			{
				_table = value;
			}
		}

		public string KeyConvention { get; set; }

		public object SqlCrud
		{
			get
			{
				try
				{
					return new
					{
						InsertCommand = Insert(),
						SelectCommand = Select(),
						UpdateCommand = Update(),
						DeleteCommand = Delete()
					};
				}
				catch
				{
					return new
					{
						InsertCommand = "(Exception)",
						SelectCommand = "(Exception)",
						UpdateCommand = "(Exception)",
						DeleteCommand = "(Exception)"
					};
				}
			}
		}

		public bool IsTableFunction => ToTable.EndsWith("()");

		public TypeBase()
		{
			KeyConvention = "ClassId";
		}

		public TypeBase(object entidade)
		{
			KeyConvention = "ClassId";
			_thisEntityClass = entidade;
			_table = entidade.GetType().Name;
		}

		public TypeBase(object entidade, string table = "")
		{
			if (entidade == null)
			{
				entidade = Instance;
			}
			KeyConvention = "ClassId";
			_thisEntityClass = entidade;
			_table = table ?? entidade.GetType().Name.ToString();
			GetDataNotations();
			Iniciatize(entidade);
			RefreshProperties();
			if (KeyConvention == "ClassId")
			{
				IdProperty = _table + "Id";
			}
			else
			{
				IdProperty = "Id";
			}
		}

		private void GetDataNotations()
		{
			dynamic tableAtt = getCustomAtt(typeof(TObject).GetCustomAttributes().ToList(), "TableAttribute");
			if (tableAtt != null)
			{
				_table = tableAtt?.Name;
			}
		}

		public void Iniciatize(object entityClass)
		{
			_thisEntityClass = entityClass;
		}

		private List<Propriedade> RefreshProperties()
		{
			_properties = new List<Propriedade>();
			typeof(TObject).GetProperties().ToList().ForEach(delegate (PropertyInfo p)
			{
				if (p.ReflectedType.Name == _thisEntityClass.GetType().Name)
				{
					Propriedade property = new Propriedade
					{
						Name = GetPropertyName(p),
						Value = GetMemberValue(_thisEntityClass, p),
						DataType = p.PropertyType.Name
					};
					if (!string.IsNullOrEmpty(property.Name))
					{
						_properties.Add(property);
					}
				}
			});
			return _properties;
		}

		private string GetPropertyName(PropertyInfo p)
		{
			string pName = p.Name;
			dynamic columnAtt = getCustomAtt(p.GetCustomAttributes().ToList(), "IgnoreAttribute");
			if (columnAtt != null)
			{
				return null;
			}
			columnAtt = getCustomAtt(p.GetCustomAttributes().ToList(), "ColumnAttribute");
			if (columnAtt != null)
			{
				return columnAtt?.Name;
			}
			columnAtt = getCustomAtt(p.GetCustomAttributes().ToList(), "KeyAttribute");
			if (columnAtt != null)
			{
				KeyConvention = "Id";
				IdProperty = p.Name;
				return p.Name;
			}
			return pName;
		}

		private dynamic getCustomAtt(dynamic obj, string type)
		{
			List<Attribute> attL = obj;
			if (attL.Count > 0)
			{
				return attL.Find((Attribute x) => ((Type)x.TypeId).Name == type);
			}
			return null;
		}

		public object GetValue(string pName)
		{
			PropertyInfo propMember = _thisEntityClass.GetType().GetProperty(pName);
			return GetMemberValue(_thisEntityClass, propMember);
		}

		public void SetValue(string pName, object pValue)
		{
			PropertyInfo propMember = _thisEntityClass.GetType().GetProperty(pName);
			SetMemberValue(_thisEntityClass, propMember, pValue);
		}

		private void SetMemberValue(object objeto, PropertyInfo membro, object valor)
		{
			try
			{
				if (membro.GetGetMethod().IsStatic)
				{
					membro.SetValue(null, valor, null);
				}
				else
				{
					membro.SetValue(objeto, valor, null);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("setMemberValue() - Erro :" + ex.Message);
			}
		}

		private object GetMemberValue(object objeto, PropertyInfo membro)
		{
			if (membro.GetGetMethod().IsStatic)
			{
				return membro.GetValue(null, null);
			}
			return membro.GetValue(objeto, null);
		}

		private object GetTSQLValue(string pName)
		{
			Propriedade p = Properties.Find((Propriedade a) => a.Name == pName);
			switch (p.DataType.ToLower())
			{
				case "string":
				case "guid":
					return "'" + p.Value?.ToString() + "'";
				case "date":
					return "'" + Convert.ToDateTime(p?.Value).ToString("yyyyMMdd") + "'";
				case "datetime":
					return "'" + Convert.ToDateTime(p?.Value).ToString("yyyyMMdd hh:mm:ss") + "'";
				default:
					return p?.Value.ToString();
			}
		}

		private string TrataCampo(string idName)
		{
			string id = "[" + idName + "]";
			if (KeyConvention == "ClassId" && idName == "Id")
			{
				id = ((!ToTable.Contains(".")) ? ("[" + ToTable + idName + "]") : ("[" + ToTable.Split(".".ToArray())[1] + idName + "]"));
			}
			return id;
		}

		private string WhereId()
		{
			return string.Format("WHERE {0} = {1}", TrataCampo("Id"), GetTSQLValue("Id"));
		}

		private string SelectWithFields(string campos)
		{
			return "SELECT " + campos + " FROM " + ToTable;
		}

		public string Select()
		{
			return SelectAll + " " + WhereId();
		}

		public string Select(bool all = false)
		{
			if (all)
			{
				return SelectAll;
			}
			return Select();
		}

		public string Select(string campos, string filterWhere)
		{
			return SelectWithFields(campos) + " WHERE " + filterWhere;
		}

		public string Select(string campos, string filtroFunction, string filterWhere)
		{
			if (IsTableFunction)
			{
				return "SELECT " + campos + " FROM " + ToTable.Replace("()", "") + "(" + filtroFunction + ") WHERE " + filterWhere;
			}
			return "SELECT " + campos + " FROM " + ToTable + " WHERE " + filterWhere;
		}

		public string Delete(bool all = false)
		{
			if (all)
			{
				return "DELETE FROM " + ToTable;
			}
			return "DELETE FROM " + ToTable + " " + WhereId();
		}

		public string Update()
		{
			return "UPDATE " + ToTable + " SET " + UpdateFields + " " + WhereId();
		}

		public string Insert()
		{
			return "INSERT INTO " + ToTable + " (" + InsertFields + ") VALUES (" + InsertValues + ")";
		}

		public string SelectById(string id)
		{
			return SelectByCollumn(IdProperty, id);
		}

		public string SelectByCollumn(string fieldName, string fieldValue)
		{
			return Select(all: true) + " WHERE " + fieldName + "='" + fieldValue + "'";
		}
	}

	public class DbEntity<TObject>
	{
		

		public TypeBase<TObject> DbEntityBase { get; set; }

		public TypeBase<TObject> Iniciatize(object entityClass)
		{
			DbEntityBase = new TypeBase<TObject>();
			DbEntityBase.KeyConvention = "ClassId";
			DbEntityBase.Iniciatize(entityClass);
			return DbEntityBase;
		}

	
	}

	public class Propriedade
	{
		public string Name { get; set; }

		public object Value { get; set; }

		public string DataType { get; set; }
	}
}
