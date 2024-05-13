using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eduChain.ViewModels.ProfileViewModels
{
    internal interface IProfileViewModel
    {
        Task LoadProfileAsync(string e, object o);
        Task UpdateProfilePicture();
        Task UpdateProfileAsync();
    }
}
