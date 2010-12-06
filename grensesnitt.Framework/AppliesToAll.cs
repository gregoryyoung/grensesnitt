using System;

namespace grensesnitt.Framework
{
	public class AppliesToAll<T> : AppliesTo<T>
	{
		protected T subject;
		public void SetSubjectUnderTest(T subject) {
			this.subject = subject;
		}
	}
}

