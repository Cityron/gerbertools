using GerberBackend.Core.DbContext;
using GerberBackend.Core.Entities.Auth;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GerberBackend.Core.Entities.UserStoreCustom;

public class UserStoreCustom : UserStore<ApplicationUser>
{
    public UserStoreCustom(ApplicationContext context, IdentityErrorDescriber describer = null)
        : base(context, describer)
    {

    }
    public virtual Task<ApplicationUser> FindByPhoneNumberAsync(string PhoneNumber, CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        return Users.FirstOrDefaultAsync(u => u.PhoneNumber == PhoneNumber, cancellationToken);
    }
}
