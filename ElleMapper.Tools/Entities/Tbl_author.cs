using System;
using System.Collections.Generic;
using ElleMapper;

[Table("tbl_author")]
public class Tbl_author
{
    [Key]
    [Identity]
    public int AuthorId { get; set; }
    public string AuthorName { get; set; }
    public virtual ICollection<Tbl_blog> tbl_blogs { get; set; } = new List<tbl_blog>();
}
