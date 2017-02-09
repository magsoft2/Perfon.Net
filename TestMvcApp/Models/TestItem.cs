using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestMvcApp.Models
{
    public class TestItem
    {
        public string Caption { get; set; }
        public int Id { get; set; }
        public bool Completed { get; set; }
        public DateTime Timestamp { get; set; }
    }
}