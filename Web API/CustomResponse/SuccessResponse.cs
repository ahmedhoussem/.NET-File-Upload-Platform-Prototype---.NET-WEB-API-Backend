using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_API.CustomResponse
{
    public class SuccessResponse : ResponseBase
    {
        public override object Data
        {
            get
            {
                return this._Data;
            }

            set
            {
                _Data = value;
            }
        }

        public override bool Success
        {
            get
            {
                return true;
            }

        }
    }
}