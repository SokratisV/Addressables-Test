using UnityEngine;

namespace Email
{
    [CreateAssetMenu(fileName = "EmailSettings", menuName = "Email")]
    public class EmailSettings : ScriptableObject
    {
        public string smtpClient = "smtp.gmail.com";
        public int port = 587;
        public int timeout = 10000;
        public string mailAddressFrom = "nilelabgames@gmail.com";
        public string mailAddressTo = "sokvogiatzakis@gmail.com";
        public string subject = "ΟΔΗΓΙΕΣ ΠΛΟΗΓΗΣΗΣ-ΒΕΝΙΖΕΛΕΙΟ ΝΟΣΟΚΟΜΕΙΟ ΗΡΑΚΛΕΙΟΥ";
        public string body = "";
        public string fileImagePath = "/StreamingAssets/snap.png";
        public string userName = "nilelabgames@gmail.com";
        public string password = "n1l3g@m3l@b";
        public bool enableSSL;
    }
}