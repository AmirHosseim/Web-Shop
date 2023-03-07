using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServices.UserManager
{
    public class SpecialUser
    {
        public long Id { get; set; }

        public DateTime CreatDate { get; set; }

        public DateTime FinishDateTime { get; set; }

        public int OutTimeDay { get; set; }

        public string UserId { get; set; }

        public bool IsFinished { get; set; }
    }

    public class OutTimeDay
    {
        public string Title { get; set; }

        public int Day { get; set; }

        public decimal Price { get; set; }
    }
}
