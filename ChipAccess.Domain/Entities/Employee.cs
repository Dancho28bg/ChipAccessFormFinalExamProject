namespace ChipAccess.Domain.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string BamId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } = "User";
        public bool IsActive { get; set; } = true;
    }
}