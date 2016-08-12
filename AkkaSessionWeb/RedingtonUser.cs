using System;
using System.Collections.Generic;

namespace AkkaSessionWeb
{
    public class RedingtonUser
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool CanDisable { get; set; }
        public bool Disabled { get; set; }
        public DateTime? DisabledTill { get; set; }
        public List<string> SelectedRoles { get; set; }
        public bool CanSetPassword { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string SharescreenId { get; set; }
    }
}