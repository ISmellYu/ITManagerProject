﻿namespace ITManagerProject.HelperTypes;

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
        public const string ManageOrganization = "Organization.ManageOrganization.Policy";
        public const string ManageSalaries = "Organization.ManageSalaries.Policy";
        public const string ManageNotifications = "Organization.ManageNotifications.Policy";
        public const string ManageMeetings = "Organization.ManageMeetings.Policy";
        public const string ManageProjects = "Organization.ManageProjects.Policy";
    }
}