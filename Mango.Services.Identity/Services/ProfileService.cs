
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using Mango.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Mango.Services.Identity.Services;
public class ProfileService : IProfileService
{
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly UserManager<ApplicationUser> _userMgr;
    private readonly RoleManager<IdentityRole> _roleMgr;

    public ProfileService(
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactor, 
        UserManager<ApplicationUser> userMgr, RoleManager<IdentityRole> roleMgr)
    {
        _userClaimsPrincipalFactory = userClaimsPrincipalFactor;
        _userMgr = userMgr;
        _roleMgr = roleMgr;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        string sub = context.Subject.GetSubjectId();
        ApplicationUser user = await _userMgr.FindByIdAsync(sub);
        ClaimsPrincipal userClaims = await _userClaimsPrincipalFactory.CreateAsync(user);

        List<Claim> claims = userClaims.Claims.ToList();
        claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();

        if (_userMgr.SupportsUserRole)
        {
            IList<string> roles = await _userMgr.GetRolesAsync(user);
            foreach (var roleName in roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, roleName));
                claims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName));
                claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName));

                if (_roleMgr.SupportsRoleClaims)
                {
                    IdentityRole role = await _roleMgr.FindByNameAsync(roleName);
                    if (role != null)
                    {
                        claims.AddRange(await _roleMgr.GetClaimsAsync(role));
                    }
                }
            }
        }

        context.IssuedClaims = claims;
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        string sub = context.Subject.GetSubjectId();
        ApplicationUser user = await _userMgr.FindByIdAsync(sub);
        context.IsActive = user != null;
    }
}
