using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace MQTTnet.Client
{
    public class MqttClientOptionsBuilderTlsParameters
    {
        public bool UseTls { get; set; }

        [Obsolete("This property will be removed soon. Use CertificateValidationHandler instead.")]
        public Func<X509Certificate, X509Chain, SslPolicyErrors, IMqttClientOptions, bool> CertificateValidationCallback
        {
            get;
            set;
        }

        public Func<MqttClientCertificateValidationCallbackContext, bool> CertificateValidationHandler { get; set; }

        public SslProtocols SslProtocol { get; set; } = SslProtocols.None;

#if WINDOWS_UWP
        public IEnumerable<IEnumerable<byte>> Certificates { get; set; }
#else
        public IEnumerable<X509Certificate> Certificates { get; set; }
#endif

#if NETCOREAPP3_1 || NET5_0_OR_GREATER
        public List<SslApplicationProtocol> ApplicationProtocols { get;set; }
#endif

	    public bool AllowUntrustedCertificates { get; set; }

        public bool IgnoreCertificateChainErrors { get; set; }

        public bool IgnoreCertificateRevocationErrors { get; set; }
    }
}
