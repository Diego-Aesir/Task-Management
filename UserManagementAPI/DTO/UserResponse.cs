namespace UserManagementAPI.DTO
{
    public class UserResponse
    {
        public required string Id { get; set; }
        public required string UserName { get; set; }
        public required string JWT { get; set; }
    }
}
