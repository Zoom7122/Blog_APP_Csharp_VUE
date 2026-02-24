using System;
using System.Collections.Generic;
using System.Text;

namespace BlogAPP_Core.Models
{
    public class UpdateArticleModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public List<string> Tag { get; set; } = new List<string>();

        public string CoverImage { get; set; }

        public string Description { get; set; }

        public string Text { get; set; }

        public int ReadTime { get; set; }
    }
}
