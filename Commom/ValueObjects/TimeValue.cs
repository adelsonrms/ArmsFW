namespace ArmsFW.ValueObjects
{
	public class TimeValue
	{
		public string Hour { get; set; } = "00";


		public string Minute { get; set; } = "00";


		public string Second { get; set; } = "00";


		public TimeValue(string value)
		{
			string[] tmSplit = value.Split(":".ToCharArray());
			Hour = tmSplit[0];
			if (tmSplit.Length == 2)
			{
				Minute = tmSplit[1];
			}
			if (tmSplit.Length == 3)
			{
				Second = tmSplit[2];
			}
		}
	}
}
