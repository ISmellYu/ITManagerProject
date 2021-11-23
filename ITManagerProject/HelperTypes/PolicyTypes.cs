namespace ITManagerProject.HelperTypes
{
    public static class PolicyTypes
    {
        public static class Users
        {
            public const string Manage = "Users.Manage.Policy";
            public const string View = "Users.View.Policy";
            public const string Edit = "Users.Edit.Policy";
        }
        
        public static class Organization
        {
            public const string Remove = "Organization.Remove.Policy";
            public const string ManageApplications = "Organization.ManageApplications.Policy";
            
        }
    }
}