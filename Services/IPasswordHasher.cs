
internal interface IPasswordHasher
{
    string GenerateSalt();
    string HashPassword(string password, string salt);
}