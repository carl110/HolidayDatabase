using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayDatabase
{
    class Holiday
    {
        int holidayNo;
        string destination;
        double cost;
        DateTime departure;
        int noOfDays;
        bool available;

        public int HolidayNo { get => holidayNo; set => holidayNo = value; }
        public string Destination { get => destination; set => destination = value; }
        public double Cost { get => cost; set => cost = value; }
        public DateTime Departure { get => departure; set => departure = value; }
        public int NoOfDays { get => noOfDays; set => noOfDays = value; }
        public bool Available { get => available; set => available = value; }
    }
}
