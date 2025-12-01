using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libqbo.Configuration;

public sealed class QboConfiguration
{
    [Required]
    public required string ClientId { get; set; }
    [Required]
    public required string ClientSecret { get; set; }
    [Required]
    public required string RedirectUrl { get; set; }

    [Required]
    public required bool UseSandbox { get; set; }
}
