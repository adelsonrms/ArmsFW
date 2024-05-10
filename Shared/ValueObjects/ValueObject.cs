using System.Collections.Generic;
using System.Linq;

namespace ArmsFW.Services.Shared
{
	public abstract class ValueObject
	{
		protected static bool EqualOperator(ValueObject left, ValueObject right)
		{
			if ((left == null) ^ (right == null))
			{
				return false;
			}
			return left?.Equals(right) ?? true;
		}

		protected static bool NotEqualOperator(ValueObject left, ValueObject right)
		{
			return !EqualOperator(left, right);
		}

		protected abstract IEnumerable<object> GetAtomicValues();

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != GetType())
			{
				return false;
			}
			ValueObject obj2 = (ValueObject)obj;
			IEnumerator<object> thisValues = GetAtomicValues().GetEnumerator();
			IEnumerator<object> otherValues = obj2.GetAtomicValues().GetEnumerator();
			while (thisValues.MoveNext() && otherValues.MoveNext())
			{
				if ((thisValues.Current == null) ^ (otherValues.Current == null))
				{
					return false;
				}
				if (thisValues.Current != null && !thisValues.Current.Equals(otherValues.Current))
				{
					return false;
				}
			}
			if (!thisValues.MoveNext())
			{
				return !otherValues.MoveNext();
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (from x in GetAtomicValues()
				select x?.GetHashCode() ?? 0).Aggregate((int x, int y) => x ^ y);
		}
	}
}
