using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LieuxDeFormation
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			StreamReader fs = new StreamReader ("../../LieuxPossibles.txt");

			String line;
			int counter = 0;

			while((line = fs.ReadLine()) != null)
			{
				Console.WriteLine (counter + ": " + line);
				counter++;
			}

			fs.Close ();
		}
	}
}
