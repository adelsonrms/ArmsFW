//using System;
//using System.Text.RegularExpressions;

//namespace app.core.Periodos
//{
//	public class PeriodoOld
//	{
//		private readonly DateTime _ref;

//		public int IdPeriodo => Convert.ToInt32(DataRef.ToString("yyyyMM"));

//		public int Ano => _ref.Year;

//		public int Mes => _ref.Month;

//		public DateTime DataRef => _ref;

//		public int DataId => Convert.ToInt32(_ref.ToString("yyyyMMdd"));

//		public DateTime? Data { get; private set; }

//		public string MesRef => DataRef.ToString("yyyyMM01");

//		public string Referencia => DataRef.ToString("MMM/yy");

//		public PeriodoOld()
//		{
//			_ref = DateTime.Today;
//		}

//		public PeriodoOld(DateTime Referencia)
//			: this()
//		{
//			_ref = Referencia;
//		}

//		public PeriodoOld(int dia, int mes, int ano)
//			: this()
//		{
//			_ref = new DateTime(ano, mes, dia);
//		}

//		public PeriodoOld(int mes, int ano)
//			: this()
//		{
//			_ref = new DateTime(ano, mes, 1);
//		}

//		public PeriodoOld(string periodo)
//			: this()
//		{
//			if (string.IsNullOrEmpty(periodo))
//			{
//				return;
//			}
//			periodo = periodo ?? "";
//			if (Regex.IsMatch(periodo, "^(19[0-9][0-9]|20[0-9][0-9])[0-2][0-9]"))
//			{
//				if (periodo.Length == 8)
//				{
//					Data = Convert.ToDateTime(periodo.Substring(0, 4) + "-" + periodo.Substring(4, 2) + "-" + periodo.Substring(6, 2) + " 00:00:00");
//				}
//				else
//				{
//					Data = Convert.ToDateTime(periodo.Substring(0, 4) + "-" + periodo.Substring(4, 2) + "-01 00:00:00");
//				}
//				_ref = Convert.ToDateTime(periodo.Substring(0, 4) + "-" + periodo.Substring(4, 2) + "-01 00:00:00");
//			}
//			else
//			{
//				if (!DateTime.TryParse(periodo, out _ref))
//				{
//					_ref = DateTime.Today;
//				}
//				Data = _ref;
//			}
//		}

//		public string ToDataFormat(string format = "dd-MM-yyyy")
//		{
//			if (Data.HasValue)
//			{
//				return Data.Value.ToString(format);
//			}
//			return "";
//		}

//		public string ToDataFormat()
//		{
//			return ToDataFormat("dd-MM-yyyy");
//		}

//		public override string ToString()
//		{
//			return string.Format("{0} | {1}", DataRef.ToString("dd-MM-yyyy"), IdPeriodo);
//		}

//		public static PeriodoOld Carregar(string data)
//		{
//			return new PeriodoOld(data);
//		}
//	}
//}
