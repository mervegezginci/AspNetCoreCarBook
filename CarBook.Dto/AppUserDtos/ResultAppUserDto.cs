namespace CarBook.Dto.AppUserDtos
{
    public class ResultAppUserDto
    {
        public int AppUserId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public int AppRoleId { get; set; }
        public string RoleName { get; set; }
    }
}
