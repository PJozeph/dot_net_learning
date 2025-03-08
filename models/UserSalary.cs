namespace DOT_NET_API
{
    public partial class UserSalary
    {
        public int UserId { get; set; }
        public decimal Salary { get; set; }

        public UserSalary()
        {
            if (Salary == 0)
            {
                Salary = 0;
            }
        }

    }

}