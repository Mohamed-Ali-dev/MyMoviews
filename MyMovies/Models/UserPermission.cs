namespace MyMovies.Models
{
    public class UserPermission
    {
        public int UserId { get; set; }
        public Permission PermissionId { get; set; }
    }
    public enum Permission
    {
        Read = 1,
        Add,
        Edit,
        Delete
    }
}
