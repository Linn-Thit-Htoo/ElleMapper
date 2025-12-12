using System;
using System.Collections.Generic;
using ElleMapper;

[View("get_blog")]
public class Get_blog
{
    public int BlogId { get; set; }
    public int? AuthorId { get; set; }
    public string BlogTitle { get; set; }
    public string BlogAuthor { get; set; }
    public string BlogContent { get; set; }
    public sbyte IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
}
