using GetImagesApi.Data.Entities.Identity;

namespace GetImagesApi.Interfaces
{
    public interface IJwtTokenService
    {
        Task<string> CreateToken(UserEntity user);
    }
}
