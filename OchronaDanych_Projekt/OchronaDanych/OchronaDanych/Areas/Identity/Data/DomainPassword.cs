namespace OchronaDanych.Areas.Identity.Data
{
    public class DomainPassword
    {
        public int ID { get; set; } 
        public ApplicationUser User { get; set; }
        public string Domain { get; set; }
        public string Password { get; set; }
    }
}
