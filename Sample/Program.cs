using System;

namespace Sample
{
	class MainClass
	{
        [STAThread()]
		public static void Main (string[] args)
		{
			using(TestGame test = new TestGame ())
				test.Run ();
		}
	}
}
