using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using transporterQuote.API;
using transporterQuote.Services;
using System.Runtime.Caching;
using System.Web;
using transporterQuote.Models;
using System.Security.Cryptography;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Web.Configuration;

namespace transporterQuote.API
{
    [RoutePrefix("api/gen")]
    public class genApiController : ApiController
    {
        #region "Functions"

        private const int DerivationIterations = 1000;
        private const int Keysize = 256;

        // getProductName() - Get the display name of our product.
        public static string getProductName()
        {
            return "Vendorum"; 
        }

        // getURL() - Get the project's URL.
        public static string getURL()
        {
            if (WebConfigurationManager.AppSettings["Access"] == "local")
            {
                return "http://localhost:53770/";
            } else
            {
                return "http://gap4.actplease.com/";
            }
        }

        // getPersonTkn() - Get a person's token.
        public static token getPersonTkn()
        {
            // Extract my tokenID
            string myTokenID = HttpContext.Current.Request.Headers["majama60"];

            // Get token object from cache
            MemoryCache memCache = MemoryCache.Default;
            var res = memCache.Get(myTokenID);

            if (res == null)
            {
                return null;
            }
            else
            {
                token myToken = (token)res;
                return myToken;
            }
        }

        // setPersonTkn() - Set a person's token.
        public static void setPersonTkn(token newToken)
        {
            MemoryCache tokenCache = MemoryCache.Default;
            DateTimeOffset expiryTime = new DateTimeOffset();

            expiryTime = DateTimeOffset.UtcNow.AddHours(1);

            // Add token object to cache
            tokenCache.Add(newToken.tokenID, newToken, expiryTime);
        }

        // getStates() - Call internal function to get a list of states.
        [HttpPost, HttpGet, Route("states/get")]
        public dynamic getStates(dynamic myParams)
        {
            return new jResponse(false, "", getIndianStates());
        }

        // getIndianState() - Get a list of Indian states.
        public static List<state> getIndianStates()
        {
            List<state> states = new List<state>();
            states.Add(new state(1, "Andaman & Nicobar Islands"));
            states.Add(new state(2, "Andhra Pradesh"));
            states.Add(new state(3, "Arunachal Pradesh"));
            states.Add(new state(4, "Assam"));
            states.Add(new state(5, "Bihar"));
            states.Add(new state(6, "Chandigarh"));
            states.Add(new state(7, "Chhatisgarh"));
            states.Add(new state(8, "Dadra & Nagar Haveli"));
            states.Add(new state(9, "Daman & Diu"));
            states.Add(new state(10, "Delhi"));
            states.Add(new state(11, "Goa"));
            states.Add(new state(12, "Gujarat"));
            states.Add(new state(13, "Haryana"));
            states.Add(new state(14, "Himachal Pradesh"));
            states.Add(new state(15, "Jammu & Kashmir"));
            states.Add(new state(16, "Jharkhand"));
            states.Add(new state(17, "Karnataka"));
            states.Add(new state(18, "Kerala"));
            states.Add(new state(19, "Lakshadweep"));
            states.Add(new state(20, "Madhya Pradesh"));
            states.Add(new state(21, "Maharashtra"));
            states.Add(new state(22, "Manipur"));
            states.Add(new state(23, "Meghalaya"));
            states.Add(new state(24, "Mizoram"));
            states.Add(new state(25, "Nagaland"));
            states.Add(new state(26, "Odisha"));
            states.Add(new state(27, "Puducherry"));
            states.Add(new state(28, "Punjab"));
            states.Add(new state(29, "Rajasthan"));
            states.Add(new state(30, "Sikkim"));
            states.Add(new state(31, "Tamil Nadu"));
            states.Add(new state(32, "Telangana"));
            states.Add(new state(33, "Tripura"));
            states.Add(new state(34, "Uttar Pradesh"));
            states.Add(new state(35, "Uttarakhand"));
            states.Add(new state(36, "West Bengal"));

            return states;
        }


