using System;
using System.Collections.Generic;
using System.Text;

namespace BlogAPP_BLL.Exceptions
{
    public class ArticleException : Exception
    {
        public ArticleException() { }
        public ArticleException(string message) : base(message) { }
        public ArticleException(string message, Exception inner) : base(message, inner) { }
    }
}
