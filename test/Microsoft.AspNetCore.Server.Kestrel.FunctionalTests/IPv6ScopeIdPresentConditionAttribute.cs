// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Microsoft.AspNetCore.Testing.xunit;

namespace Microsoft.AspNetCore.Server.Kestrel.FunctionalTests
{
    public class IPv6ScopeIdPresentConditionAttribute : Attribute, ITestCondition
    {
        private static readonly Lazy<bool> _ipv6Supported = new Lazy<bool>(CanBindToIPv6Address);

        public bool IsMet
        {
            get
            {
                return _ipv6Supported.Value;
            }
        }

        public string SkipReason
        {
            get
            {
                return "No IPv6 addresses with scope IDs were found on the host.";
            }
        }

        private static bool CanBindToIPv6Address()
        {
            try
            {
                return NetworkInterface.GetAllNetworkInterfaces()
                    .Where(i => i.OperationalStatus == OperationalStatus.Up)
                    .SelectMany(i => i.GetIPProperties().UnicastAddresses)
                    .Select(a => a.Address)
                    .Where(ip => ip.AddressFamily == AddressFamily.InterNetworkV6)
                    .Where(ip => ip.ScopeId != 0)
                    .Any();
            }
            catch (SocketException)
            {
                return false;
            }
        }
    }
}