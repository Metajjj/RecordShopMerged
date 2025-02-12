using System.ComponentModel.DataAnnotations;

namespace RecordShop_BE.Tables
{
    public class Albums
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } = "";
        public string Artist { get; set; } = "";
        public DateOnly ReleaseDate { get; set; }        
    }
}