        // getWorldCountries() - Call internal function to get a list of country.
        [HttpPost, HttpGet, Route("countries/get")]
        public dynamic getWorldCountries(dynamic myParams)
        {
            return new jResponse(false, "", getCountries());
        }

        // getIndianState() - Get a list of Indian states.
        public static List<country> getCountries()
        {

            List<country> countries = new List<country>();
            countries.Add(new country("Afghanistan"));
            countries.Add(new country("Albania"));
            countries.Add(new country("Algeria"));
            countries.Add(new country("Andorra"));
            countries.Add(new country("Angola"));
            countries.Add(new country("Antigua and Barbuda"));
            countries.Add(new country("Argentina"));
            countries.Add(new country("Armenia"));
            countries.Add(new country("Australia"));
            countries.Add(new country("Austria"));
            countries.Add(new country("Azerbaijan"));
            countries.Add(new country("The Bahamas"));
            countries.Add(new country("Bahrain"));
            countries.Add(new country("Bangladesh"));
            countries.Add(new country("Barbados"));
            countries.Add(new country("Belarus"));
            countries.Add(new country("Belgium"));
            countries.Add(new country("Belize"));
            countries.Add(new country("Benin"));
            countries.Add(new country("Bhutan"));
            countries.Add(new country("Bolivia"));
            countries.Add(new country("Bosnia and Herzegovina"));
            countries.Add(new country("Botswana"));
            countries.Add(new country("Brazil"));
            countries.Add(new country("Brunei"));
            countries.Add(new country("Bulgaria"));
            countries.Add(new country("Burkina Faso"));
            countries.Add(new country("Burundi"));
            countries.Add(new country("Cabo Verde"));
            countries.Add(new country("Cambodia"));
            countries.Add(new country("Cameroon"));
            countries.Add(new country("Canada"));
            countries.Add(new country("Central African Republic"));
            countries.Add(new country("Chad"));
            countries.Add(new country("Chile"));
            countries.Add(new country("China"));
            countries.Add(new country("Colombia"));
            countries.Add(new country("Comoros"));
            countries.Add(new country("Congo, Democratic Republic of the"));
            countries.Add(new country("Congo, Republic of the"));
            countries.Add(new country("Costa Rica"));
            countries.Add(new country("Côte d’Ivoire"));
            countries.Add(new country("Croatia"));
            countries.Add(new country("Cuba"));
            countries.Add(new country("Cyprus"));
            countries.Add(new country("Czech Republic"));
            countries.Add(new country("Denmark"));
            countries.Add(new country("Djibouti"));
            countries.Add(new country("Dominica"));
            countries.Add(new country("Dominican Republic"));
            countries.Add(new country("East Timor(Timor - Leste)"));
            countries.Add(new country("Ecuador"));
            countries.Add(new country("Egypt"));
            countries.Add(new country("El Salvador"));
            countries.Add(new country("Equatorial Guinea"));
            countries.Add(new country("Eritrea"));
            countries.Add(new country("Estonia"));
            countries.Add(new country("Ethiopia"));
            countries.Add(new country("Fiji"));
            countries.Add(new country("Finland"));
            countries.Add(new country("France"));
            countries.Add(new country("Gabon"));
            countries.Add(new country("The Gambia"));
            countries.Add(new country("Georgia"));
            countries.Add(new country("Germany"));
            countries.Add(new country("Ghana"));
            countries.Add(new country("Greece"));
            countries.Add(new country("Grenada"));
            countries.Add(new country("Guatemala"));
            countries.Add(new country("Guinea"));
            countries.Add(new country("Guinea - Bissau"));
            countries.Add(new country("Guyana"));
            countries.Add(new country("Haiti"));
            countries.Add(new country("Honduras"));
            countries.Add(new country("Hungary"));
            countries.Add(new country("Iceland"));
            countries.Add(new country("India"));
            countries.Add(new country("Indonesia"));
            countries.Add(new country("Iran"));
            countries.Add(new country("Iraq"));
            countries.Add(new country("Ireland"));
            countries.Add(new country("Israel"));
            countries.Add(new country("Italy"));
            countries.Add(new country("Jamaica"));
            countries.Add(new country("Japan"));
            countries.Add(new country("Jordan"));
            countries.Add(new country("Kazakhstan"));
            countries.Add(new country("Kenya"));
            countries.Add(new country("Kiribati"));
            countries.Add(new country("Korea, North"));
            countries.Add(new country("Korea, South"));
            countries.Add(new country("Kosovo"));
            countries.Add(new country("Kuwait"));
            countries.Add(new country("Kyrgyzstan"));
            countries.Add(new country("Laos"));
            countries.Add(new country("Latvia"));
            countries.Add(new country("Lebanon"));
            countries.Add(new country("Lesotho"));
            countries.Add(new country("Liberia"));
            countries.Add(new country("Libya"));
            countries.Add(new country("Liechtenstein"));
            countries.Add(new country("Lithuania"));
            countries.Add(new country("Luxembourg"));
            countries.Add(new country("Macedonia"));
            countries.Add(new country("Madagascar"));
            countries.Add(new country("Malawi"));
            countries.Add(new country("Malaysia"));
            countries.Add(new country("Maldives"));
            countries.Add(new country("Mali"));
            countries.Add(new country("Malta"));
            countries.Add(new country("Marshall Islands"));
            countries.Add(new country("Mauritania"));
            countries.Add(new country("Mauritius"));
            countries.Add(new country("Mexico"));
            countries.Add(new country("Micronesia, Federated States of"));
            countries.Add(new country("Moldova"));
            countries.Add(new country("Monaco"));
            countries.Add(new country("Mongolia"));
            countries.Add(new country("Montenegro"));
            countries.Add(new country("Morocco"));
            countries.Add(new country("Mozambique"));
            countries.Add(new country("Myanmar(Burma)"));
            countries.Add(new country("Namibia"));
            countries.Add(new country("Nauru"));
            countries.Add(new country("Nepal"));
            countries.Add(new country("Netherlands"));
            countries.Add(new country("New Zealand"));
            countries.Add(new country("Nicaragua"));
            countries.Add(new country("Niger"));
            countries.Add(new country("Nigeria"));
            countries.Add(new country("Norway"));
            countries.Add(new country("Oman"));
            countries.Add(new country("Pakistan"));
            countries.Add(new country("Palau"));
            countries.Add(new country("Panama"));
            countries.Add(new country("Papua New Guinea"));
            countries.Add(new country("Paraguay"));
            countries.Add(new country("Peru"));
            countries.Add(new country("Philippines"));
            countries.Add(new country("Poland"));
            countries.Add(new country("Portugal"));
            countries.Add(new country("Qatar"));
            countries.Add(new country("Romania"));
            countries.Add(new country("Russia"));
            countries.Add(new country("Rwanda"));
            countries.Add(new country("Saint Kitts and Nevis"));
            countries.Add(new country("Saint Lucia"));
            countries.Add(new country("Saint Vincent and the Grenadines"));
            countries.Add(new country("Samoa"));
            countries.Add(new country("San Marino"));
            countries.Add(new country("Sao Tome and Principe"));
            countries.Add(new country("Saudi Arabia"));
            countries.Add(new country("Senegal"));
            countries.Add(new country("Serbia"));
            countries.Add(new country("Seychelles"));
            countries.Add(new country("Sierra Leone"));
            countries.Add(new country("Singapore"));
            countries.Add(new country("Slovakia"));
            countries.Add(new country("Slovenia"));
            countries.Add(new country("Solomon Islands"));
            countries.Add(new country("Somalia"));
            countries.Add(new country("South Africa"));
            countries.Add(new country("Spain"));
            countries.Add(new country("Sri Lanka"));
            countries.Add(new country("Sudan"));
            countries.Add(new country("Sudan, South"));
            countries.Add(new country("Suriname"));
            countries.Add(new country("Swaziland"));
            countries.Add(new country("Sweden"));
            countries.Add(new country("Switzerland"));
            countries.Add(new country("Syria"));
            countries.Add(new country("Taiwan"));
            countries.Add(new country("Tajikistan"));
            countries.Add(new country("Tanzania"));
            countries.Add(new country("Thailand"));
            countries.Add(new country("Togo"));
            countries.Add(new country("Tonga"));
            countries.Add(new country("Trinidad and Tobago"));
            countries.Add(new country("Tunisia"));
            countries.Add(new country("Turkey"));
            countries.Add(new country("Turkmenistan"));
            countries.Add(new country("Tuvalu"));
            countries.Add(new country("Uganda"));
            countries.Add(new country("Ukraine"));
            countries.Add(new country("United Arab Emirates"));
            countries.Add(new country("United Kingdom"));
            countries.Add(new country("United States"));
            countries.Add(new country("Uruguay"));
            countries.Add(new country("Uzbekistan"));
            countries.Add(new country("Vanuatu"));
            countries.Add(new country("Vatican City"));
            countries.Add(new country("Venezuela"));
            countries.Add(new country("Vietnam"));
            countries.Add(new country("Yemen"));
            countries.Add(new country("Zambia"));
            countries.Add(new country("Zimbabwe"));



            return countries;
        }

