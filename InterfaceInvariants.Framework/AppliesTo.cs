using System;
namespace InterfaceInvariants.Framework
{
	public interface AppliesTo<T>
	{
		void SetSubjectUnderTest(T subject);
	}
}

