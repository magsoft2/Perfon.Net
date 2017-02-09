using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestMvcApp.Models
{
    public class TestItemsContext
    {
        public TestItemsContext()
        {
            Items = new List<TestItem>();

            Items.Add(new TestItem { Caption="Test 1", Id = 0, Timestamp=DateTime.Now });
            Items.Add(new TestItem { Caption = "Test 2", Id = 1, Timestamp = DateTime.Now, Completed=true });
            Items.Add(new TestItem { Caption = "Test 3", Id = 2, Timestamp = DateTime.Now });
        }

        public List<TestItem> Items { get; set; }
    }
}