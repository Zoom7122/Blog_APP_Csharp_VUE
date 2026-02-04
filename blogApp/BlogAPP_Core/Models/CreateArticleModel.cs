using System;
using System.Collections.Generic;
using System.Text;

namespace BlogAPP_Core.Models
{
    public class CreateArticleModel
    {
        public string Title { get; set; }

        public string CoverImage { get; set; }

        public string description { get; set; }

        public string text { get; set; }

        public int ReadTime { get; set; }

        //public List<string> tags { get; set; }
    }
}
