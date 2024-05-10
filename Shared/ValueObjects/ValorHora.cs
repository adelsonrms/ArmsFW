using System;

public static class ValorHoraExtensions
{
	public static ValorHora GetValorHora(this string totalSegundos) => new ValorHora(totalSegundos);
	public static ValorHora GetValorHora(this int totalSegundos) => new ValorHora(totalSegundos);
	public static ValorHora GetValorHora(this int? totalSegundos) => new ValorHora(totalSegundos);
	public static ValorHora GetValorHora(this double totalSegundos) => new ValorHora(((int)totalSegundos));
	public static ValorHora GetValorHora(this decimal totalSegundos) => new ValorHora(((int)totalSegundos));
}

/// <summary>
/// Representa um valor de horario com algumas funcionalidades de formatação de saida
/// </summary>
public class ValorHora
{
	public ValorHora() => Total = "00:00";
	public ValorHora(string valor) : this() => Total = valor;

	public ValorHora(int? segundos) : this(segundos ?? 0) { }

	public ValorHora(int segundos) : this() => CriarValorHoraPorSegundos(segundos);

	public ValorHora CriarValorHoraPorSegundos(int segundos)
	{
		Segundos = segundos;
		TimeSpanValor = TimeSpan.FromSeconds((int)Math.Truncate((decimal)Segundos));
		Total = $"{Math.Truncate(TimeSpanValor.TotalHours).ToString("00")}:{TimeSpanValor.Minutes.ToString("00")}";
		return this;
	}

	public string Total { get; private set; }
	public TimeSpan TimeSpanValor { get; private set; }

	public decimal TotalSerial => 0m;

	public string Sinal
	{
		get
		{
			if (string.IsNullOrEmpty(Total)) return "";
			if (!Total.Contains("-")) return "+";
			return "";
		}
	}

	internal string CorPositivo { get; set; } = "#1ABB9C;";


	internal string CorNegativo { get; set; } = "red";


	internal string CorStyle
	{
		get
		{
			if (string.IsNullOrEmpty(Total)) return "";
			if (Total.Contains("-")) return "color: " + CorNegativo;
			return "color: " + CorPositivo;
		}
	}

	public string Template
	{
		get
		{
			return @$"<div class='titulo_semama rotulo' style='{CorStyle}'><span class='hora'>{TotalHoras}</span><span class='total_resumo rotulo minuto'>:{TotalMinutos}</span></div>";
		}
	}


	public string FormatarValorDeHora(string cor = "", string classHora = "hora", string classMinutos = "minuto", string styleHora = "", string styleMinutos = "")
	{
		string style = CorStyle;
		if (!string.IsNullOrEmpty(cor)) style = $"color:{cor}";
		return @$"<div class='titulo_semama rotulo' style='{style}'><span{(!string.IsNullOrEmpty(classHora) ? $" class='{classHora}'" : "")}{(!string.IsNullOrEmpty(styleHora) ? $" style='{styleHora}'" : "")}>{TotalHoras}</span><span class='total_resumo rotulo {(!string.IsNullOrEmpty(classMinutos) ? classMinutos : "")}' {(!string.IsNullOrEmpty(styleMinutos) ? $"style='{styleMinutos}'" : "")}>:{TotalMinutos}</span></div>".Replace("\n", "").Replace("\t", "").Replace("\r", "");
	}

	public string TotalHoras
	{
		get
		{
			try
			{
				string total = Total;

				if (string.IsNullOrEmpty(total)) total = "--";
				string valor = total;
				if (total.Contains(":")) valor = total.ToString().Split(":".ToCharArray())[0];

				if (string.IsNullOrEmpty(valor)) valor = "--";
				if (valor.Length == 1) valor = "0" + valor;
				if (valor.StartsWith("-") && valor.Length == 2) valor = "-0" + valor.Replace("-", "");
				if (valor == "0") valor = "00";

				return valor;
			}
			catch (Exception)
			{
				return "00";
			}
		}
	}

	public string TotalMinutos
	{
		get
		{
			try
			{
				if (string.IsNullOrEmpty(Total)) return "00";
				if (Total.Contains(":"))
				{
					string total = Total;

					if (string.IsNullOrEmpty(total)) total = "--";
					string valor = total;

					if (total.Contains(":")) valor = total.ToString().Split(":".ToCharArray())[1];

					if (string.IsNullOrEmpty(valor)) valor = "--";
					if (valor == "0") valor = "00";

					return valor;
				}
				else
				{
					return "00";
				}

			}
			catch (Exception)
			{
				return "00";
			}
		}
	}

	public int Segundos { get; internal set; }
	public int Horas => ((int)TimeSpanValor.TotalHours);

	public override string ToString() => $"{Sinal}{Total}";

	public ValorHora Soma(int totalSegundos)
	{
		Segundos += totalSegundos;
		CriarValorHoraPorSegundos(Segundos);
		return this;
	}
}
//namespace ArmsFW.Services.Shared
//{


//}
