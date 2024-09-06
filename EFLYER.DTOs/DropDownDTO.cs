using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFLYER.DTOs
{
    public class DropDownDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public string CountryName { get; set; }
        public int CountryId { get; set; }
    }
}
