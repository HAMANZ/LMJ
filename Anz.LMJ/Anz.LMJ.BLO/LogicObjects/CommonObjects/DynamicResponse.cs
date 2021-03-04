using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.BLO.LogicObjects.CommonObjects
{
    public class DynamicResponse<T>
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public string ServerMessage { get; set; }

    }
}
