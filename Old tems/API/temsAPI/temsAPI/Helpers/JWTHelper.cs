using System;
using JWT;
using JWT.Algorithms;
using JWT.Exceptions;
using JWT.Serializers;

namespace temsAPI.Helpers
{
    

    public class JWTHelper
    {
        private class JwtToken_Helper
        {
            public long Exp { get; set; }
        }

        readonly IJsonSerializer _serializer = new JsonNetSerializer();
        readonly IDateTimeProvider _provider = new UtcDateTimeProvider();
        readonly IBase64UrlEncoder _urlEncoder = new JwtBase64UrlEncoder();
        readonly IJwtAlgorithm _algorithm = new HMACSHA256Algorithm();

        public DateTime GetExpiryTimestamp(string accessToken)
        {
            try
            {
                IJwtValidator _validator = new JwtValidator(_serializer, _provider);
                IJwtDecoder decoder = new JwtDecoder(_serializer, _validator, _urlEncoder, _algorithm);
                var token = decoder.DecodeToObject<JwtToken_Helper>(accessToken);
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(token.Exp);
                return dateTimeOffset.LocalDateTime;
            }
            catch (TokenExpiredException)
            {
                return DateTime.MinValue;
            }
            catch (SignatureVerificationException)
            {
                return DateTime.MinValue;
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }
    }
}
