namespace GiRest.Templates
{
    public class AuthorizationHeader
    {
        [PropertyName("Authorization")]
        public string Token { get; set; }
        public AuthorizationHeader(string token, bool withBearer = true)
        {
            Token = (withBearer ? "Bearer " : "") + token;
        }
    }
}
