using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace TestServer
{
    [EnableCors(origins: "*", headers: "*", methods: "GET,POST")]
    public class TestController:ApiController
    {
        private static Random rand = new Random();

        public async Task<IHttpActionResult> Get1()
        {
            var array = new double[1000];

            for (var i = 0; i < array.Length; i++)
            {
                array[i] = rand.NextDouble();
            }

            return Ok(array);
        }

        public async Task<IHttpActionResult> Post([FromBody]string body)
        {
            return Ok(body);
        }
               
    }
}