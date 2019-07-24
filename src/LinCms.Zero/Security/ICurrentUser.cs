namespace LinCms.Zero.Security
{
    public interface ICurrentUser
    {
        int? Id { get; }

        int? GroupId { get; }

        bool? IsAdmin { get; }
    }
}
