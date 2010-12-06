using System;

namespace grensesnitt.Sample.SimpleKeyValue
{
	public interface ISimpleKeyValue
	{
		void Insert(string key, object val);
		object GetByKey(string key);
		void Remove(string key);
		int Count { get; }
	}
}

