using System;

namespace InterfaceInvariants.Framework
{
	public class AppliesToAll<T> : AppliesTo<T>
	{
		protected T subject;
		public void SetSubjectUnderTest(T subject) {
			this.subject = subject;
		}
	}
}

