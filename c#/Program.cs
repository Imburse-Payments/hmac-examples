using System;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HmacGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var publicKey = "bfaa9a45-5c5a-4bbc-b353-eb6989486c87";
            var privateKey = "x7uNykwHN3F4ATXMqVcWcCqrc2PnNId7mZ0Ei6IfIMY=";

            var httpRequestBodyContent = "";
            var nonce = CreateNonce();
            var timeStamp = (int)(DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds;
            
            //hash the body
            var hashedBody = SignBody(httpRequestBodyContent);
            //create the unsigned signature
            var unsignedSignature = $"{publicKey}:{nonce}:{timeStamp}:{hashedBody}";
            //generate the hmac signature
            var hmacToken = CreateToken(privateKey, unsignedSignature);
            //create signed signature
            var signedSignature = $"{publicKey}:{nonce}:{timeStamp}:{hmacToken}";

            Console.WriteLine($"hmac {signedSignature}");
            Console.Read();
        }


        public static string CreateToken(string secret, string message)
        {
            //must be UTF8 (incase there are special characters - same byte size but different encoding page
            //var encoding = new System.Text.ASCIIEncoding(); //


            byte[] keyByte = Convert.FromBase64String(secret);
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }

        private static string CreateNonce()
        {
            return DateTime.UtcNow.Ticks.ToString();
        }

        private static string SignBody(string body)
        {

            if (body == string.Empty)
                return body;

            var bodyBytes = System.Text.Encoding.UTF8.GetBytes(body);
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(bodyBytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
