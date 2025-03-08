namespace DOT_NET_API
{
    public partial class UserJobinfo
    {
        public int UserId { get; set; }
        public string JobTitle { get; set; }
        public string JobDescription { get; set; }

        public UserJobinfo()
        {
            if (JobTitle == null)
            {
                JobTitle = "";
            }
            if (JobDescription == null)
            {
                JobDescription = "";
            }

        }

    }
}