namespace DevSkill.Inventory.Domain.Dtos
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public Guid ApplicationUserId { get; set; }
        public string? ProfilePicturePath { get; set; }
    }
}
