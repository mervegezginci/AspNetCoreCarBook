using System;
using System.Collections.Generic;
using System.Text;

namespace CarBook.Dto.CommentDtos
{
    public class ResultCommentDto
    {
        public int CommentID { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
        public int BlogID { get; set; }
        public string BlogTitle { get; set; }
    }
}
