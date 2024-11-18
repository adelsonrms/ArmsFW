using System;

namespace ArmsFW.Services.Shared
{
    public class Periodo
    {
        private readonly DateTime _ref;

        public Periodo()
        {
            _ref = DateTime.Today;
        }
        public Periodo(DateTime Referencia):this()
        {
            _ref = Referencia;
        }
        public Periodo(int dia, int mes, int ano) : this()
        {
            _ref = new DateTime(ano, mes, dia);
        }

        public Periodo(int mes, int ano ) : this()
        {
            _ref = new DateTime(ano, mes, 1);
        }
        public DateTime? Data { get; set; }
        public Periodo(string periodo) : this()
        {
            if (!string.IsNullOrEmpty(periodo))
            {
                periodo = periodo ?? "";

                //Para periodo no formato de data yyyyMMdd
                if (System.Text.RegularExpressions.Regex.IsMatch(periodo, "^(19[0-9][0-9]|20[0-9][0-9])[0-2][0-9]"))
                {
                    if (periodo.Length == 6)
                    {
                        periodo = $"{periodo.Substring(0, 4)}-{periodo.Substring(4, 2)}-01";
                    }
                    else
                    {
                        periodo = $"{periodo.Substring(0, 4)}-{periodo.Substring(4, 2)}-{periodo.Substring(6, 2)}";
                    }

                    
                }

                if (DateTime.TryParse(periodo, out DateTime _dtref))
                {
                    Data = _dtref;
                    _ref = new DateTime(_dtref.Year, _dtref.Month, 1);
                }
                else
                {
                    _ref = DateTime.Today;
                    Data = _ref;
                };
            }
        }

        public int IdPeriodo => Convert.ToInt32(DataRef.ToString("yyyyMM"));
        public int Ano => _ref.Year;
        public int Mes => _ref.Month;
        public DateTime DataRef => _ref;// new DateTime(Ano, Mes, 1);
        public int  DataId => Convert.ToInt32(_ref.ToString("yyyyMMdd"));
        public string MesRef => DataRef.ToString("yyyyMM01");
        public string Referencia => DataRef.ToString("MMMM/yyyy");
        public string ToDataFormat(string format = "dd-MM-yyyy")
        {
            if (Data.HasValue)
            {
                return Data.Value.ToString(format);
            }
            return "";
        }

        public string ToDataFormat()
        {
            return ToDataFormat("dd-MM-yyyy");
        }

        public override string ToString() => $"{DataRef.ToString("dd-MM-yyyy")} | {IdPeriodo}";

        public static Periodo Carregar(string data)
        {
            return new Periodo(data);
        }
    }
}
