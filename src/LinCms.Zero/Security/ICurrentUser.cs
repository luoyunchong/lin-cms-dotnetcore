namespace LinCms.Zero.Security
{
    public interface ICurrentUser
    {
        int? Id { get; }

        string UserName { get; }

        int? GroupId { get; }

        bool? IsAdmin { get; }

        string GetFileUrl(string hash);
    }
}
