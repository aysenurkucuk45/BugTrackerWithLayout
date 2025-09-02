using BugTrackerWithLayout.Models;
using System;

public class Solution
{
    public int Id { get; set; }
    public string Description { get; set; }
    public string Author { get; set; }  
    public DateTime CreatedAt { get; set; }

    public int BugId { get; set; }
    public virtual Bug Bug { get; set; }
    

}
