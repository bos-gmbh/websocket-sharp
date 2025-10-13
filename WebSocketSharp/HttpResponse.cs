#region License

/*
 * HttpResponse.cs
 *
 * The MIT License
 *
 * Copyright (c) 2012-2014 sta.blockhead
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

using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using WebSocketSharp.Net;

#pragma warning disable CS8625
namespace WebSocketSharp;

internal class HttpResponse : HttpBase
{
    #region Private Constructors

    private HttpResponse(string code, string reason, Version version, NameValueCollection headers)
        : base(version, headers)
    {
        StatusCode = code;
        Reason = reason;
    }

    #endregion

    #region Private Fields

    #endregion

    #region Internal Constructors

    internal HttpResponse(HttpStatusCode code)
        : this(code, code.GetDescription())
    {
    }

    internal HttpResponse(HttpStatusCode code, string reason)
        : this(((int)code).ToString(), reason, HttpVersion.Version11, new NameValueCollection())
    {
        Headers["Server"] = "WebSocketSharp/1.0";
    }

    #endregion

    #region Public Properties

    public CookieCollection Cookies => Headers.GetCookies(true);

    public bool HasConnectionClose => Headers.Contains("Connection", "close");

    public bool IsProxyAuthenticationRequired => StatusCode == "407";

    public bool IsRedirect => StatusCode == "301" || StatusCode == "302";

    public bool IsUnauthorized => StatusCode == "401";

    public bool IsWebSocketResponse
    {
        get
        {
            var headers = Headers;
            return ProtocolVersion > HttpVersion.Version10 &&
                   StatusCode == "101" &&
                   headers.Contains("Upgrade", "websocket") &&
                   headers.Contains("Connection", "Upgrade");
        }
    }

    public string Reason { get; }

    public string StatusCode { get; }

    #endregion

    #region Internal Methods

    internal static HttpResponse CreateCloseResponse(HttpStatusCode code)
    {
        var res = new HttpResponse(code);
        res.Headers["Connection"] = "close";

        return res;
    }

    internal static HttpResponse CreateUnauthorizedResponse(string challenge)
    {
        var res = new HttpResponse(HttpStatusCode.Unauthorized);
        res.Headers["WWW-Authenticate"] = challenge;

        return res;
    }

    internal static HttpResponse CreateWebSocketResponse()
    {
        var res = new HttpResponse(HttpStatusCode.SwitchingProtocols);

        var headers = res.Headers;
        headers["Upgrade"] = "websocket";
        headers["Connection"] = "Upgrade";

        return res;
    }

    internal static HttpResponse Parse(string[] headerParts)
    {
        var statusLine = headerParts[0].Split(new[] { ' ' }, 3);
        if (statusLine.Length != 3)
            throw new ArgumentException($"Invalid status line: {headerParts[0]}");

        var headers = new WebHeaderCollection();
        for (var i = 1; i < headerParts.Length; i++)
            headers.InternalSet(headerParts[i], true);

        return new HttpResponse(
            statusLine[1], statusLine[2], new Version(statusLine[0].Substring(5)), headers);
    }

    internal static HttpResponse Read(Stream stream, int millisecondsTimeout)
    {
        return Read(stream, Parse, millisecondsTimeout);
    }

    #endregion

    #region Public Methods

    public void SetCookies(CookieCollection cookies)
    {
        if (cookies == null || cookies.Count == 0)
            return;

        var headers = Headers;
        foreach (var cookie in cookies.Sorted)
            headers.Add("Set-Cookie", cookie.ToResponseString());
    }

    public override string ToString()
    {
        var output = new StringBuilder(64);
        output.AppendFormat("HTTP/{0} {1} {2}{3}", ProtocolVersion, StatusCode, Reason, CrLf);

        var headers = Headers;
        foreach (var key in headers.AllKeys)
            output.AppendFormat("{0}: {1}{2}", key, headers[key], CrLf);

        output.Append(CrLf);

        var entity = EntityBody;
        if (entity.Length > 0)
            output.Append(entity);

        return output.ToString();
    }

    #endregion
}
