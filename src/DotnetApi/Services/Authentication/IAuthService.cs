using DotnetApi.Models.Authentication;
using DotnetApi.Models.Common;
using DotnetApi.Models.Users;

namespace DotnetApi.Services.Authentication;

public interface IAuthService
{
    /// <summary>
    ///     Main method for logging in a user. Checks the user exists using the
    ///     userRepository, that the credentials match, and in the future, that
    ///     they are not inactive/banned/locked, before returning a successful
    ///     login. 
    /// </summary>
    /// <param name="credentials">
    ///     The user's email and password, wrapped as a single LoginDto
    /// </param>
    /// <returns>
    ///     The JWT
    /// </returns>
    public Task<Result<TokenResponseDto>> Login(LoginDto credentials);
    
    /// <summary>
    ///     Creates a new user, handling sign-ups.
    /// </summary>
    /// <param name="user">A CreateUserDto, built from user inputs</param>
    /// <returns>The new user's JWT</returns>
    public Task<Result<TokenResponseDto>> Register(RegisterDto user);
    
    /// <summary>
    ///     Revokes exactly one token based on it's hash
    /// </summary>
    /// <param name="refreshToken">The token passed by the client to be revoked for logging out a specific device</param>
    /// <returns>The new user's JWT</returns>
    public Task Logout(string refreshToken);
    
    /// <summary>
    ///     Issues a new refresh token for the user, revoking their previous token
    /// </summary>
    /// <param name="refreshToken">the previous refresh token that was used to re-authenticate the client. Will be revoked during the refresh after a new token is issued.</param>
    /// <param name="user">The user related to the token refresh, used to issue a new jwt</param>
    /// <returns>Returns a TokenResponseDto containing both the new jwt and refresh token</returns>
    public Task<Result<TokenResponseDto>> Refresh(string refreshToken, User user);
    
    /// <summary>
    ///     Revokes all tokens for a specific user, enabling logging out of all devices.
    /// </summary>
    /// <param name="userId">the id related to the user logging out of all devices</param>
    /// <returns></returns>
    public Task LogoutOfAllDevices(Guid userId);

}