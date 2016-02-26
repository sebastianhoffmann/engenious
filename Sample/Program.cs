using System;

namespace Sample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			using(TestGame test = new TestGame ())
				test.Run ();
		}
	}
}
