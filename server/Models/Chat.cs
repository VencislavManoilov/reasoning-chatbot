using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    [Table("chats")]
    public class Chat
    {
        public int id { get; set; }
        public int userid { get; set; }
        public string? title { get; set; }
        public string? messages { get; set; }
        private DateTime _createdat;
        private DateTime _updatedat;

        public DateTime createdat
        {
            get => _createdat;
            set => _createdat = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public DateTime updatedat
        {
            get => _updatedat;
            set => _updatedat = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public Chat()
        {
            _createdat = DateTime.UtcNow;
            _updatedat = DateTime.UtcNow;
        }
    }
}