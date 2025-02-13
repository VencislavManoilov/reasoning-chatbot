namespace App.Models {
    public class session_data {
        public required string id { get; set; }
        public required byte[] value { get; set; }
        public DateTime expires_at { get; set; }
    }
}