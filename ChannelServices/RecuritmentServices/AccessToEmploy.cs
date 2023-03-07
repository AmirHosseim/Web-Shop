using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelServices.RecuritmentServices
{
    public class AccessToEmploy
    {
        [Key]
        public long Id { get; set; }

        public string UserId { get; set; }

        public string ChannelId { get; set; }

        public int AccessId { get; set; }


        public Access Access { get; set; }
    }
}
