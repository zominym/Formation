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
			FileStream fs = File.Open ("../../ListeAgences_100.txt", FileMode.Open);

			UTF8Encoding temp = new UTF8Encoding(true);
			byte[] b = new byte[1024];

			while (fs.Read(b,0,b.Length) > 0)
			{
				Console.WriteLine(temp.GetString(b));
			}
		}
	}
}
