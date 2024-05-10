using System.Collections.Generic;

namespace Portal.Infra
{
	public class ListaInMemory<TEntity> : ListaInMemory<int, TEntity>
	{
	}
	public class ListaInMemory<TKey, TEntity> : Dictionary<TKey, TEntity>
	{
		public TEntity GetEntity(TKey id)
		{
			try
			{
				if (TryGetValue(id, out var item))
				{
					return item;
				}
			}
			catch
			{
			}
			return default(TEntity);
		}
	}
}
