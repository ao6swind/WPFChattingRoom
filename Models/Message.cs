namespace Models
{
    class Message
    {
        public User From { get; set; }
        public User To { get; set; }
        public string Content { get; set; }
    }
}
