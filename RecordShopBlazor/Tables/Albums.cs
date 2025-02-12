using System.ComponentModel.DataAnnotations;

namespace RecordShop_FE.Tables
{
    public class Albums
    {
        [Key]
        [Range(0,uint.MaxValue)]
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Artist { get; set; } = "";
        public DateOnly ReleaseDate { get; set; }        
    }
}
