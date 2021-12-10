using System;
using System.Collections.Generic;
using System.Linq;
using ITManagerProject.Models;

namespace ITManagerProject.HelperTypes;

public static class RoleTypesString
{
    public static string CEO = "CEO";
    public static string CTO = "CTO";
    public static string CIO = "CIO";
    public static string HeadofProduct = "Head of Product";
    public static string ProductManager = "Product Manager";
    public static string VPofMarketing = "VP of Marketing";
    public static string DirectorofEngineering = "Director of Engineering";
    public static string ChiefArchitect = "Chief Architect";
    public static string SoftwareArchitect = "Software Architect";
    public static string EngineeringProjectManager = "Engineering Project Manager";
    public static string TechnicalLead = "Technical Lead";
    public static string PrincipalSoftwareEngineer = "Principal Software Engineer";
    public static string SeniorSoftwareEngineer = "Senior Software Engineer";
    public static string SoftwareEngineer = "Software Engineer";
    public static string SoftwareDeveloper = "Software Developer";
    public static string JuniorSoftwareDeveloper = "Junior Software Developer";
    public static string InternSoftwareDeveloper = "Intern Software Developer";
    public static string Candidate = "Candidate";

    public static List<string> AllRolesAvailable = new()
    {
        CEO, CTO, CIO, HeadofProduct, ProductManager, VPofMarketing, DirectorofEngineering, ChiefArchitect,
        SoftwareArchitect, EngineeringProjectManager, TechnicalLead, PrincipalSoftwareEngineer,
        SeniorSoftwareEngineer, SoftwareEngineer, SoftwareDeveloper, JuniorSoftwareDeveloper, 
        InternSoftwareDeveloper, Candidate
    };

    public static List<Role> GetAllRolesToBeSeeded()
    {
        var kl = new List<Role>();
        for (int i = 0; i < AllRolesAvailable.Count; i++)
        {
            var rl = new Role(AllRolesAvailable[i]);
            rl.Id = i + 1;
            rl.NormalizedName = AllRolesAvailable[i].ToUpper();
            rl.ConcurrencyStamp = Guid.NewGuid().ToString();
            kl.Add(rl);
        }
        return kl;
    }

}