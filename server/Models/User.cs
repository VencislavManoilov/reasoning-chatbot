using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    [Table("users")]
    public class User
    {
        public int id { get; set; }
        public required string name { get; set; }
        public required string email { get; set; }
        public required string password { get; set; }
        public bool isactive { get; set; } = true;
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

        public User()
        {
            _createdat = DateTime.UtcNow;
            _updatedat = DateTime.UtcNow;
        }
    }
}