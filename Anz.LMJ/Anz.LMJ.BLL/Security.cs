//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Collections;
//using System.Text;
//using System.Security.Cryptography;
//using System.Collections.Specialized;
//using Anz.LMJ.BLO;

//namespace Anz.LMJ.BLL
//{
//    public static class Security
//    {

//        public static String sign(IDictionary<string, string> paramsArray)
//        {
//            return sign(buildDataToSign(paramsArray), Keys.tac_secret_key);
//        }

//        private static String sign(String data, String secretKey)
//        {
//            UTF8Encoding encoding = new System.Text.UTF8Encoding();
//            byte[] keyByte = encoding.GetBytes(secretKey);

//            HMACSHA256 hmacsha256 = new HMACSHA256(keyByte);
//            byte[] messageBytes = encoding.GetBytes(data);
//            return Convert.ToBase64String(hmacsha256.ComputeHash(messageBytes));
//        }

//        private static String buildDataToSign(IDictionary<string, string> paramsArray)
//        {
//            String[] signedFieldNames = paramsArray["signed_field_names"].Split(',');
//            IList<string> dataToSign = new List<string>();

//            foreach (String signedFieldName in signedFieldNames)
//            {
//                dataToSign.Add(signedFieldName + "=" + paramsArray[signedFieldName]);
//            }

//            return commaSeparate(dataToSign);
//        }

//        private static String commaSeparate(IList<string> dataToSign)
//        {
//            return String.Join(",", dataToSign);
//        }
//    }
//}
