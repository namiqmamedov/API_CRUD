using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication4.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
    }
}
