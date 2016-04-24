using System;

namespace TrainingProblem
{
	public class Agency : City
	{

		public Agency() : base()
		{

		}

		public Agency(string csvLine) : base(csvLine)
		{
			nbPers = int.Parse(csvLine.Split (';')[5]);
		}

		public override string ToString ()
		{
			return id + " : " + name + " ; " + codepostal + " (" + longitude + ";" + latitude + ") : " + nbPers;
		}
	}
}

