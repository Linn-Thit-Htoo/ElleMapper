using System;
using System.Collections.Generic;
using ElleMapper;

[Table("tbl_blog")]
public class Tbl_blog
{
    [Key]
    [Identity]
    public int BlogId { get; set; }
    public int? AuthorId { get; set; }
    public string BlogTitle { get; set; }
    public string BlogAuthor { get; set; }
    public string BlogContent { get; set; }
    public sbyte IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual Tbl_author tbl_author { get; set; }
}
