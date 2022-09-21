using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteIndexer.Models
{
    public class TransactionResult<T> : TransactionResult
    {
        public T ReturnValue;

        public TransactionResult()
        {
            Succeeded = false;
            ReturnValue = default(T);
            ErrorMessage = string.Empty;
        }
    }

    public class TransactionResult
    {
        public bool Succeeded;
        public string ErrorMessage;

        public TransactionResult()
        {
            Succeeded = false;
            ErrorMessage = string.Empty;
        }
    }
}