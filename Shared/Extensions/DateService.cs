using System;

namespace ArmsFW.Util.Date
{
	public class DateService
	{
		private int _years;

		private int _months;

		private int _days;

		public int Years => _years;

		public int Months => _months;

		public int Days => _days;

		public DateTime? DtInicial { get; set; }

		public DateTime? DtFinal { get; set; }

		public DateService()
		{
		}

		public DateService(DateTime? startDate, DateTime? endDate)
		{
			DtInicial = startDate;
			DtFinal = endDate;
		}

		public string TempoDecorrido(string formato)
		{
			return TempoDecorrido(DtInicial, DtFinal, formato);
		}

		public string TempoDecorrido(DateTime? startDate, DateTime? endDate, string formato)
		{
			if (!startDate.HasValue) return $"";

			if (!endDate.HasValue)
			{
				endDate = startDate;
			}
			
			DtInicial = startDate;
			DtFinal = endDate;

			if (startDate > endDate)
			{
				DateTime? dateTime = startDate;
				startDate = endDate;
				endDate = dateTime;
			}
			int startYear = startDate.Value.Year;
			int startMonth = startDate.Value.Month;
			int startDay = startDate.Value.Day;
			int endYear = endDate.Value.Year;
			int endMonth = endDate.Value.Month;
			int endDay = endDate.Value.Day;
			if (endDay < startDay)
			{
				DateTime previousMonth = endDate.Value.AddMonths(-1);
				endDay += DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);
				endMonth--;
				if (endMonth < 1)
				{
					endMonth += 12;
					endYear--;
				}
			}
			if (endMonth < startMonth)
			{
				endMonth += 12;
				endYear--;
			}
			_years = endYear - startYear;
			_months = endMonth - startMonth;
			_days = endDay - startDay;
			string yearString = ((Years == 0) ? string.Empty : string.Format("{0} ano{1}", Years, (Years > 1) ? "s" : string.Empty)).ToString();
			string monthString = ((Months == 0) ? string.Empty : string.Format("{0} mes{1}", Months, (Months > 1) ? "es" : string.Empty)).ToString();
			string dayString = string.Format("{0} dia{1}", Days, (Days != 1) ? "s" : string.Empty);
			if (formato == "y")
			{
				return $"{yearString}";
			}
			if (formato == "ym")
			{
				if (yearString == "")
				{
					if (string.IsNullOrEmpty(monthString))
					{
						return $"{dayString}";
					}
					return $"{monthString}";
				}
				return $"{yearString} e {monthString}";
			}
			return $"{yearString}, {monthString} e {dayString}";
		}

		public static string CalculaIdade(DateTime data) => new DateService().TempoDecorrido(data, DateTime.Today, "y");
		public static string CalculaIdade(string data)
		{
            if (DateTime.TryParse(data, out DateTime dt))
            {
				return new DateService().TempoDecorrido(dt, DateTime.Today, "y");
			}
			return "";
		}
	}
}
