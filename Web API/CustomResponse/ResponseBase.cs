using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_API.CustomResponse
{
    public abstract class ResponseBase
    {
        protected bool _Success;
        protected object _Data;

        public abstract bool Success { get; }
        public abstract object Data { get; set; }
    }
}