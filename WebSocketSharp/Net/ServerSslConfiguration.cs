﻿#region License

/*
 * ServerSslConfiguration.cs
 *
 * The MIT License
 *
 * Copyright (c) 2014 liryna
 * Copyright (c) 2014-2017 sta.blockhead
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

#endregion

#region Authors

/*
 * Authors:
 * - Liryna <liryna.stark@gmail.com>
 */

#endregion

using System;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

#pragma warning disable CS8625
namespace WebSocketSharp.Net;

/// <summary>
///     Stores the parameters for the <see cref="SslStream" /> used by servers.
/// </summary>
public class ServerSslConfiguration
{
    #region Private Fields

    private RemoteCertificateValidationCallback _clientCertValidationCallback;

    #endregion

    #region Private Methods

    private static bool defaultValidateClientCertificate(
        object sender,
        X509Certificate certificate,
        X509Chain chain,
        SslPolicyErrors sslPolicyErrors
    )
    {
        return true;
    }

    #endregion

    #region Public Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServerSslConfiguration" /> class.
    /// </summary>
    public ServerSslConfiguration()
    {
        EnabledSslProtocols = SslProtocols.Default;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServerSslConfiguration" /> class
    ///     with the specified <paramref name="serverCertificate" />.
    /// </summary>
    /// <param name="serverCertificate">
    ///     A <see cref="X509Certificate2" /> that represents the certificate used to
    ///     authenticate the server.
    /// </param>
    public ServerSslConfiguration(X509Certificate2 serverCertificate)
    {
        ServerCertificate = serverCertificate;
        EnabledSslProtocols = SslProtocols.Default;
    }

    /// <summary>
    ///     Copies the parameters from the specified <paramref name="configuration" /> to
    ///     a new instance of the <see cref="ServerSslConfiguration" /> class.
    /// </summary>
    /// <param name="configuration">
    ///     A <see cref="ServerSslConfiguration" /> from which to copy.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="configuration" /> is <see langword="null" />.
    /// </exception>
    public ServerSslConfiguration(ServerSslConfiguration configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        CheckCertificateRevocation = configuration.CheckCertificateRevocation;
        ClientCertificateRequired = configuration.ClientCertificateRequired;
        _clientCertValidationCallback = configuration._clientCertValidationCallback;
        EnabledSslProtocols = configuration.EnabledSslProtocols;
        ServerCertificate = configuration.ServerCertificate;
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///     Gets or sets a value indicating whether the certificate revocation
    ///     list is checked during authentication.
    /// </summary>
    /// <value>
    ///     <para>
    ///         <c>true</c> if the certificate revocation list is checked during
    ///         authentication; otherwise, <c>false</c>.
    ///     </para>
    ///     <para>
    ///         The default value is <c>false</c>.
    ///     </para>
    /// </value>
    public bool CheckCertificateRevocation { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the client is asked for
    ///     a certificate for authentication.
    /// </summary>
    /// <value>
    ///     <para>
    ///         <c>true</c> if the client is asked for a certificate for
    ///         authentication; otherwise, <c>false</c>.
    ///     </para>
    ///     <para>
    ///         The default value is <c>false</c>.
    ///     </para>
    /// </value>
    public bool ClientCertificateRequired { get; set; }

    /// <summary>
    ///     Gets or sets the callback used to validate the certificate
    ///     supplied by the client.
    /// </summary>
    /// <remarks>
    ///     The certificate is valid if the callback returns <c>true</c>.
    /// </remarks>
    /// <value>
    ///     <para>
    ///         A <see cref="RemoteCertificateValidationCallback" /> delegate that
    ///         invokes the method called for validating the certificate.
    ///     </para>
    ///     <para>
    ///         The default value is a delegate that invokes a method that
    ///         only returns <c>true</c>.
    ///     </para>
    /// </value>
    public RemoteCertificateValidationCallback ClientCertificateValidationCallback
    {
        get
        {
            if (_clientCertValidationCallback == null)
                _clientCertValidationCallback = defaultValidateClientCertificate;

            return _clientCertValidationCallback;
        }

        set => _clientCertValidationCallback = value;
    }

    /// <summary>
    ///     Gets or sets the protocols used for authentication.
    /// </summary>
    /// <value>
    ///     <para>
    ///         The <see cref="SslProtocols" /> enum values that represent
    ///         the protocols used for authentication.
    ///     </para>
    ///     <para>
    ///         The default value is <see cref="SslProtocols.Default" />.
    ///     </para>
    /// </value>
    public SslProtocols EnabledSslProtocols { get; set; }

    /// <summary>
    ///     Gets or sets the certificate used to authenticate the server.
    /// </summary>
    /// <value>
    ///     <para>
    ///         A <see cref="X509Certificate2" /> or <see langword="null" />
    ///         if not specified.
    ///     </para>
    ///     <para>
    ///         That instance represents an X.509 certificate.
    ///     </para>
    /// </value>
    public X509Certificate2 ServerCertificate { get; set; }

    #endregion
}
