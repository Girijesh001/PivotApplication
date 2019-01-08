using System;

namespace PivotApplication
{
    public class Person
    {
       

        public string SYMBOL { get; set; }

        public DateTime EXPIRYDATE { get; set; }
        public double HIGH { get; set; }

        public double LOW { get; set; }
        public double CLOSE { get; set; }
        public int ID { get; set; }
    }

    public enum Gender
    {
        Male,
        Female
    }
}