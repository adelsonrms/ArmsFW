using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ArmsFW.Domain
{
	public class DbEntity<TClass>
	{
		public class TEntityBase<TEntityClass>
		{
			private List<Property> _properties;

			private object _thisEntityClass;

			private string _table;

			private StringBuilder sbCampo = new StringBuilder();

			public TEntityClass Instance => (TEntityClass)Activator.CreateInstance(typeof(TEntityClass));

			public string IdProperty { get; set; }

			[Key]
			public int Id { get; set; }

			public List<Property> Properties
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
					Properties.ForEach(p => 
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
					Properties.Where((Property pr) => !pr.Name.ToLower().Contains("ordem")).ToList().ForEach(delegate(Property p)
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
					Properties.Where((Property pr) => !pr.Name.ToLower().Contains("ordem")).ToList().ForEach(delegate(Property p)
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
					Properties.Where((Property pr) => !pr.Name.ToLower().Contains("ordem")).ToList().ForEach(delegate(Property p)
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

			public TEntityBase()
			{
				KeyConvention = "ClassId";
			}

			public TEntityBase(object entidade)
			{
				KeyConvention = "ClassId";
				_thisEntityClass = entidade;
				_table = entidade.GetType().Name;
			}

			public TEntityBase(object entidade, string table = "")
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
				dynamic tableAtt = getCustomAtt(typeof(TEntityClass).GetCustomAttributes().ToList(), "TableAttribute");
				if (tableAtt != null)
				{
					_table = tableAtt?.Name;
				}
			}

			public void Iniciatize(object entityClass)
			{
				_thisEntityClass = entityClass;
			}

			private List<Property> RefreshProperties()
			{
				_properties = new List<Property>();

				typeof(TEntityClass).GetProperties().ToList().ForEach(delegate(PropertyInfo p)
				{
					if (p.ReflectedType.Name == _thisEntityClass.GetType().Name)
					{
						Property property = new Property
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
				Property p = Properties.Find((Property a) => a.Name == pName);
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

		public TEntityBase<TClass> DbEntityBase { get; set; }

		public TEntityBase<TClass> Iniciatize(object entityClass)
		{
			DbEntityBase = new TEntityBase<TClass>();
			DbEntityBase.KeyConvention = "ClassId";
			DbEntityBase.Iniciatize(entityClass);
			return DbEntityBase;
		}
	}
}