        // getDate() - Get current DT.
        public static DateTime getDate()
        {
            DateTime dt = new DateTime();
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                dt = db.Database.SqlQuery<DateTime>("SELECT GETDATE()").FirstOrDefault<DateTime>();
            }
            return dt;
        }

        // #region "Encrypt-Decrypt URL"

        public static string encryptURL(string data, string passPhrase)
        {

            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(data);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                Console.WriteLine(cipherTextBytes);
                                return Convert.ToBase64String(cipherTextBytes);

                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        public static string decryptURL(string cipherText, string passPhrase)
        {
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        // #endregion

        // #region "New Encrypt-Decrypt URL"
        public static string encryptParam(string data)
        {
            string cipherText = data;
            string enc = "";
            string str = cipherText;
            int key = 456;
            string inputText = str;
            for (int i = 0; i < inputText.Length; i++)
            {
                str = ((char)(inputText[i] ^ key)).ToString();
                enc = enc + str;
            }

            return enc;
        }

        // #endregion

        /*
        - splitString()
        - Purpose: Split a string into list of strings.
        - In: delimeter, myString
        - Out: List<string>
        */
        public List<string> splitString(char delimeter, string myString)
        {
            string[] optionIDsArray = Array.ConvertAll(myString.Split(delimeter), s => s.ToString());
            return optionIDsArray.ToList();
        }

        // getEncryptedPW() - Get encrypted password
        public static string getEncryptedPW(string password)
        {

            string functionReturnValue = "'";
            int X = 0;
            string sRet = null;
            string sChar = null;
            long lChar = 0;
            for (X = 1; X <= password.Length; X++)
            {
                sChar = mid(password, X, 1);
                lChar = Convert.ToChar(sChar);
                if ((lChar > 128))
                {
                    lChar = lChar - 128;

                }
                else if (lChar < 128)
                {
                    lChar = lChar + 128;
                }
                else
                {
                    lChar = 128;
                }
                sRet = sRet + Convert.ToChar(lChar);
            }
            functionReturnValue = sRet;


            return functionReturnValue;
        }

        // mid() - Use of substring
        public static string mid(string s, int a, int b)
        {
            string temp = s.Substring(a - 1, b);
            return temp;
        }

        // getConfigValue() - Get config value.
        [HttpPost, HttpGet, Route("config/get")]
        public dynamic getConfigValue(dynamic myParams)
        {
            return getConfig();
        }

        // getConfig() - Get config value
        public dynamic getConfig()
        {
            dynamic dbConfig = null;

            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                dbConfig = db.Configs.ToList();
            }
            return new jResponse(false, "", dbConfig);
        }


        #endregion

        #region "Classes"

        public class state
        {
            public int id { get; set; }
            public string name { get; set; }

            public state(int id, string name)
            {
                this.id = id;
                this.name = name;
            }
        }

        public class country
        {
            public string name { get; set; }

            public country(string name)
            {
                this.name = name;
            }
        }

        #endregion
    }
}