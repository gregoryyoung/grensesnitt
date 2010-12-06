using System;
namespace grensesnitt.Framework
{
	public interface AppliesTo<T>
	{
		void SetSubjectUnderTest(T subject);
	}
}

