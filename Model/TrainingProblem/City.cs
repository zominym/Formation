﻿using System;

namespace TrainingProblem
{
	public class City
	{

		protected String id;
		protected String name;
		protected String codepostal;
		protected double longitude;
		protected double latitude;

		public City () : this("\"example_id\";\"example_name\";\"example_codepostal\";9.99999;99.9999") {
			// NOTHING
		}

		public City (String csvLine){
			// "id";"nom";"codepostal";"longitude";"latitude"

            String[] csvTab = csvLine.Replace ("\"", "").Split (';');
			id = csvTab [0];
			name = csvTab [1];
			codepostal = csvTab [2];
            longitude = Double.Parse(csvTab[3]);
            latitude = Double.Parse(csvTab[4]);
						
		}

		public override string ToString ()
		{
			return id + " : " + name + " ; " + codepostal + " (" + longitude + ";" + latitude + ")";
		}

		public double distanceTo(City c)
		{
			double lat1 = this.latitude;
			double lon1 = this.longitude;
			double lat2 = c.latitude;
			double lon2 = c.longitude;

			double theta = lon1 - lon2;
			double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
			dist = Math.Acos(dist);
			dist = rad2deg(dist) * 60 * 1.1515 * 1.609344;

			return dist;
		}

		private double deg2rad(double deg) {
			return (deg * Math.PI / 180.0);
		}

		private double rad2deg(double rad) {
			return (rad / Math.PI * 180.0);
		}

		public string getId() {
			return id;
		}

		public string getName() {
			return name;
		}

		public string getCodePostal() {
			return codepostal;
		}
	}
}
