using Microsoft.AspNetCore.Identity;

namespace Shopper.Models
{
    public class ErrorString : IdentityErrorDescriber
    {
        public override IdentityError PasswordTooShort(int lenght)
        => new IdentityError
        {
            Code = nameof(PasswordTooShort),
            Description = "رمز عبور باید حداقل 8 حرق باشد"
        };

        public override IdentityError InvalidUserName(string userName)
            => new IdentityError
            {
                Code = nameof(InvalidUserName),
                Description = "نام کاربری فقط باید متشکل از A_Z a_z . باشد"
            };

        public override IdentityError PasswordRequiresLower()
            => new IdentityError
            {
                Code = nameof(PasswordRequiresLower),
                Description = "رمز عبور باید حداقل یک حرف کوچک باشد"
            };

        public override IdentityError PasswordRequiresUpper()
            => new IdentityError
            {
                Code = nameof(PasswordRequiresUpper),
                Description = "رمز عبور باید دارای حداقل یک حرف بزرگ باشد"
            };
    }
}
