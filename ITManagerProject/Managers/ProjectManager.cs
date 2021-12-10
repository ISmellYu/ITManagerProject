using ITManagerProject.Models;

namespace ITManagerProject.Managers;

public class ProjectManager
{
    private OrganizationManager<Organization> OrganizationManager;

    public ProjectManager(OrganizationManager<Organization> organizationManager)
    {
        OrganizationManager = organizationManager;
    }
}