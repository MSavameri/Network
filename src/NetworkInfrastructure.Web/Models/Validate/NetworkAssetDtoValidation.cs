using FluentValidation;

namespace NetworkInfrastructure.Web.Models.Validate
{
    public class NetworkAssetDtoValidation : AbstractValidator<NetworkAssetDto>
    {
        public NetworkAssetDtoValidation()
        {
            RuleFor(x => x.ServerName).NotEmpty().NotNull().MaximumLength(50);
            RuleFor(x => x.Ip).NotEmpty().NotNull().MaximumLength(15);
            RuleFor(x => x.BackupType).NotNull();
            RuleFor(x => x.ServiceOwner).NotEmpty().NotNull().MaximumLength(50);
            RuleFor(x => x.OsType).NotNull();
            RuleFor(x => x.LocationName).NotNull();
            RuleFor(x => x.Description).NotEmpty().NotNull().MaximumLength(100);
            RuleFor(x => x.LastUpdate).NotEmpty().NotNull();
            RuleFor(x => x.ServerPort).NotEmpty().NotNull().MaximumLength(5);
        }
    }
}
