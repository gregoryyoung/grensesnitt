using System;
namespace grensesnitt.Framework
{
	/// <summary>
	/// Interface that specifies what type the test fixture is a fixture for. The type specified here
	/// denotes what interface implementations of will be searched for, from the available assembly sources.
	/// </summary>
	/// <typeparam name="T">The type to search for.</typeparam>
	public interface AppliesTo<T>
	{
		/// <summary>
		/// Method called by the add-in to set the current test subject instance.
		/// </summary>
		/// <param name="subject">subject instance</param>
		void SetSubjectUnderTest(T subject);
	}
}

