using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelServices.RecuritmentServices
{
    public class Access
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }    

        public List<AccessToEmploy> AccessToEmploys { get; set; }
    }
}
