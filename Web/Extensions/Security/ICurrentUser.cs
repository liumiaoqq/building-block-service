using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Web
{
    public interface  ICurrentUser
    {
        public Guid? UserId { get; }

  
        public abstract Claim GetCliam(string type);

        public abstract Guid GetUserId();

        public abstract RoleType GetRoleType();

        public bool IsUser();

    }
}
