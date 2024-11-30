using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ReviewAggregatorWebApp.Model;

public partial class User : IdentityUser<int>
{
    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public override string ToString()
    {
        return $"Login: {Login}; Password: {Password}; E-mail: {Email}.";
    }
}
