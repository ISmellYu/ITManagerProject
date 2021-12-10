using System.Collections.Generic;
using System.Threading.Tasks;
using ITManagerProject.CustomAttributes;
using ITManagerProject.HelperTypes;
using ITManagerProject.Managers;
using ITManagerProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITManagerProject.Controllers;

public class NotificationController : Controller
{
    private readonly OrganizationManager<Organization> _organizationManager;
    private readonly NotificationManager _notificationManager;

    public NotificationController(OrganizationManager<Organization> organizationManager, NotificationManager notificationManager)
    {
        _organizationManager = organizationManager;
        _notificationManager = notificationManager;
    }
        
    [HttpPost]
    [InOrganization]
    public async Task<List<Notification>> GetNotifications()
    {
        var user = await _organizationManager.UserManager.GetUserAsync(User);
        var organization = await _organizationManager.GetOrganizationFromUserAsync(user);
        var notifications = await _notificationManager.GetNotifications(organization.Id);
        return notifications;
    }

    [HttpPost]
    [InOrganization]
    [Authorize(Policy = PolicyTypes.Organization.ManageNotifications)]
    public async Task<bool> RemoveNotification(int notificationId)
    {
        await _notificationManager.RemoveNotification(notificationId);
        return true;
    }

    [HttpPost]
    [InOrganization]
    [Authorize(Policy = PolicyTypes.Organization.ManageNotifications)]
    public async Task<bool> AddNotification(string title, string body)
    {
        var user = await _organizationManager.UserManager.GetUserAsync(User);
        var organization = await _organizationManager.GetOrganizationFromUserAsync(user);

        await _notificationManager.AddNotification(title, body, organization.Id, user.Id);
        return true;
    }
}