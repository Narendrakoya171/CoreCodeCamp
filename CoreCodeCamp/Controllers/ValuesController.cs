using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
   
        [Route("api/[controller]")]
        public class ValuesController
        {

            public string[] Get()
            {
                return new[] { "Hello", "From", "Pluralsight" };
            }
        }
    }

